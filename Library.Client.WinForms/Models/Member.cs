using System.Xml.Serialization;

namespace Library.Client.WinForms.Models
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
        public string MembershipDate { get; set; } // Basitlik için string tutalım

        [XmlElement("Email")]
        public string Email { get; set; }
    }
}