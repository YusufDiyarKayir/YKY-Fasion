using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YKY.Pages
{
    public class UrunDetayModel : PageModel
    {
        private readonly string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

        public UrunDetay UrunDetay { get; set; }
        public List<UrunGorselleri> UrunGorseller { get; set; }

        public void OnGet(int id)
        {
            // Veritabanýndan ürün bilgilerini alalým
            UrunDetay = GetUrunDetayById(id);

            // Veritabanýndan ürün görsellerini alalým
            UrunGorseller = GetUrunGorsellerById(id);

            // Görselleri base64 formatýna dönüþtürme
            foreach (var gorsel in UrunGorseller)
            {
                if (gorsel.GorselResim != null && gorsel.GorselResim.Length > 0)
                {
                    gorsel.Base64Resim = Convert.ToBase64String(gorsel.GorselResim);
                }
                else
                {
                    gorsel.Base64Resim = string.Empty;
                }
            }
        }

        private UrunDetay GetUrunDetayById(int urunId)
        {
            UrunDetay urunDetay = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Urunler WHERE UrunId = @UrunId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UrunId", urunId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            urunDetay = new UrunDetay
                            {
                                UrunId = (int)reader["UrunId"],
                                UrunAdý = reader["UrunAdý"].ToString(),
                                UrunCinsiyeti = reader["UrunCinsiyeti"].ToString(),
                                UrunKategori = reader["UrunKategori"].ToString(),
                                UrunFiyat = (int)reader["UrunFiyat"],
                                UrunMateryal = reader["UrunMateryal"].ToString(),
                                UrunMarka = reader["UrunMarka"].ToString(),
                                UrunSezon = reader["UrunSezon"].ToString(),
                                UrunBeden = reader["UrunBeden"].ToString(),
                                UrunRenk = reader["UrunRenk"].ToString(),
                                // Eðer varsa Resim alaný
                                UrunAnaResim = reader["UrunAnaResim"] as byte[]
                            };
                        }
                    }
                }
            }

            return urunDetay;
        }

        private List<UrunGorselleri> GetUrunGorsellerById(int urunId)
        {
            List<UrunGorselleri> gorseller = new List<UrunGorselleri>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM UrunGorselleri WHERE UrunId = @UrunId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UrunId", urunId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var gorsel = new UrunGorselleri
                            {
                                GorselId = (int)reader["GorselId"],
                                UrunId = (int)reader["UrunId"],
                                GorselResim = reader["GorselResim"] as byte[]
                            };
                            gorseller.Add(gorsel);
                        }
                    }
                }
            }

            return gorseller;
        }
    }

    public class UrunDetay
    {
        public int UrunId { get; set; }
        public string UrunAdý { get; set; }
        public string UrunCinsiyeti { get; set; }
        public string UrunKategori { get; set; }
        public int UrunFiyat { get; set; }
        public string UrunMateryal { get; set; }
        public string UrunMarka { get; set; }
        public string UrunSezon { get; set; }
        public string UrunBeden { get; set; }
        public string UrunRenk { get; set; }
        public byte[] UrunAnaResim { get; set; }
    }

    public class UrunGorselleri
    {
        public int GorselId { get; set; }
        public int UrunId { get; set; }
        public byte[] GorselResim { get; set; }
        public string Base64Resim { get; set; }  // Yeni eklenen özellik
    }
}
