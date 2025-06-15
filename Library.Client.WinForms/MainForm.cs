using Library.API.Models;
using Library.Client.WinForms.Models;
using Microsoft.VisualBasic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Library.Client.WinForms
{
    public partial class MainForm : Form
    {
        // HttpClient nesnesini uygulama boyunca tek bir kere oluþturup yeniden kullanmak en iyi pratiktir.
        private static readonly HttpClient client = new HttpClient();
        private static string? _jwtToken = null; // Giriþ yaptýktan sonra alýnacak JWT'yi saklamak için.
                                                 // API'nin temel adresini buraya yaz.
                                                 // Projeyi çalýþtýrdýðýnda API'nin hangi portta çalýþtýðýný kontrol et ve gerekirse güncelle.
        private const string ApiBaseUrl = "https://localhost:7090/";

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Sadece API adresini ayarla ve baþlangýçta butonlarý pasif yap.
            // Baþka HÝÇBÝR GÖRÜNÜRLÜK ayarý yok.
            client.BaseAddress = new Uri(ApiBaseUrl);
            ToggleBookOperationControls(false);


        }

        private void ToggleBookOperationControls(bool isEnabled)
        {
            // Bu metot, SADECE GÝRÝÞ GEREKTÝREN kontrollerin durumunu ayarlar.

            btnGetAllBooks.Enabled = isEnabled;
            btnGetBookById.Enabled = isEnabled;
            txtBookId.Enabled = isEnabled;
            btnDeleteBook.Enabled = isEnabled;
            btnHtmlReport.Enabled = isEnabled; // Rapor için de giriþ gereksin mi? Þimdilik evet.

            // Ekleme ve Güncelleme kontrolleri
            btnAddBook.Enabled = isEnabled;
            btnUpdateBook.Enabled = isEnabled;
            txtTitle.Enabled = isEnabled;
            txtAuthor.Enabled = isEnabled;
            // txtIsbn.Enabled = isEnabled;  // <-- BU SATIRI SÝLDÝK/YORUM YAPTIK
            txtYear.Enabled = isEnabled;
            txtGenre.Enabled = isEnabled;
            txtMemberFirstName.Enabled = isEnabled;
            txtMemberLastName.Enabled = isEnabled;
            txtMemberEmail.Enabled = isEnabled;
            btnAddMember.Enabled = isEnabled;
            btnDeleteMember.Enabled = isEnabled;
            btnGetAllLoans.Enabled = isEnabled;
            btnReturnBook.Enabled = isEnabled;

            // Not: btnFetchBookDetails butonu bu listeye hiç eklenmemeli ki her zaman aktif kalsýn.
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            var loginData = new { username = txtUsername.Text, password = txtPassword.Text };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(loginData), System.Text.Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync("api/auth/login", content);
                if (response.IsSuccessStatusCode)
                {
                    var tokenObject = await response.Content.ReadFromJsonAsync<JsonElement>();
                    _jwtToken = tokenObject.GetProperty("token").GetString();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

                    // Sadece durum etiketini güncelle ve kontrolleri aktif et.
                    lblAuthStatus.Text = $"Status: Logged in as {txtUsername.Text}";
                    lblAuthStatus.ForeColor = Color.Green;
                    ToggleBookOperationControls(true);
                    MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnLogin.Enabled = false;

                }
                else
                {
                    lblAuthStatus.Text = "Status: Login Failed";
                    lblAuthStatus.ForeColor = Color.Red;
                    MessageBox.Show("Login failed. Please check your credentials.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void btnGetAllBooks_Click(object sender, EventArgs e)
        {
            // Giriþ yapýlýp yapýlmadýðýný kontrol et (_jwtToken dolu mu?)
            if (string.IsNullOrEmpty(_jwtToken))
            {
                MessageBox.Show("You must be logged in to perform this action.", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // API'nin /api/books endpoint'ine GET isteði gönder.
                // Token'ý zaten HttpClient'ýn varsayýlan baþlýklarýna eklediðimiz için tekrar eklememize gerek yok.
                HttpResponseMessage response = await client.GetAsync("api/books");

                if (response.IsSuccessStatusCode)
                {
                    // Gelen XML cevabýný string olarak al.
                    string xmlContent = await response.Content.ReadAsStringAsync();

                    // XML'i C# nesnelerine dönüþtür (Deserialization).
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<Book>), new System.Xml.Serialization.XmlRootAttribute("Books"));

                    List<Book> books;
                    using (var reader = new System.IO.StringReader(xmlContent))
                    {
                        // API'miz <Library><Books>... yapýsýnda döndüðü için bu kýsmý atlamamýz gerek.
                        // Bunun yerine direkt gelen XML'i XDocument ile iþleyip Book listesine çevirelim.
                        var doc = System.Xml.Linq.XDocument.Parse(xmlContent);
                        books = doc.Descendants("Book").Select(b => new Book
                        {
                            Id = (int)b.Attribute("ID"),
                            Title = b.Element("Title")?.Value,
                            Author = b.Element("Author")?.Value,
                            Isbn = b.Element("ISBN")?.Value,
                            PublicationYear = (int)b.Element("PublicationYear"),
                            Genre = b.Element("Genre")?.Value
                        }).ToList();
                    }

                    // DataGridView'in veri kaynaðýný bu kitap listesi olarak ayarla.
                    dgvBooks.DataSource = books;
                    MessageBox.Show($"{books.Count} books loaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Eðer istek baþarýsýzsa (örn: token geçersizse, 401 hatasý)
                    MessageBox.Show($"Failed to load books. Status: {response.ReasonPhrase}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnGetBookById_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtBookId.Text, out int bookId))
            {
                MessageBox.Show("Please enter a valid numeric ID.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/books/{bookId}");

                if (response.IsSuccessStatusCode)
                {
                    string xmlContent = await response.Content.ReadAsStringAsync();
                    var doc = System.Xml.Linq.XDocument.Parse(xmlContent);

                    string bookDetails = $"ID: {doc.Root.Attribute("ID")?.Value}\n" +
                                         $"Title: {doc.Root.Element("Title")?.Value}\n" +
                                         $"Author: {doc.Root.Element("Author")?.Value}\n" +
                                         $"Year: {doc.Root.Element("PublicationYear")?.Value}";

                    MessageBox.Show(bookDetails, "Book Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show($"Book with ID {bookId} was not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"Failed to get book. Status: {response.ReasonPhrase}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDeleteBook_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtBookId.Text, out int bookId))
            {
                MessageBox.Show("Please enter a valid numeric ID.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kullanýcýya onay sorusu soralým, bu iyi bir pratiktir.
            var confirmation = MessageBox.Show($"Are you sure you want to delete the book with ID {bookId}?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmation == DialogResult.No)
            {
                return;
            }

            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"api/books/{bookId}");

                if (response.IsSuccessStatusCode) // DELETE için 204 No Content döner
                {
                    MessageBox.Show($"Book with ID {bookId} has been deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Tabloyu yenilemek için Get All Books butonunun click olayýný tekrar çaðýrabiliriz.
                    // Bu, en kolay yenileme yöntemidir.
                    btnGetAllBooks.PerformClick();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show($"Book with ID {bookId} was not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"Failed to delete book. Status: {response.ReasonPhrase}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnAddBook_Click(object sender, EventArgs e)
        {
            // Gerekli alanlarýn dolu olup olmadýðýný kontrol et
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Title and Author are required fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kullanýcýdan alýnan verilerle bir XML string'i oluþturalým.
            // Bu, API'nin beklediði <Book> yapýsýyla eþleþmeli.
            string bookXml = $@"
    <Book>
        <Title>{txtTitle.Text}</Title>
        <Author>{txtAuthor.Text}</Author>
        <ISBN>{txtIsbn.Text}</ISBN>
        <PublicationYear>{txtYear.Text}</PublicationYear>
        <Genre>{txtGenre.Text}</Genre>
    </Book>";

            // XML string'ini HTTP isteði için içeriðe dönüþtür.
            var content = new StringContent(bookXml, System.Text.Encoding.UTF8, "application/xml");

            try
            {
                HttpResponseMessage response = await client.PostAsync("api/books", content);

                if (response.IsSuccessStatusCode) // POST için 201 Created döner
                {
                    MessageBox.Show("Book added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Alanlarý temizle ve tabloyu yenile
                    ClearInputFields();
                    btnGetAllBooks.PerformClick();
                }
                else
                {
                    // API'den gelen validasyon hatasý gibi detaylý hatalarý gösterelim.
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to add book. Status: {response.ReasonPhrase}\nDetails: {errorContent}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Metin kutularýný temizlemek için yardýmcý bir metot
        private void ClearInputFields()
        {
            txtTitle.Clear();
            txtAuthor.Clear();
            txtIsbn.Clear();
            txtYear.Clear();
            txtGenre.Clear();
        }

        private void dgvBooks_SelectionChanged(object sender, EventArgs e)
        {
            // Eðer seçili bir satýr varsa ve bu satýr bir kitaba aitse...
            if (dgvBooks.SelectedRows.Count > 0 && dgvBooks.SelectedRows[0].DataBoundItem is Book selectedBook)
            {
                // Seçilen kitabýn bilgilerini metin kutularýna doldur.
                txtTitle.Text = selectedBook.Title;
                txtAuthor.Text = selectedBook.Author;
                txtIsbn.Text = selectedBook.Isbn;
                txtYear.Text = selectedBook.PublicationYear.ToString();
                txtGenre.Text = selectedBook.Genre;

                // Güncellenecek kitabýn ID'sini de bir yerde tutmak faydalý olur.
                // txtBookId kutusunu bu amaçla kullanabiliriz.
                txtBookId.Text = selectedBook.Id.ToString();
            }
        }

        private async void btnUpdateBook_Click(object sender, EventArgs e)
        {
            // Güncellenecek kitabýn ID'sini al
            if (!int.TryParse(txtBookId.Text, out int bookId))
            {
                MessageBox.Show("Please select a book from the list to update, or enter a valid ID in the 'Get Book by ID' box.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Gerekli alanlarýn dolu olup olmadýðýný kontrol et
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Title and Author are required fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Metin kutularýndaki verilerle bir XML string'i oluþturalým.
            string bookXml = $@"
    <Book>
        <Title>{txtTitle.Text}</Title>
        <Author>{txtAuthor.Text}</Author>
        <ISBN>{txtIsbn.Text}</ISBN>
        <PublicationYear>{txtYear.Text}</PublicationYear>
        <Genre>{txtGenre.Text}</Genre>
    </Book>";

            var content = new StringContent(bookXml, System.Text.Encoding.UTF8, "application/xml");

            try
            {
                HttpResponseMessage response = await client.PutAsync($"api/books/{bookId}", content);

                if (response.IsSuccessStatusCode) // PUT için 204 No Content döner
                {
                    MessageBox.Show("Book updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Alanlarý temizle ve tabloyu yenile
                    ClearInputFields();
                    txtBookId.Clear();
                    btnGetAllBooks.PerformClick();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to update book. Status: {response.ReasonPhrase}\nDetails: {errorContent}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnHtmlReport_Click(object sender, EventArgs e)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("api/books/report");

                if (response.IsSuccessStatusCode)
                {
                    string htmlContent = await response.Content.ReadAsStringAsync();

                    // Gelen HTML'i geçici bir dosyaya yaz
                    string filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "BookReport.html");
                    await System.IO.File.WriteAllTextAsync(filePath, htmlContent);

                    // Oluþturulan HTML dosyasýný varsayýlan tarayýcýda aç
                    var processInfo = new System.Diagnostics.ProcessStartInfo(filePath)
                    {
                        UseShellExecute = true // Bu satýr .NET Core/.NET 5+ için gereklidir
                    };
                    System.Diagnostics.Process.Start(processInfo);

                    MessageBox.Show("HTML report has been generated and opened in your browser.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Failed to generate report. Status: {response.ReasonPhrase}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnFetchBookDetails_Click(object sender, EventArgs e)
        {
            string isbn = txtIsbn.Text.Trim(); // Baþýndaki/sonundaki boþluklarý temizle
            if (string.IsNullOrEmpty(isbn))
            {
                MessageBox.Show("Please enter an ISBN to fetch details.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Open Library API'nin adresi
            string apiUrl = $"https://openlibrary.org/api/books?bibkeys=ISBN:{isbn}&format=json&jscmd=data";

            // Not: Open Library API ile konuþmak için yeni bir HttpClient kullanabiliriz
            // veya mevcut olaný kullanabiliriz. Ayrý bir client kullanmak bazen daha temiz olabilir.
            using (var externalApiClient = new HttpClient())
            {
                try
                {
                    // API'ye GET isteði gönder
                    HttpResponseMessage response = await externalApiClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        // ---- BU KISMI DEÐÝÞTÝRÝYORUZ ----
                        // API, ISBN'e karþýlýk bir þey bulamazsa boþ bir JSON nesnesi "{}" döndürür.
                        // Bunu metin olarak kontrol etmek en güvenilir yoldur.
                        if (jsonContent.Trim() == "{}")
                        {
                            MessageBox.Show($"No book found for ISBN: {isbn}", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        // Gelen JSON'u ayrýþtýr.
                        using (var jsonDoc = System.Text.Json.JsonDocument.Parse(jsonContent))
                        {
                            // JSON'un içindeki doðru yola ulaþalým: "ISBN:..." -> "title"
                            var bookData = jsonDoc.RootElement.GetProperty($"ISBN:{isbn}");

                            // Metin kutularýný dolduralým
                            txtTitle.Text = bookData.TryGetProperty("title", out var title) ? title.GetString() : "";

                            // Yazar bilgisi bir dizi (array) olarak gelebilir, ilkini alalým.
                            if (bookData.TryGetProperty("authors", out var authors) && authors.GetArrayLength() > 0)
                            {
                                txtAuthor.Text = authors[0].TryGetProperty("name", out var authorName) ? authorName.GetString() : "";
                            }
                            else
                            {
                                txtAuthor.Text = ""; // Yazar bulunamadýysa kutuyu temizle
                            }

                            // Yayýn yýlý
                            txtYear.Text = bookData.TryGetProperty("publish_date", out var publishDate) ? publishDate.GetString() : "";

                            DialogResult dialogResult = MessageBox.Show("Book details fetched successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        // ---------------------------------
                    }
                    else
                    {
                        MessageBox.Show($"Failed to fetch data from Open Library. Status: {response.ReasonPhrase}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while fetching book details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnGetAllMembers_Click(object sender, EventArgs e)
        {
            // Bu iþlem de giriþ gerektiriyor.
            if (string.IsNullOrEmpty(_jwtToken))
            {
                MessageBox.Show("You must be logged in to view members.", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // API'nin /api/v1/members endpoint'ine GET isteði gönder.
                HttpResponseMessage response = await client.GetAsync("api/members");

                if (response.IsSuccessStatusCode)
                {
                    // API bu sefer JSON döndürüyor.
                    var members = await response.Content.ReadFromJsonAsync<List<MemberDetailDto>>();

                    if (members != null)
                    {
                        dgvMembers.DataSource = members;
                        MessageBox.Show($"{members.Count} members loaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"Failed to load members. Status: {response.ReasonPhrase}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSearchBooks_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearchTerm.Text;
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                MessageBox.Show("Please enter a term to search for.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // URL'i manuel ve dikkatli bir þekilde oluþturalým.
            // BaseAddress'e GÜVENMEYEN, tam adres yöntemini SADECE BU METOT ÝÇÝN deneyelim.
            // Bu, sorunun BaseAddress ile ilgili olup olmadýðýný kesin olarak test eder.
            string requestUrl = $"{ApiBaseUrl}api/books/search?term={searchTerm}";

            try
            {
                // Önceki hata ayýklama mesajýný býrakalým, çok faydalý.
                Console.WriteLine($"SEARCH REQUEST URL: {requestUrl}");

                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var foundBooks = await response.Content.ReadFromJsonAsync<List<Book>>();
                    dgvBooks.DataSource = foundBooks;

                    if (foundBooks != null && foundBooks.Any())
                    {
                        MessageBox.Show($"{foundBooks.Count} book(s) found.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No books found matching your search term.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Sunucudan dönen hata mesajýný da gösterelim.
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Search failed. Status: {response.ReasonPhrase}\nDetails: {errorContent}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnLoanBook_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0 || dgvBooks.SelectedRows[0].DataBoundItem is not Book selectedBook)
            {
                MessageBox.Show("Please select a book from the list to loan.", "Selection Error");
                return;
            }

            if (!int.TryParse(txtLoanMemberId.Text, out int memberId))
            {
                MessageBox.Show("Please enter a valid numeric Member ID.", "Input Error");
                return;
            }

            // API'ye göndereceðimiz JSON Body'sini oluþtur.
            var loanRequest = new { bookId = selectedBook.Id, memberId = memberId };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(loanRequest), System.Text.Encoding.UTF8, "application/json");

            try
            {
                // POST /api/loans adresine istek at.
                HttpResponseMessage response = await client.PostAsync("api/loans", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Book loaned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // API'den gelen Conflict (409) gibi hatalarý yakala ve göster.
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to loan book. Status: {response.ReasonPhrase}\nDetails: {errorContent}", "API Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error");
            }
        }

        private async void btnAddMember_Click(object sender, EventArgs e)
        {
            // Gerekli alanlarýn dolu olup olmadýðýný kontrol et
            if (string.IsNullOrWhiteSpace(txtMemberFirstName.Text) || string.IsNullOrWhiteSpace(txtMemberLastName.Text))
            {
                MessageBox.Show("First Name and Last Name are required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // API'ye göndereceðimiz JSON Body'sini oluþtur.
            // API'deki Member modelimizle ayný özelliklere sahip bir nesne oluþturuyoruz.
            var newMemberData = new
            {
                firstName = txtMemberFirstName.Text,
                lastName = txtMemberLastName.Text,
                email = txtMemberEmail.Text,
                membershipDate = dtpMembershipDate.Value.ToString("yyyy-MM-dd")

            };

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(newMemberData), System.Text.Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync("api/members", content);

                if (response.IsSuccessStatusCode) // 201 Created döner
                {
                    MessageBox.Show("Member added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Alanlarý temizle ve üye listesini yenile
                    txtMemberFirstName.Clear();
                    txtMemberLastName.Clear();
                    txtMemberEmail.Clear();
                    btnGetAllMembers.PerformClick(); // Üye listesini güncelle
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to add member. Status: {response.ReasonPhrase}\nDetails: {errorContent}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDeleteMember_Click(object sender, EventArgs e)
        {
            // 1. Tablodan bir üye seçildi mi?
            if (dgvMembers.SelectedRows.Count == 0 || dgvMembers.SelectedRows[0].DataBoundItem is not MemberDetailDto selectedMember)
            {
                MessageBox.Show("Please select a member from the list to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kullanýcýya onay sorusu soralým.
            var confirmation = MessageBox.Show($"Are you sure you want to delete {selectedMember.FirstName} {selectedMember.LastName}?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmation == DialogResult.No)
            {
                return;
            }

            try
            {
                // DELETE api/members/{id} adresine istek at
                HttpResponseMessage response = await client.DeleteAsync($"api/members/{selectedMember.Id}");

                if (response.IsSuccessStatusCode) // 204 No Content döner
                {
                    MessageBox.Show("Member deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnGetAllMembers.PerformClick(); // Üye listesini yenile
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("The selected member was not found in the database. The list will be refreshed.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnGetAllMembers.PerformClick();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to delete member. Status: {response.ReasonPhrase}\nDetails: {errorContent}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvMembers_SelectionChanged(object sender, EventArgs e)
        {
            // Önce ListBox'ý temizle
            lstMemberLoans.Items.Clear();

            // Eðer seçili bir satýr varsa ve bu bir MemberDetailDto ise...
            if (dgvMembers.SelectedRows.Count > 0 && dgvMembers.SelectedRows[0].DataBoundItem is MemberDetailDto selectedMember)
            {
                // Eðer üyenin ödünç aldýðý kitap yoksa bir mesaj göster
                if (selectedMember.ActiveLoanBookIds == null || !selectedMember.ActiveLoanBookIds.Any())
                {
                    lstMemberLoans.Items.Add("This member has no active loans.");
                    return;
                }

                // Þimdi her bir Book ID için kitabýn baþlýðýný bulmamýz gerekiyor.
                // Bunun için en kolay yol, kitap listesini bir yerde saklamaktýr.
                // Geçici olarak, her seferinde books.xml'den okuyalým. (Daha performanslý bir yol da var)

                try
                {
                    // Kitap listesini DataGridView'in veri kaynaðýndan almayý deneyelim. Bu daha verimli.
                    if (dgvBooks.DataSource is List<Book> allBooks)
                    {
                        foreach (var bookId in selectedMember.ActiveLoanBookIds)
                        {
                            var book = allBooks.FirstOrDefault(b => b.Id == bookId);
                            if (book != null)
                            {
                                lstMemberLoans.Items.Add($"ID: {book.Id} - {book.Title}");
                            }
                            else
                            {
                                lstMemberLoans.Items.Add($"Book with ID {bookId} (Details not loaded)");
                            }
                        }
                    }
                    else
                    {
                        // Eðer kitaplar henüz yüklenmediyse bir uyarý ver
                        lstMemberLoans.Items.Add("Load books in 'Books' tab to see titles.");
                    }
                }
                catch
                {
                    lstMemberLoans.Items.Add("Error retrieving book titles.");
                }
            }
        }

        private async void btnGetAllLoans_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_jwtToken)) { MessageBox.Show("Please login first."); return; }

            try
            {
                // GET /api/loans adresine istek at
                HttpResponseMessage response = await client.GetAsync("api/loans");
                if (response.IsSuccessStatusCode)
                {
                    var loans = await response.Content.ReadFromJsonAsync<List<LoanDetailDto>>();
                    dgvLoans.DataSource = loans; // Yeni DataGridView'e veriyi baðla
                }
                else
                {
                    MessageBox.Show($"Error loading loan records: {response.ReasonPhrase}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnReturnBook_Click(object sender, EventArgs e)
        {
            // 1. Tablodan bir kayýt seçildi mi?
            if (dgvLoans.SelectedRows.Count == 0 || dgvLoans.SelectedRows[0].DataBoundItem is not LoanDetailDto selectedLoan)
            {
                MessageBox.Show("Please select a loan record from the list to return.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kayýt zaten iade edilmiþ mi?
            if (!string.IsNullOrEmpty(selectedLoan.ReturnDate))
            {
                MessageBox.Show("This book has already been returned.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int loanId = selectedLoan.LoanId; // <-- Ýþte LoanID'yi buradan kolayca alýyoruz.

            var confirmation = MessageBox.Show($"Are you sure you want to mark the loan for '{selectedLoan.BookTitle}' as returned?", "Confirm Return", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmation == DialogResult.No) return;

            try
            {
                // PUT api/loans/{loanId}/return adresine istek at
                HttpResponseMessage response = await client.PutAsync($"api/loans/{loanId}/return", null);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Book returned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Listeyi yenilemek için "Refresh All Loans" butonunu tetikle
                    btnGetAllLoans.PerformClick();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to return book. Status: {response.ReasonPhrase}\nDetails: {errorContent}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
