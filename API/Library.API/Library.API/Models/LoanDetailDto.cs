namespace Library.API.Models
{
    // Bu sınıf, bir ödünç alma kaydının tüm detaylarını
    // istemciye göndermek için kullanılır (Data Transfer Object).
    public class LoanDetailDto
    {
        public int LoanId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } // Kitabın adı
        public int MemberId { get; set; }
        public string MemberName { get; set; } // Üyenin adı
        public string LoanDate { get; set; }
        public string ReturnDate { get; set; }
    }
}