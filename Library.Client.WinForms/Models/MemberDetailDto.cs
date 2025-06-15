using Library.Client.WinForms.Models;

namespace Library.API.Models
{
    public class MemberDetailDto : Member
    {
        public List<int> ActiveLoanBookIds { get; set; } = new List<int>();
    }
}