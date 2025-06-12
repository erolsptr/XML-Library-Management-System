namespace Library.API.Models
{
    // Kullanıcının API'ye giriş yapmak için göndereceği bilgileri temsil eder.
    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}