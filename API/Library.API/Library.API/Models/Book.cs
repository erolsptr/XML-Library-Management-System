using System.Xml.Serialization;

namespace Library.API.Models
{
    [XmlRoot("Book")]
    public class Book
    {
        [XmlAttribute("ID")]
        public int Id { get; set; }

        [XmlElement("Title")]
        public string Title { get; set; }

        [XmlElement("Author")]
        public string Author { get; set; }

        [XmlElement("ISBN")]
        public string Isbn { get; set; }

        [XmlElement("PublicationYear")]
        public int PublicationYear { get; set; }

        [XmlElement("Genre")]
        public string Genre { get; set; }
    }
}