namespace Library.API.Models
{
    // Bir kullanıcının temel bilgilerini temsil eder (şifre hariç).
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}