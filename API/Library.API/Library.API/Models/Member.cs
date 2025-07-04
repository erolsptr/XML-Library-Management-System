﻿using System.Xml.Serialization;

namespace Library.API.Models
{
    [XmlRoot("Member")]
    public class Member
    {
        [XmlAttribute("ID")]
        public int Id { get; set; }

        [XmlElement("FirstName")]
        public string FirstName { get; set; }

        [XmlElement("LastName")]
        public string LastName { get; set; }

        [XmlElement("MembershipDate")]
        public string? MembershipDate { get; set; }
        [XmlElement("Email")]
        public string Email { get; set; }
    }
}