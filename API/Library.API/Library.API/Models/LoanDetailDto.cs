namespace Library.API.Models
{
    public class LoanDetailDto
    {
        public int LoanId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } 
        public int MemberId { get; set; }
        public string MemberName { get; set; } 
        public string LoanDate { get; set; }
        public string ReturnDate { get; set; }
    }
}