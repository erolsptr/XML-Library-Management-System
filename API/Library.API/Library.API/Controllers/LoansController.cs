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
    [Route("api/loans")] 
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LoansController : ControllerBase
    {
        private readonly string _loansFilePath = "loans.xml";
        private readonly string _booksFilePath = "books.xml";

        [HttpGet]
        public ActionResult<IEnumerable<LoanDetailDto>> GetAllLoans()
        {
            try
            {
                var loansDoc = XDocument.Load(_loansFilePath);
                var booksDoc = XDocument.Load(_booksFilePath);
                var membersDoc = XDocument.Load("members.xml");

                var loansWithDetails = loansDoc.Descendants("Loan").Select(l =>
                {
                    int bookId = (int)l.Element("BookID");
                    int memberId = (int)l.Element("MemberID");

                    string bookTitle = booksDoc.Descendants("Book")
                                           .FirstOrDefault(b => b.Attribute("ID")?.Value == bookId.ToString())
                                           ?.Element("Title")?.Value ?? "Unknown Book"; 

                    var memberElement = membersDoc.Descendants("Member")
                                           .FirstOrDefault(m => m.Attribute("ID")?.Value == memberId.ToString());

                    string memberName = (memberElement != null)
                                           ? $"{memberElement.Element("FirstName")?.Value} {memberElement.Element("LastName")?.Value}"
                                           : "Unknown Member"; 

                    
                    return new LoanDetailDto
                    {
                        LoanId = (int)l.Attribute("LoanID"),
                        BookId = bookId,
                        BookTitle = bookTitle,       
                        MemberId = memberId,
                        MemberName = memberName,     
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

        [HttpPost]
        public IActionResult LoanBook([FromBody] LoanRequest request)
        {
            try
            {
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
                var loansDoc = XDocument.Load(_loansFilePath);
                bool isAlreadyLoaned = loansDoc.Descendants("Loan")
                    .Any(l => l.Element("BookID")?.Value == request.BookId.ToString() &&
                              string.IsNullOrEmpty(l.Element("ReturnDate")?.Value));

                if (isAlreadyLoaned)
                {
                    return Conflict(new { Message = $"Book with ID {request.BookId} is already on loan." });
                }

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
        [HttpPut("{loanId}/return")]
        public IActionResult ReturnBook(int loanId)
        {
            try
            {
                var loansDoc = XDocument.Load(_loansFilePath);

                var loanElement = loansDoc.Descendants("Loan")
                                          .FirstOrDefault(l => l.Attribute("LoanID")?.Value == loanId.ToString());

                if (loanElement == null)
                {
                    return NotFound(new { Message = $"Loan record with ID {loanId} not found." });
                }

                var returnDateElement = loanElement.Element("ReturnDate");
                if (returnDateElement != null && !string.IsNullOrEmpty(returnDateElement.Value))
                {
                    return BadRequest(new { Message = "This book has already been returned." });
                }

                returnDateElement.Value = DateTime.Now.ToString("yyyy-MM-dd");

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

public class LoanRequest
{
    public int BookId { get; set; }
    public int MemberId { get; set; }
}