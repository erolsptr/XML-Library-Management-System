using System.Xml.Serialization;

namespace Library.API.Models
{
    // Bu attribute, bu sınıfın bir XML elementine karşılık geldiğini belirtir.
    // İsim olarak "Book" kullanılacak.
    [XmlRoot("Book")]
    public class Book
    {
        // Bu attribute, bu özelliğin XML'de bir attribute ("ID") olarak serileştirileceğini belirtir.
        [XmlAttribute("ID")]
        public int Id { get; set; }

        // Bu attribute'lar, özelliklerin XML'de element olarak ("Title", "Author" vb.) serileştirileceğini belirtir.
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