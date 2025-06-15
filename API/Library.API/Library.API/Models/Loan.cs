using System.Xml.Serialization;

namespace Library.API.Models
{
    [XmlRoot("Loan")]
    public class Loan
    {
        [XmlAttribute("LoanID")]
        public int LoanId { get; set; }

        [XmlElement("BookID")]
        public int BookId { get; set; }

        [XmlElement("MemberID")]
        public int MemberId { get; set; }

        [XmlElement("LoanDate")]
        public string LoanDate { get; set; }

        [XmlElement("ReturnDate")]
        public string ReturnDate { get; set; }
    }
}