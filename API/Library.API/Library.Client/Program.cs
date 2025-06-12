using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json; // JSON işlemek için
using System.Threading.Tasks;
using System;

public class Program
{
    // HttpClient nesnesini uygulama boyunca tek bir kere oluşturup yeniden kullanmak en iyi pratiktir.
    static readonly HttpClient client = new HttpClient();
    private static string? _jwtToken = null; // Giriş yaptıktan sonra alınacak JWT'yi saklamak için.

    // API'nin temel adresini buraya yaz.
    // Projeyi çalıştırdığında API'nin hangi portta çalıştığını kontrol et ve gerekirse güncelle.
    private const string ApiBaseUrl = "https://localhost:7090/";

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Library API Client");
        Console.WriteLine("==================");

        // HttpClient için temel ayarları yap
        client.BaseAddress = new Uri(ApiBaseUrl);
        client.DefaultRequestHeaders.Accept.Clear();
        // Varsayılan olarak JSON ile haberleşeceğiz.
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        await RunMainMenu();
    }

    private static async Task RunMainMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- Main Menu ---");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. View All Books (Requires Login)");
            Console.WriteLine("0. Exit");
            Console.Write("Enter your choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await Login();
                    break;
                case "2":
                    // await GetAllBooks(); // Henüz yazmadık
                    Console.WriteLine("Bu fonksiyon henüz yazılmadı.");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private static async Task Login()
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine();

        Console.Write("Enter password: ");
        string password = Console.ReadLine();

        // API'ye göndereceğimiz giriş bilgilerini içeren bir nesne oluştur.
        var loginData = new { username, password };

        // Bu nesneyi JSON formatına çevir.
        var content = new StringContent(JsonSerializer.Serialize(loginData), System.Text.Encoding.UTF8, "application/json");

        // API'nin /api/auth/login endpoint'ine POST isteği gönder.
        HttpResponseMessage response = await client.PostAsync("api/auth/login", content);

        if (response.IsSuccessStatusCode)
        {
            // Cevap başarılıysa (200 OK), gelen JSON içeriğini oku.
            string responseBody = await response.Content.ReadAsStringAsync();

            // JSON içeriğinden token'ı ayıkla.
            // Bu basit ayıklama için geçici bir isimsiz tip kullanıyoruz.
            var tokenObject = JsonSerializer.Deserialize<JsonElement>(responseBody);
            _jwtToken = tokenObject.GetProperty("token").GetString();

            Console.WriteLine("\nLogin successful!");
            // Aldığımız token'ı HttpClient'ın varsayılan başlıklarına ekle.
            // Bu sayede bundan sonraki tüm istekler bu token'ı içerecek.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        }
        else
        {
            // Giriş başarısızsa hata mesajını göster.
            Console.WriteLine($"\nLogin failed. Status code: {response.StatusCode}");
            string error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {error}");
        }
    }
}