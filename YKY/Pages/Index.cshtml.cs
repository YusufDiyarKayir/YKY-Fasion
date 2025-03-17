using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace YKY.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        // Veritabanı bağlantısı için connection string
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";
        public List<byte[]> Gorseller { get; set; } = new();
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT AnaSayfaGorsel FROM AnaSayfa";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Gorseller.Add((byte[])reader["AnaSayfaGorsel"]);
                        }
                    }
                }
            }
        }
        // POST methodu
        public IActionResult OnPost(string SikayetAD, string SikayetTelefon, string SikayetBilgi, string SikayetEposta)
        {
            // Veritabanı için SQL sorgusu
            string query = "INSERT INTO Sikayet (SikayetAD, SikayetTelefon, SikayetBilgi, SikayetEposta) VALUES (@SikayetAD, @SikayetTelefon, @SikayetBilgi, @SikayetEposta)";

            // Veritabanına bağlanıp veriyi ekleyelim
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SikayetAD", SikayetAD);
                    command.Parameters.AddWithValue("@SikayetTelefon", SikayetTelefon);
                    command.Parameters.AddWithValue("@SikayetBilgi", SikayetBilgi);
                    command.Parameters.AddWithValue("@SikayetEposta", string.IsNullOrEmpty(SikayetEposta) ? (object)DBNull.Value : SikayetEposta);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            // Başarıyla veriyi ekledikten sonra kullanıcıya mesaj gösterelim
            ViewData["Message"] = "Şikayetiniz başarıyla gönderildi!";
            return Page();
        }
    }
}
