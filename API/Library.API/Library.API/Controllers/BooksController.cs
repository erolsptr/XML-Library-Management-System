using Library.API.Models; // Book modelimizi kullanabilmek için bu satırı ekleyin.
using Microsoft.AspNetCore.Mvc;
using System.Xml; // XML ile çalışmak için bu kütüphaneyi ekliyoruz.
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization; // XSD validasyonu için bu satırı ekleyin.
using System.Xml.XPath;
using System.Xml.Xsl; // LINQ to XML için bu kütüphaneyi ekliyoruz.
using Microsoft.AspNetCore.Authorization;


namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly string _xmlFilePath = "books.xml";

        // GET: api/books
        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                // XDocument kullanarak XML dosyasını güvenli bir şekilde yüklüyoruz.
                XDocument doc = XDocument.Load(_xmlFilePath);

                // XML içeriğini olduğu gibi string formatında döndürüyoruz.
                // Content metodu, cevabın tipini ve içeriğini belirtmemizi sağlar.
                // "application/xml" -> Bu cevabın bir XML verisi olduğunu tarayıcıya/istemciye söyler.
                return Content(doc.ToString(), "application/xml");
            }
            catch (XmlException ex)
            {
                // XML dosyası bozuk veya okunamıyorsa hata döndür.
                return BadRequest($"XML parsing error: {ex.Message}");
            }
            catch (System.IO.FileNotFoundException)
            {
                // XML dosyası bulunamazsa hata döndür.
                return NotFound("The 'books.xml' file could not be found.");
            }
            catch (Exception ex)
            {
                // Beklenmedik diğer tüm hatalar için genel bir sunucu hatası döndür.
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
        // Önceki GetAllBooks() metodu burada duruyor...

        // GET: api/books/5
        [HttpGet("{id}")]
        public IActionResult GetBookById(int id)
        {
            try
            {
                XDocument doc = XDocument.Load(_xmlFilePath);

                // XPath kullanarak belirli bir ID'ye sahip kitabı arıyoruz.
                // "//Book[@ID='{id}']" ifadesi şu anlama gelir:
                // "//Book"      -> Dokümanın herhangi bir yerindeki <Book> elementlerini bul.
                // "[@ID='...']" -> Bu elementler arasından ID niteliği (attribute) bizim verdiğimiz id değerine eşit olanı seç.
                string xpath = $"//Book[@ID='{id}']";
                XElement bookElement = doc.XPathSelectElement(xpath);

                // Eğer aranan ID'ye sahip bir kitap bulunursa...
                if (bookElement != null)
                {
                    // Bulunan kitabın XML'ini string olarak döndür.
                    return Content(bookElement.ToString(), "application/xml");
                }
                else
                {
                    // Kitap bulunamazsa 404 Not Found hatası döndür.
                    return NotFound($"Book with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                // Genel bir hata durumunda 500 Internal Server Error döndür.
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
        // POST: api/books
        // POST: api/books
        [HttpPost]
        public IActionResult CreateBook([FromBody] XElement bookElement)
        {
            // Gelen XML'in null olup olmadığını kontrol et
            if (bookElement == null)
            {
                return BadRequest("Request body does not contain a valid XML book element.");
            }

            try
            {
                // --- 1. Adım: XML'i Book nesnesine dönüştürme (Deserialization) ---
                // Bu adımda XSD validasyonu yapmıyoruz, sadece veriyi alıyoruz.
                Book receivedBook;
                var serializer = new XmlSerializer(typeof(Book));
                using (var reader = bookElement.CreateReader())
                {
                    receivedBook = (Book)serializer.Deserialize(reader);
                }

                // --- 2. Adım: Yeni ID atama ve XSD'ye uygun yeni bir XElement oluşturma ---
                XDocument doc = XDocument.Load(_xmlFilePath);
                int maxId = doc.Descendants("Book").Max(b => (int?)b.Attribute("ID")) ?? 0;
                int newId = maxId + 1;

                // Bu, bizim XSD şemamıza %100 uyacak şekilde, elle oluşturduğumuz element.
                // ID'yi burada ekliyoruz.
                var validatableBookElement = new XElement("Book",
                    new XAttribute("ID", newId),
                    new XElement("Title", receivedBook.Title ?? ""),
                    new XElement("Author", receivedBook.Author ?? ""),
                    new XElement("ISBN", receivedBook.Isbn ?? ""),
                    new XElement("PublicationYear", receivedBook.PublicationYear.ToString()),
                    new XElement("Genre", receivedBook.Genre ?? "")
                );

                // --- 3. Adım: Elle oluşturduğumuz bu elementi XSD'ye göre doğrulama ---
                var schemas = new XmlSchemaSet();
                schemas.Add("", "books.xsd");

                var docToValidate = new XDocument(
                    new XElement("Library",
                        new XElement("Books",
                            validatableBookElement
                        )
                    )
                );

                string validationErrors = "";
                docToValidate.Validate(schemas, (sender, e) =>
                {
                    if (e.Severity == XmlSeverityType.Error)
                    {
                        validationErrors += e.Message + "\n";
                    }
                });

                if (!string.IsNullOrEmpty(validationErrors))
                {
                    // Eğer hala validasyon hatası varsa, gelen veride sorun vardır (örn: PublicationYear metin ise).
                    return BadRequest($"XML data is invalid: \n{validationErrors}");
                }

                // --- 4. Adım: Doğrulanmış elementi ana XML dosyasına ekleme ve kaydetme ---
                doc.Element("Library").Element("Books").Add(validatableBookElement);
                doc.Save(_xmlFilePath);

                // --- 5. Adım: Başarılı cevabı döndürme ---
                // Dönen nesnenin ID'sini de güncelleyelim.
                receivedBook.Id = newId;
                return CreatedAtAction(nameof(GetBookById), new { id = newId }, receivedBook);

            }
            catch (InvalidOperationException ex)
            {
                // Deserialization sırasında bir hata olursa (örn: PublicationYear'a metin girilirse)
                return BadRequest($"XML structure is invalid. Check data types. Details: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.ToString()}");
            }
        }
        // PUT: api/books/5
        // PUT: api/books/5
        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, [FromBody] XElement bookElement)
        {
            if (bookElement == null)
            {
                return BadRequest("Request body does not contain a valid XML book element.");
            }

            try
            {
                XDocument doc = XDocument.Load(_xmlFilePath);

                // XPath ile güncellenecek kitabı bul.
                string xpath = $"//Book[@ID='{id}']";
                XElement bookToUpdate = doc.XPathSelectElement(xpath);

                if (bookToUpdate == null)
                {
                    return NotFound($"Book with ID {id} not found.");
                }

                // --- Gelen XML verisini XSD'ye göre doğrulama (GÜNCELLENMİŞ MANTIK) ---

                // 1. Gelen XML'e, URL'den aldığımız ID'yi bir attribute olarak ekleyelim.
                // Bu, "ID eksik" hatasını önleyecek.
                // Önce mevcut ID niteliği var mı diye kontrol edip silelim, sonra kendimizinkini ekleyelim.
                bookElement.Attribute("ID")?.Remove();
                bookElement.SetAttributeValue("ID", id);

                // 2. Artık içinde ID olan bu elementi doğrulayalım.
                var schemas = new XmlSchemaSet();
                schemas.Add("", "books.xsd");

                var docToValidate = new XDocument(
                    new XElement("Library", new XElement("Books", bookElement))
                );

                string validationErrors = "";
                docToValidate.Validate(schemas, (s, e) => {
                    if (e.Severity == XmlSeverityType.Error) validationErrors += e.Message + "\n";
                });

                if (!string.IsNullOrEmpty(validationErrors))
                {
                    return BadRequest($"XML validation failed for the new data: \n{validationErrors}");
                }

                // --- XML'i Book nesnesine dönüştürme (Deserialization) ---
                Book updatedBookData;
                var serializer = new XmlSerializer(typeof(Book));
                using (var reader = bookElement.CreateReader())
                {
                    updatedBookData = (Book)serializer.Deserialize(reader);
                }

                // --- Mevcut kitap elementinin içeriğini yenisiyle değiştirme ---
                bookToUpdate.Element("Title").Value = updatedBookData.Title;
                bookToUpdate.Element("Author").Value = updatedBookData.Author;
                bookToUpdate.Element("ISBN").Value = updatedBookData.Isbn;
                bookToUpdate.Element("PublicationYear").Value = updatedBookData.PublicationYear.ToString();
                bookToUpdate.Element("Genre").Value = updatedBookData.Genre;

                doc.Save(_xmlFilePath);

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"XML structure is invalid. Check data types. Details: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.ToString()}");
            }
        }
        // DELETE: api/books/5
        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            try
            {
                XDocument doc = XDocument.Load(_xmlFilePath);

                // XPath ile silinecek kitabı bul.
                string xpath = $"//Book[@ID='{id}']";
                XElement bookToDelete = doc.XPathSelectElement(xpath);

                // Eğer o ID'ye sahip bir kitap yoksa, 404 Not Found hatası döndür.
                if (bookToDelete == null)
                {
                    return NotFound($"Book with ID {id} not found.");
                }

                // Bulunan elementi XML ağacından kaldır.
                bookToDelete.Remove();

                // Değişiklikleri dosyaya kaydet.
                doc.Save(_xmlFilePath);

                // Başarılı olduğunda 204 No Content döndür.
                // Silme işlemi sonrası geriye bir içerik döndürmek anlamsızdır.
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.ToString()}");
            }
        }
        // GET: api/books/report
        [HttpGet("report")]
        public IActionResult GetBooksAsHtmlReport()
        {
            try
            {
                // XSLT dönüşümünü hazırlama
                var xslt = new XslCompiledTransform();
                xslt.Load("BooksToHtml.xslt");

                // Dönüşümün sonucunu yazacağımız bir yer (hafızada bir metin yazıcısı)
                using (var sw = new StringWriter())
                {
                    // Dönüşümü gerçekleştir: XML dosyasını al, XSLT'yi uygula ve sonucu sw'ye yaz.
                    xslt.Transform("books.xml", null, sw);

                    // Sonucu string olarak al ve HTML olarak döndür.
                    string htmlResult = sw.ToString();
                    return Content(htmlResult, "text/html");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred during XSLT transformation: {ex.ToString()}");
            }
        }
        // GET: api/v1/books/search?term=Dune
        // GET: api/v1/books/search?term=Dune
        [HttpGet("search")]
        public ActionResult<IEnumerable<Book>> SearchBooks([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest("Search term cannot be empty.");
            }

            var foundBooks = new List<Book>();

            try
            {
                using (XmlReader reader = XmlReader.Create("books.xml"))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Book")
                        {
                            XElement bookElement = XNode.ReadFrom(reader) as XElement;
                            if (bookElement != null)
                            {
                                string title = bookElement.Element("Title")?.Value;

                                if (title != null && title.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    var book = new Book
                                    {
                                        Id = (int)bookElement.Attribute("ID"),
                                        Title = title,
                                        Author = bookElement.Element("Author")?.Value,
                                        Isbn = bookElement.Element("ISBN")?.Value,
                                        PublicationYear = int.TryParse(bookElement.Element("PublicationYear")?.Value, out int year) ? year : 0,
                                        Genre = bookElement.Element("Genre")?.Value
                                    };
                                    foundBooks.Add(book);
                                }
                            }
                        }
                    }
                }
                return Ok(foundBooks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred during search: {ex.Message}");
            }
        }
    }
}