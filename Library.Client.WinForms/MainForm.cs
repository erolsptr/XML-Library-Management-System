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
        private static readonly HttpClient client = new HttpClient();
        private static string? _jwtToken = null; 
        private const string ApiBaseUrl = "https://localhost:7090/";

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            client.BaseAddress = new Uri(ApiBaseUrl);
            ToggleBookOperationControls(false);


        }

        private void ToggleBookOperationControls(bool isEnabled)
        {

            btnGetAllBooks.Enabled = isEnabled;
            btnGetBookById.Enabled = isEnabled;
            txtBookId.Enabled = isEnabled;
            btnDeleteBook.Enabled = isEnabled;
            btnHtmlReport.Enabled = isEnabled;

            btnAddBook.Enabled = isEnabled;
            btnUpdateBook.Enabled = isEnabled;
            txtTitle.Enabled = isEnabled;
            txtAuthor.Enabled = isEnabled;
            txtYear.Enabled = isEnabled;
            txtGenre.Enabled = isEnabled;
            txtMemberFirstName.Enabled = isEnabled;
            txtMemberLastName.Enabled = isEnabled;
            txtMemberEmail.Enabled = isEnabled;
            btnAddMember.Enabled = isEnabled;
            btnDeleteMember.Enabled = isEnabled;
            btnGetAllLoans.Enabled = isEnabled;
            btnReturnBook.Enabled = isEnabled;

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
            if (string.IsNullOrEmpty(_jwtToken))
            {
                MessageBox.Show("You must be logged in to perform this action.", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                HttpResponseMessage response = await client.GetAsync("api/books");

                if (response.IsSuccessStatusCode)
                {
                    string xmlContent = await response.Content.ReadAsStringAsync();

                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<Book>), new System.Xml.Serialization.XmlRootAttribute("Books"));

                    List<Book> books;
                    using (var reader = new System.IO.StringReader(xmlContent))
                    {
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

                    dgvBooks.DataSource = books;
                    MessageBox.Show($"{books.Count} books loaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
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

            var confirmation = MessageBox.Show($"Are you sure you want to delete the book with ID {bookId}?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmation == DialogResult.No)
            {
                return;
            }

            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"api/books/{bookId}");

                if (response.IsSuccessStatusCode) 
                {
                    MessageBox.Show($"Book with ID {bookId} has been deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Title and Author are required fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
                HttpResponseMessage response = await client.PostAsync("api/books", content);

                if (response.IsSuccessStatusCode) 
                {
                    MessageBox.Show("Book added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClearInputFields();
                    btnGetAllBooks.PerformClick();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to add book. Status: {response.ReasonPhrase}\nDetails: {errorContent}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
            if (dgvBooks.SelectedRows.Count > 0 && dgvBooks.SelectedRows[0].DataBoundItem is Book selectedBook)
            {
                txtTitle.Text = selectedBook.Title;
                txtAuthor.Text = selectedBook.Author;
                txtIsbn.Text = selectedBook.Isbn;
                txtYear.Text = selectedBook.PublicationYear.ToString();
                txtGenre.Text = selectedBook.Genre;

                txtBookId.Text = selectedBook.Id.ToString();
            }
        }

        private async void btnUpdateBook_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtBookId.Text, out int bookId))
            {
                MessageBox.Show("Please select a book from the list to update, or enter a valid ID in the 'Get Book by ID' box.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Title and Author are required fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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

                if (response.IsSuccessStatusCode) 
                {
                    MessageBox.Show("Book updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

                    string filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "BookReport.html");
                    await System.IO.File.WriteAllTextAsync(filePath, htmlContent);

                    var processInfo = new System.Diagnostics.ProcessStartInfo(filePath)
                    {
                        UseShellExecute = true 
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
            string isbn = txtIsbn.Text.Trim(); 
            if (string.IsNullOrEmpty(isbn))
            {
                MessageBox.Show("Please enter an ISBN to fetch details.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string apiUrl = $"https://openlibrary.org/api/books?bibkeys=ISBN:{isbn}&format=json&jscmd=data";

            using (var externalApiClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await externalApiClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        if (jsonContent.Trim() == "{}")
                        {
                            MessageBox.Show($"No book found for ISBN: {isbn}", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        using (var jsonDoc = System.Text.Json.JsonDocument.Parse(jsonContent))
                        {
                            var bookData = jsonDoc.RootElement.GetProperty($"ISBN:{isbn}");

                            txtTitle.Text = bookData.TryGetProperty("title", out var title) ? title.GetString() : "";

                            if (bookData.TryGetProperty("authors", out var authors) && authors.GetArrayLength() > 0)
                            {
                                txtAuthor.Text = authors[0].TryGetProperty("name", out var authorName) ? authorName.GetString() : "";
                            }
                            else
                            {
                                txtAuthor.Text = ""; 
                            }

                            txtYear.Text = bookData.TryGetProperty("publish_date", out var publishDate) ? publishDate.GetString() : "";

                            DialogResult dialogResult = MessageBox.Show("Book details fetched successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        
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
            if (string.IsNullOrEmpty(_jwtToken))
            {
                MessageBox.Show("You must be logged in to view members.", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                HttpResponseMessage response = await client.GetAsync("api/members");

                if (response.IsSuccessStatusCode)
                {
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

            string requestUrl = $"{ApiBaseUrl}api/books/search?term={searchTerm}";

            try
            {
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

            var loanRequest = new { bookId = selectedBook.Id, memberId = memberId };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(loanRequest), System.Text.Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync("api/loans", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Book loaned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
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
            if (string.IsNullOrWhiteSpace(txtMemberFirstName.Text) || string.IsNullOrWhiteSpace(txtMemberLastName.Text))
            {
                MessageBox.Show("First Name and Last Name are required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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

                if (response.IsSuccessStatusCode) 
                {
                    MessageBox.Show("Member added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtMemberFirstName.Clear();
                    txtMemberLastName.Clear();
                    txtMemberEmail.Clear();
                    btnGetAllMembers.PerformClick(); 
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
            if (dgvMembers.SelectedRows.Count == 0 || dgvMembers.SelectedRows[0].DataBoundItem is not MemberDetailDto selectedMember)
            {
                MessageBox.Show("Please select a member from the list to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmation = MessageBox.Show($"Are you sure you want to delete {selectedMember.FirstName} {selectedMember.LastName}?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmation == DialogResult.No)
            {
                return;
            }

            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"api/members/{selectedMember.Id}");

                if (response.IsSuccessStatusCode) 
                {
                    MessageBox.Show("Member deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnGetAllMembers.PerformClick(); 
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
            lstMemberLoans.Items.Clear();

            if (dgvMembers.SelectedRows.Count > 0 && dgvMembers.SelectedRows[0].DataBoundItem is MemberDetailDto selectedMember)
            {
                if (selectedMember.ActiveLoanBookIds == null || !selectedMember.ActiveLoanBookIds.Any())
                {
                    lstMemberLoans.Items.Add("This member has no active loans.");
                    return;
                }


                try
                {
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
                HttpResponseMessage response = await client.GetAsync("api/loans");
                if (response.IsSuccessStatusCode)
                {
                    var loans = await response.Content.ReadFromJsonAsync<List<LoanDetailDto>>();
                    dgvLoans.DataSource = loans; 
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
            if (dgvLoans.SelectedRows.Count == 0 || dgvLoans.SelectedRows[0].DataBoundItem is not LoanDetailDto selectedLoan)
            {
                MessageBox.Show("Please select a loan record from the list to return.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(selectedLoan.ReturnDate))
            {
                MessageBox.Show("This book has already been returned.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int loanId = selectedLoan.LoanId; 

            var confirmation = MessageBox.Show($"Are you sure you want to mark the loan for '{selectedLoan.BookTitle}' as returned?", "Confirm Return", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmation == DialogResult.No) return;

            try
            {
                HttpResponseMessage response = await client.PutAsync($"api/loans/{loanId}/return", null);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Book returned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
