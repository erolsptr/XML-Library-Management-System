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
        // HttpClient nesnesini uygulama boyunca tek bir kere olu�turup yeniden kullanmak en iyi pratiktir.
        private static readonly HttpClient client = new HttpClient();
        private static string? _jwtToken = null; // Giri� yapt�ktan sonra al�nacak JWT'yi saklamak i�in.
                                                 // API'nin temel adresini buraya yaz.
                                                 // Projeyi �al��t�rd���nda API'nin hangi portta �al��t���n� kontrol et ve gerekirse g�ncelle.
        private const string ApiBaseUrl = "https://localhost:7090/";

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Sadece API adresini ayarla ve ba�lang��ta butonlar� pasif yap.
            // Ba�ka H��B�R G�R�N�RL�K ayar� yok.
            client.BaseAddress = new Uri(ApiBaseUrl);
            ToggleBookOperationControls(false);


        }

        private void ToggleBookOperationControls(bool isEnabled)
        {
            // Bu metot, SADECE G�R�� GEREKT�REN kontrollerin durumunu ayarlar.

            btnGetAllBooks.Enabled = isEnabled;
            btnGetBookById.Enabled = isEnabled;
            txtBookId.Enabled = isEnabled;
            btnDeleteBook.Enabled = isEnabled;
            btnHtmlReport.Enabled = isEnabled; // Rapor i�in de giri� gereksin mi? �imdilik evet.

            // Ekleme ve G�ncelleme kontrolleri
            btnAddBook.Enabled = isEnabled;
            btnUpdateBook.Enabled = isEnabled;
            txtTitle.Enabled = isEnabled;
            txtAuthor.Enabled = isEnabled;
            // txtIsbn.Enabled = isEnabled;  // <-- BU SATIRI S�LD�K/YORUM YAPTIK
            txtYear.Enabled = isEnabled;
            txtGenre.Enabled = isEnabled;
            txtMemberFirstName.Enabled = isEnabled;
            txtMemberLastName.Enabled = isEnabled;
            txtMemberEmail.Enabled = isEnabled;
            btnAddMember.Enabled = isEnabled;
            btnDeleteMember.Enabled = isEnabled;
            btnGetAllLoans.Enabled = isEnabled;
            btnReturnBook.Enabled = isEnabled;

            // Not: btnFetchBookDetails butonu bu listeye hi� eklenmemeli ki her zaman aktif kals�n.
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

                    // Sadece durum etiketini g�ncelle ve kontrolleri aktif et.
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
            // Giri� yap�l�p yap�lmad���n� kontrol et (_jwtToken dolu mu?)
            if (string.IsNullOrEmpty(_jwtToken))
            {
                MessageBox.Show("You must be logged in to perform this action.", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // API'nin /api/books endpoint'ine GET iste�i g�nder.
                // Token'� zaten HttpClient'�n varsay�lan ba�l�klar�na ekledi�imiz i�in tekrar eklememize gerek yok.
                HttpResponseMessage response = await client.GetAsync("api/books");

                if (response.IsSuccessStatusCode)
                {
                    // Gelen XML cevab�n� string olarak al.
                    string xmlContent = await response.Content.ReadAsStringAsync();

                    // XML'i C# nesnelerine d�n��t�r (Deserialization).
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<Book>), new System.Xml.Serialization.XmlRootAttribute("Books"));

                    List<Book> books;
                    using (var reader = new System.IO.StringReader(xmlContent))
                    {
                        // API'miz <Library><Books>... yap�s�nda d�nd��� i�in bu k�sm� atlamam�z gerek.
                        // Bunun yerine direkt gelen XML'i XDocument ile i�leyip Book listesine �evirelim.
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

                    // DataGridView'in veri kayna��n� bu kitap listesi olarak ayarla.
                    dgvBooks.DataSource = books;
                    MessageBox.Show($"{books.Count} books loaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // E�er istek ba�ar�s�zsa (�rn: token ge�ersizse, 401 hatas�)
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

            // Kullan�c�ya onay sorusu soral�m, bu iyi bir pratiktir.
            var confirmation = MessageBox.Show($"Are you sure you want to delete the book with ID {bookId}?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmation == DialogResult.No)
            {
                return;
            }

            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"api/books/{bookId}");

                if (response.IsSuccessStatusCode) // DELETE i�in 204 No Content d�ner
                {
                    MessageBox.Show($"Book with ID {bookId} has been deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Tabloyu yenilemek i�in Get All Books butonunun click olay�n� tekrar �a��rabiliriz.
                    // Bu, en kolay yenileme y�ntemidir.
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
            // Gerekli alanlar�n dolu olup olmad���n� kontrol et
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Title and Author are required fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kullan�c�dan al�nan verilerle bir XML string'i olu�tural�m.
            // Bu, API'nin bekledi�i <Book> yap�s�yla e�le�meli.
            string bookXml = $@"
    <Book>
        <Title>{txtTitle.Text}</Title>
        <Author>{txtAuthor.Text}</Author>
        <ISBN>{txtIsbn.Text}</ISBN>
        <PublicationYear>{txtYear.Text}</PublicationYear>
        <Genre>{txtGenre.Text}</Genre>
    </Book>";

            // XML string'ini HTTP iste�i i�in i�eri�e d�n��t�r.
            var content = new StringContent(bookXml, System.Text.Encoding.UTF8, "application/xml");

            try
            {
                HttpResponseMessage response = await client.PostAsync("api/books", content);

                if (response.IsSuccessStatusCode) // POST i�in 201 Created d�ner
                {
                    MessageBox.Show("Book added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Alanlar� temizle ve tabloyu yenile
                    ClearInputFields();
                    btnGetAllBooks.PerformClick();
                }
                else
                {
                    // API'den gelen validasyon hatas� gibi detayl� hatalar� g�sterelim.
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to add book. Status: {response.ReasonPhrase}\nDetails: {errorContent}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Metin kutular�n� temizlemek i�in yard�mc� bir metot
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
            // E�er se�ili bir sat�r varsa ve bu sat�r bir kitaba aitse...
            if (dgvBooks.SelectedRows.Count > 0 && dgvBooks.SelectedRows[0].DataBoundItem is Book selectedBook)
            {
                // Se�ilen kitab�n bilgilerini metin kutular�na doldur.
                txtTitle.Text = selectedBook.Title;
                txtAuthor.Text = selectedBook.Author;
                txtIsbn.Text = selectedBook.Isbn;
                txtYear.Text = selectedBook.PublicationYear.ToString();
                txtGenre.Text = selectedBook.Genre;

                // G�ncellenecek kitab�n ID'sini de bir yerde tutmak faydal� olur.
                // txtBookId kutusunu bu ama�la kullanabiliriz.
                txtBookId.Text = selectedBook.Id.ToString();
            }
        }

        private async void btnUpdateBook_Click(object sender, EventArgs e)
        {
            // G�ncellenecek kitab�n ID'sini al
            if (!int.TryParse(txtBookId.Text, out int bookId))
            {
                MessageBox.Show("Please select a book from the list to update, or enter a valid ID in the 'Get Book by ID' box.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Gerekli alanlar�n dolu olup olmad���n� kontrol et
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Title and Author are required fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Metin kutular�ndaki verilerle bir XML string'i olu�tural�m.
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

                if (response.IsSuccessStatusCode) // PUT i�in 204 No Content d�ner
                {
                    MessageBox.Show("Book updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Alanlar� temizle ve tabloyu yenile
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

                    // Gelen HTML'i ge�ici bir dosyaya yaz
                    string filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "BookReport.html");
                    await System.IO.File.WriteAllTextAsync(filePath, htmlContent);

                    // Olu�turulan HTML dosyas�n� varsay�lan taray�c�da a�
                    var processInfo = new System.Diagnostics.ProcessStartInfo(filePath)
                    {
                        UseShellExecute = true // Bu sat�r .NET Core/.NET 5+ i�in gereklidir
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
            string isbn = txtIsbn.Text.Trim(); // Ba��ndaki/sonundaki bo�luklar� temizle
            if (string.IsNullOrEmpty(isbn))
            {
                MessageBox.Show("Please enter an ISBN to fetch details.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Open Library API'nin adresi
            string apiUrl = $"https://openlibrary.org/api/books?bibkeys=ISBN:{isbn}&format=json&jscmd=data";

            // Not: Open Library API ile konu�mak i�in yeni bir HttpClient kullanabiliriz
            // veya mevcut olan� kullanabiliriz. Ayr� bir client kullanmak bazen daha temiz olabilir.
            using (var externalApiClient = new HttpClient())
            {
                try
                {
                    // API'ye GET iste�i g�nder
                    HttpResponseMessage response = await externalApiClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        // ---- BU KISMI DE���T�R�YORUZ ----
                        // API, ISBN'e kar��l�k bir �ey bulamazsa bo� bir JSON nesnesi "{}" d�nd�r�r.
                        // Bunu metin olarak kontrol etmek en g�venilir yoldur.
                        if (jsonContent.Trim() == "{}")
                        {
                            MessageBox.Show($"No book found for ISBN: {isbn}", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        // Gelen JSON'u ayr��t�r.
                        using (var jsonDoc = System.Text.Json.JsonDocument.Parse(jsonContent))
                        {
                            // JSON'un i�indeki do�ru yola ula�al�m: "ISBN:..." -> "title"
                            var bookData = jsonDoc.RootElement.GetProperty($"ISBN:{isbn}");

                            // Metin kutular�n� doldural�m
                            txtTitle.Text = bookData.TryGetProperty("title", out var title) ? title.GetString() : "";

                            // Yazar bilgisi bir dizi (array) olarak gelebilir, ilkini alal�m.
                            if (bookData.TryGetProperty("authors", out var authors) && authors.GetArrayLength() > 0)
                            {
                                txtAuthor.Text = authors[0].TryGetProperty("name", out var authorName) ? authorName.GetString() : "";
                            }
                            else
                            {
                                txtAuthor.Text = ""; // Yazar bulunamad�ysa kutuyu temizle
                            }

                            // Yay�n y�l�
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
            // Bu i�lem de giri� gerektiriyor.
            if (string.IsNullOrEmpty(_jwtToken))
            {
                MessageBox.Show("You must be logged in to view members.", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // API'nin /api/v1/members endpoint'ine GET iste�i g�nder.
                HttpResponseMessage response = await client.GetAsync("api/members");

                if (response.IsSuccessStatusCode)
                {
                    // API bu sefer JSON d�nd�r�yor.
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

            // URL'i manuel ve dikkatli bir �ekilde olu�tural�m.
            // BaseAddress'e G�VENMEYEN, tam adres y�ntemini SADECE BU METOT ���N deneyelim.
            // Bu, sorunun BaseAddress ile ilgili olup olmad���n� kesin olarak test eder.
            string requestUrl = $"{ApiBaseUrl}api/books/search?term={searchTerm}";

            try
            {
                // �nceki hata ay�klama mesaj�n� b�rakal�m, �ok faydal�.
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
                    // Sunucudan d�nen hata mesaj�n� da g�sterelim.
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

            // API'ye g�nderece�imiz JSON Body'sini olu�tur.
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
                    // API'den gelen Conflict (409) gibi hatalar� yakala ve g�ster.
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
            // Gerekli alanlar�n dolu olup olmad���n� kontrol et
            if (string.IsNullOrWhiteSpace(txtMemberFirstName.Text) || string.IsNullOrWhiteSpace(txtMemberLastName.Text))
            {
                MessageBox.Show("First Name and Last Name are required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // API'ye g�nderece�imiz JSON Body'sini olu�tur.
            // API'deki Member modelimizle ayn� �zelliklere sahip bir nesne olu�turuyoruz.
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

                if (response.IsSuccessStatusCode) // 201 Created d�ner
                {
                    MessageBox.Show("Member added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Alanlar� temizle ve �ye listesini yenile
                    txtMemberFirstName.Clear();
                    txtMemberLastName.Clear();
                    txtMemberEmail.Clear();
                    btnGetAllMembers.PerformClick(); // �ye listesini g�ncelle
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
            // 1. Tablodan bir �ye se�ildi mi?
            if (dgvMembers.SelectedRows.Count == 0 || dgvMembers.SelectedRows[0].DataBoundItem is not MemberDetailDto selectedMember)
            {
                MessageBox.Show("Please select a member from the list to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kullan�c�ya onay sorusu soral�m.
            var confirmation = MessageBox.Show($"Are you sure you want to delete {selectedMember.FirstName} {selectedMember.LastName}?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmation == DialogResult.No)
            {
                return;
            }

            try
            {
                // DELETE api/members/{id} adresine istek at
                HttpResponseMessage response = await client.DeleteAsync($"api/members/{selectedMember.Id}");

                if (response.IsSuccessStatusCode) // 204 No Content d�ner
                {
                    MessageBox.Show("Member deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnGetAllMembers.PerformClick(); // �ye listesini yenile
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
            // �nce ListBox'� temizle
            lstMemberLoans.Items.Clear();

            // E�er se�ili bir sat�r varsa ve bu bir MemberDetailDto ise...
            if (dgvMembers.SelectedRows.Count > 0 && dgvMembers.SelectedRows[0].DataBoundItem is MemberDetailDto selectedMember)
            {
                // E�er �yenin �d�n� ald��� kitap yoksa bir mesaj g�ster
                if (selectedMember.ActiveLoanBookIds == null || !selectedMember.ActiveLoanBookIds.Any())
                {
                    lstMemberLoans.Items.Add("This member has no active loans.");
                    return;
                }

                // �imdi her bir Book ID i�in kitab�n ba�l���n� bulmam�z gerekiyor.
                // Bunun i�in en kolay yol, kitap listesini bir yerde saklamakt�r.
                // Ge�ici olarak, her seferinde books.xml'den okuyal�m. (Daha performansl� bir yol da var)

                try
                {
                    // Kitap listesini DataGridView'in veri kayna��ndan almay� deneyelim. Bu daha verimli.
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
                        // E�er kitaplar hen�z y�klenmediyse bir uyar� ver
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
                    dgvLoans.DataSource = loans; // Yeni DataGridView'e veriyi ba�la
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
            // 1. Tablodan bir kay�t se�ildi mi?
            if (dgvLoans.SelectedRows.Count == 0 || dgvLoans.SelectedRows[0].DataBoundItem is not LoanDetailDto selectedLoan)
            {
                MessageBox.Show("Please select a loan record from the list to return.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kay�t zaten iade edilmi� mi?
            if (!string.IsNullOrEmpty(selectedLoan.ReturnDate))
            {
                MessageBox.Show("This book has already been returned.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int loanId = selectedLoan.LoanId; // <-- ��te LoanID'yi buradan kolayca al�yoruz.

            var confirmation = MessageBox.Show($"Are you sure you want to mark the loan for '{selectedLoan.BookTitle}' as returned?", "Confirm Return", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmation == DialogResult.No) return;

            try
            {
                // PUT api/loans/{loanId}/return adresine istek at
                HttpResponseMessage response = await client.PutAsync($"api/loans/{loanId}/return", null);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Book returned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Listeyi yenilemek i�in "Refresh All Loans" butonunu tetikle
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
