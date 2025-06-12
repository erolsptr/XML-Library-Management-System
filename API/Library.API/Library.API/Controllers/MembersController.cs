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
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Bu da korumalı olsun
    public class MembersController : ControllerBase
    {
        private readonly string _membersFilePath = "members.xml";

        // GET: api/v1/members
        [HttpGet]
        public ActionResult<IEnumerable<Member>> GetAllMembers()
        {
            try
            {
                var doc = XDocument.Load(_membersFilePath);
                var members = doc.Descendants("Member").Select(m => new Member
                {
                    Id = (int)m.Attribute("ID"),
                    FirstName = m.Element("FirstName")?.Value,
                    LastName = m.Element("LastName")?.Value,
                    MembershipDate = m.Element("MembershipDate")?.Value,
                    Email = m.Element("Email")?.Value
                }).ToList();

                return Ok(members); // Not: Bu sefer JSON olarak dönecek.
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
    }
}