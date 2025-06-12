using Library.Client.WinForms.Models;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Forms;

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
            // HttpClient i�in temel ayarlar� yap
            client.BaseAddress = new Uri(ApiBaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            // API'miz hem JSON hem de XML ile konu�abildi�i i�in ikisini de ekleyelim.
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));

            // Ba�lang��ta i�lem butonlar� pasif olsun, giri� yap�nca aktifle�sin.
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

            // Not: btnFetchBookDetails butonu bu listeye hi� eklenmemeli ki her zaman aktif kals�n.
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var loginData = new { username, password };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(loginData), System.Text.Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync("api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var tokenObject = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(responseBody);
                    _jwtToken = tokenObject.GetProperty("token").GetString();

                    // HttpClient'�n varsay�lan ba�l�klar�na token'� ekle
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

                    // Aray�z� g�ncelle
                    lblAuthStatus.Text = $"Status: Logged in as {username}";
                    lblAuthStatus.ForeColor = Color.Green;
                    MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // ��lem butonlar�n� aktif hale getir
                    ToggleBookOperationControls(true);
                }
                else
                {
                    lblAuthStatus.Text = "Status: Login Failed";
                    lblAuthStatus.ForeColor = Color.Red;
                    MessageBox.Show($"Login failed. Status: {response.ReasonPhrase}", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
