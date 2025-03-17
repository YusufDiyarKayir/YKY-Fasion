using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace YKY.Pages.admin
{
    public class UrunEkleModel : PageModel
    {
        private readonly string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

        [BindProperty]
        public string UrunAdi { get; set; }
        [BindProperty]
        public string UrunCinsiyeti { get; set; }
        [BindProperty]
        public string UrunKategori { get; set; }
        [BindProperty]
        public int UrunFiyat { get; set; }
        [BindProperty]
        public string UrunMateryal { get; set; }
        [BindProperty]
        public string UrunMarka { get; set; }
        [BindProperty]
        public string UrunSezon { get; set; }
        [BindProperty]
        public string UrunBeden { get; set; }
        [BindProperty]
        public string UrunRenk { get; set; }
        [BindProperty]
        public IFormFile UrunAnaResim { get; set; }
        [BindProperty]
        public List<IFormFile> UrunGorselleri { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            byte[] anaResimData = null;

            // Ana resim verisini byte array'e dönüþtürme
            if (UrunAnaResim != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await UrunAnaResim.CopyToAsync(memoryStream);
                    anaResimData = memoryStream.ToArray();
                }
            }

            // Ürün verilerini veritabanýna eklemek için ADO.NET kullanarak SQL sorgusu
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "INSERT INTO Urunler (UrunAdý, UrunCinsiyeti, UrunKategori, UrunFiyat, UrunMateryal, UrunMarka, UrunSezon, UrunBeden, UrunRenk, UrunAnaResim) " +
                            "VALUES (@UrunAdi, @UrunCinsiyeti, @UrunKategori, @UrunFiyat, @UrunMateryal, @UrunMarka, @UrunSezon, @UrunBeden, @UrunRenk, @UrunAnaResim); SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UrunAdi", UrunAdi);
                    command.Parameters.AddWithValue("@UrunCinsiyeti", UrunCinsiyeti);
                    command.Parameters.AddWithValue("@UrunKategori", UrunKategori);
                    command.Parameters.AddWithValue("@UrunFiyat", UrunFiyat);
                    command.Parameters.AddWithValue("@UrunMateryal", UrunMateryal);
                    command.Parameters.AddWithValue("@UrunMarka", UrunMarka);
                    command.Parameters.AddWithValue("@UrunSezon", UrunSezon);
                    command.Parameters.AddWithValue("@UrunBeden", UrunBeden);
                    command.Parameters.AddWithValue("@UrunRenk", UrunRenk);
                    command.Parameters.AddWithValue("@UrunAnaResim", anaResimData);

                    var urunId = Convert.ToInt32(await command.ExecuteScalarAsync());

                    // Ürün görsellerini eklemek için
                    if (UrunGorselleri != null && UrunGorselleri.Count > 0)
                    {
                        foreach (var gorsel in UrunGorselleri)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await gorsel.CopyToAsync(memoryStream);
                                var gorselResim = memoryStream.ToArray();

                                var gorselQuery = "INSERT INTO UrunGorselleri (UrunId, GorselResim) VALUES (@UrunId, @GorselResim)";
                                using (var gorselCommand = new SqlCommand(gorselQuery, connection))
                                {
                                    gorselCommand.Parameters.AddWithValue("@UrunId", urunId);
                                    gorselCommand.Parameters.AddWithValue("@GorselResim", gorselResim);
                                    await gorselCommand.ExecuteNonQueryAsync();
                                }
                            }
                        }
                    }
                }
            }

            return RedirectToPage("/admin/UrunEkle");
        }
    }
}
