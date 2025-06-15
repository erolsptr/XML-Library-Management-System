using Library.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Library.API.Controllers
{
    [Route("api/loans")] // Rota daha basit: /api/loans
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LoansController : ControllerBase
    {
        private readonly string _loansFilePath = "loans.xml";
        private readonly string _booksFilePath = "books.xml";

        // GET: api/loans
        // Tüm ödünç alma kayıtlarını, kitap ve üye isimleriyle birlikte listeler.
        [HttpGet]
        public ActionResult<IEnumerable<LoanDetailDto>> GetAllLoans()
        {
            try
            {
                // Gerekli tüm XML dosyalarını yükle
                var loansDoc = XDocument.Load(_loansFilePath);
                var booksDoc = XDocument.Load(_booksFilePath);
                var membersDoc = XDocument.Load("members.xml");

                // Her bir <Loan> elementi için zenginleştirilmiş bir DTO oluştur
                var loansWithDetails = loansDoc.Descendants("Loan").Select(l =>
                {
                    int bookId = (int)l.Element("BookID");
                    int memberId = (int)l.Element("MemberID");

                    // ID'leri kullanarak isimleri ve başlıkları bul
                    string bookTitle = booksDoc.Descendants("Book")
                                           .FirstOrDefault(b => b.Attribute("ID")?.Value == bookId.ToString())
                                           ?.Element("Title")?.Value ?? "Unknown Book"; // Bulamazsa "Bilinmeyen Kitap" yaz

                    var memberElement = membersDoc.Descendants("Member")
                                           .FirstOrDefault(m => m.Attribute("ID")?.Value == memberId.ToString());

                    string memberName = (memberElement != null)
                                           ? $"{memberElement.Element("FirstName")?.Value} {memberElement.Element("LastName")?.Value}"
                                           : "Unknown Member"; // Bulamazsa "Bilinmeyen Üye" yaz

                    // LoanDetailDto nesnesini oluştur ve doldur
                    return new LoanDetailDto
                    {
                        LoanId = (int)l.Attribute("LoanID"),
                        BookId = bookId,
                        BookTitle = bookTitle,       // <-- Zenginleştirilmiş veri
                        MemberId = memberId,
                        MemberName = memberName,     // <-- Zenginleştirilmiş veri
                        LoanDate = l.Element("LoanDate")?.Value,
                        ReturnDate = l.Element("ReturnDate")?.Value
                    };
                }).ToList();

                return Ok(loansWithDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // POST: api/loans
        // Yeni bir ödünç alma işlemi yaratır.
        // Body'de {"bookId": 1, "memberId": 101} gibi bir JSON bekler.
        [HttpPost]
        public IActionResult LoanBook([FromBody] LoanRequest request)
        {
            try
            {
                // TODO: Kitap ve üyenin var olup olmadığını kontrol et.
                var membersDoc = XDocument.Load("members.xml");
                bool memberExists = membersDoc.Descendants("Member")
                                             .Any(m => m.Attribute("ID")?.Value == request.MemberId.ToString());

                if (!memberExists)
                {
                    return NotFound(new { Message = $"Member with ID {request.MemberId} does not exist." });
                }

                var booksDoc = XDocument.Load("books.xml");
                bool bookExists = booksDoc.Descendants("Book")
                                          .Any(b => b.Attribute("ID")?.Value == request.BookId.ToString());

                if (!bookExists)
                {
                    return NotFound(new { Message = $"Book with ID {request.BookId} does not exist." });
                }
                // Kitabın daha önce ödünç alınıp geri verilmediğini kontrol et
                var loansDoc = XDocument.Load(_loansFilePath);
                bool isAlreadyLoaned = loansDoc.Descendants("Loan")
                    .Any(l => l.Element("BookID")?.Value == request.BookId.ToString() &&
                              string.IsNullOrEmpty(l.Element("ReturnDate")?.Value));

                if (isAlreadyLoaned)
                {
                    return Conflict(new { Message = $"Book with ID {request.BookId} is already on loan." });
                }

                // Yeni LoanID oluştur
                int maxLoanId = loansDoc.Descendants("Loan").Max(l => (int?)l.Attribute("LoanID")) ?? 1000;
                int newLoanId = maxLoanId + 1;

                var newLoanElement = new XElement("Loan",
                    new XAttribute("LoanID", newLoanId),
                    new XElement("BookID", request.BookId),
                    new XElement("MemberID", request.MemberId),
                    new XElement("LoanDate", DateTime.Now.ToString("yyyy-MM-dd")),
                    new XElement("ReturnDate", "")
                );

                loansDoc.Root.Add(newLoanElement);
                loansDoc.Save(_loansFilePath);

                return CreatedAtAction(nameof(GetAllLoans), new { Message = $"Book {request.BookId} loaned to member {request.MemberId}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        // PUT: api/loans/{loanId}/return
        // Belirli bir ödünç alma kaydını "iade edildi" olarak işaretler.
        [HttpPut("{loanId}/return")]
        public IActionResult ReturnBook(int loanId)
        {
            try
            {
                var loansDoc = XDocument.Load(_loansFilePath);

                // Güncellenecek olan <Loan> elementini LoanID'ye göre bul
                var loanElement = loansDoc.Descendants("Loan")
                                          .FirstOrDefault(l => l.Attribute("LoanID")?.Value == loanId.ToString());

                if (loanElement == null)
                {
                    return NotFound(new { Message = $"Loan record with ID {loanId} not found." });
                }

                // ReturnDate alanının zaten dolu olup olmadığını kontrol et
                var returnDateElement = loanElement.Element("ReturnDate");
                if (returnDateElement != null && !string.IsNullOrEmpty(returnDateElement.Value))
                {
                    return BadRequest(new { Message = "This book has already been returned." });
                }

                // ReturnDate'i o anki tarihle güncelle
                returnDateElement.Value = DateTime.Now.ToString("yyyy-MM-dd");

                // Değişiklikleri XML dosyasına kaydet
                loansDoc.Save(_loansFilePath);

                return Ok(new { Message = $"Loan {loanId} has been successfully marked as returned." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}

// POST isteği için Body'de gelecek veriyi temsil eden basit bir sınıf.
public class LoanRequest
{
    public int BookId { get; set; }
    public int MemberId { get; set; }
}