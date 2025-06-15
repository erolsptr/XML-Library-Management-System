using Library.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Bu da korumalı olsun
    public class MembersController : ControllerBase
    {
        private readonly string _membersFilePath = "members.xml";

        // GET: api/members
        [HttpGet]
        public ActionResult<IEnumerable<MemberDetailDto>> GetAllMembers()
        {
            try
            {
                var membersDoc = XDocument.Load("members.xml");
                var loansDoc = XDocument.Load("loans.xml");

                var membersWithLoans = membersDoc.Descendants("Member").Select(m =>
                {
                    var memberId = (int)m.Attribute("ID");

                    // Bu üyenin aktif olan (geri verilmeyen) tüm ödünç kayıtlarını bul
                    var activeLoans = loansDoc.Descendants("Loan")
                        .Where(l => l.Element("MemberID")?.Value == memberId.ToString() &&
                                    string.IsNullOrEmpty(l.Element("ReturnDate")?.Value))
                        .Select(l => (int)l.Element("BookID"))
                        .ToList();

                    return new MemberDetailDto
                    {
                        Id = memberId,
                        FirstName = m.Element("FirstName")?.Value,
                        LastName = m.Element("LastName")?.Value,
                        MembershipDate = m.Element("MembershipDate")?.Value,
                        Email = m.Element("Email")?.Value,
                        ActiveLoanBookIds = activeLoans
                    };
                }).ToList();

                return Ok(membersWithLoans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // GET: api/v1/members/101
        [HttpGet("{id}")]
        public ActionResult<Member> GetMemberById(int id)
        {
            try
            {
                var doc = XDocument.Load(_membersFilePath);
                var memberElement = doc.XPathSelectElement($"//Member[@ID='{id}']");

                if (memberElement == null)
                {
                    return NotFound();
                }

                var member = new Member
                {
                    Id = (int)memberElement.Attribute("ID"),
                    FirstName = memberElement.Element("FirstName")?.Value,
                    LastName = memberElement.Element("LastName")?.Value,
                    MembershipDate = memberElement.Element("MembershipDate")?.Value,
                    Email = memberElement.Element("Email")?.Value
                };

                return Ok(member);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        // POST: api/members
        [HttpPost]
        // POST: api/members
        [HttpPost]
        public IActionResult AddMember([FromBody] Member newMemberData)
        {
            try
            {
                var doc = XDocument.Load("members.xml");

                int maxId = doc.Descendants("Member").Max(m => (int?)m.Attribute("ID")) ?? 100;
                newMemberData.Id = maxId + 1;

                // Üyelik tarihini sunucu tarafında o anki tarih olarak ayarla
                string membershipDate = DateTime.Now.ToString("yyyy-MM-dd");
                newMemberData.MembershipDate = membershipDate; // Dönen nesnenin de doğru olması için

                var newElement = new XElement("Member",
                    new XAttribute("ID", newMemberData.Id),
                    new XElement("FirstName", newMemberData.FirstName),
                    new XElement("LastName", newMemberData.LastName),
                    new XElement("MembershipDate", membershipDate), // <-- DÜZELTİLDİ
                    new XElement("Email", newMemberData.Email)
                );

                doc.Root.Add(newElement);
                doc.Save("members.xml");

                // İstemciye oluşturulan tam üye bilgisini geri döndür.
                return CreatedAtAction(nameof(GetMemberById), new { id = newMemberData.Id }, newMemberData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // DELETE: api/members/101
        [HttpDelete("{id}")]
        public IActionResult DeleteMember(int id)
        {
            try
            {
                var doc = XDocument.Load("members.xml");
                var elementToDelete = doc.Descendants("Member").FirstOrDefault(m => m.Attribute("ID")?.Value == id.ToString());

                if (elementToDelete == null)
                {
                    return NotFound();
                }

                elementToDelete.Remove();
                doc.Save("members.xml");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}