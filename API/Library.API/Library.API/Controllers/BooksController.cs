using Library.API.Models; 
using System.Xml; 
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization; 
using System.Xml.XPath;
using System.Xml.Xsl; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



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
                XDocument doc = XDocument.Load(_xmlFilePath);

            
                return Content(doc.ToString(), "application/xml");
            }
            catch (XmlException ex)
            {
                return BadRequest($"XML parsing error: {ex.Message}");
            }
            catch (System.IO.FileNotFoundException)
            {
                return NotFound("The 'books.xml' file could not be found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetBookById(int id)
        {
            try
            {
                XDocument doc = XDocument.Load(_xmlFilePath);

                
                string xpath = $"//Book[@ID='{id}']";
                XElement bookElement = doc.XPathSelectElement(xpath);

                if (bookElement != null)
                {
                    return Content(bookElement.ToString(), "application/xml");
                }
                else
                {
                    return NotFound($"Book with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
        [HttpPost]
        public IActionResult CreateBook([FromBody] XElement bookElement)
        {
            if (bookElement == null)
            {
                return BadRequest("Request body does not contain a valid XML book element.");
            }

            try
            {
                
                Book receivedBook;
                var serializer = new XmlSerializer(typeof(Book));
                using (var reader = bookElement.CreateReader())
                {
                    receivedBook = (Book)serializer.Deserialize(reader);
                }

                XDocument doc = XDocument.Load(_xmlFilePath);
                int maxId = doc.Descendants("Book").Max(b => (int?)b.Attribute("ID")) ?? 0;
                int newId = maxId + 1;

                var validatableBookElement = new XElement("Book",
                    new XAttribute("ID", newId),
                    new XElement("Title", receivedBook.Title ?? ""),
                    new XElement("Author", receivedBook.Author ?? ""),
                    new XElement("ISBN", receivedBook.Isbn ?? ""),
                    new XElement("PublicationYear", receivedBook.PublicationYear.ToString()),
                    new XElement("Genre", receivedBook.Genre ?? "")
                );

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
                    return BadRequest($"XML data is invalid: \n{validationErrors}");
                }

                doc.Element("Library").Element("Books").Add(validatableBookElement);
                doc.Save(_xmlFilePath);

                receivedBook.Id = newId;
                return CreatedAtAction(nameof(GetBookById), new { id = newId }, receivedBook);

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

                string xpath = $"//Book[@ID='{id}']";
                XElement bookToUpdate = doc.XPathSelectElement(xpath);

                if (bookToUpdate == null)
                {
                    return NotFound($"Book with ID {id} not found.");
                }


                bookElement.Attribute("ID")?.Remove();
                bookElement.SetAttributeValue("ID", id);

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

                Book updatedBookData;
                var serializer = new XmlSerializer(typeof(Book));
                using (var reader = bookElement.CreateReader())
                {
                    updatedBookData = (Book)serializer.Deserialize(reader);
                }

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
        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            try
            {
                XDocument doc = XDocument.Load(_xmlFilePath);

                string xpath = $"//Book[@ID='{id}']";
                XElement bookToDelete = doc.XPathSelectElement(xpath);

                if (bookToDelete == null)
                {
                    return NotFound($"Book with ID {id} not found.");
                }

                bookToDelete.Remove();

                doc.Save(_xmlFilePath);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.ToString()}");
            }
        }
        [HttpGet("report")]
        public IActionResult GetBooksAsHtmlReport()
        {
            try
            {
                var xslt = new XslCompiledTransform();
                xslt.Load("BooksToHtml.xslt");

                using (var sw = new StringWriter())
                {
                    xslt.Transform("books.xml", null, sw);

                    string htmlResult = sw.ToString();
                    return Content(htmlResult, "text/html");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred during XSLT transformation: {ex.ToString()}");
            }
        }
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
