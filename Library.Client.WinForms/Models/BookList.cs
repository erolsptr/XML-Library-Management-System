using System.Collections.Generic;
using System.Xml.Serialization;

namespace Library.Client.WinForms.Models
{
    [XmlRoot("ArrayOfBook")] // API'nin varsayılan XML listesi kök adı
    public class BookList
    {
        [XmlElement("Book")]
        public List<Book> Books { get; set; }
    }
}