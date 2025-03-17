using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;

namespace YKY.Pages
{
    public class UrunlerimizModel : PageModel
    {
        // Veritabaný baðlantýsý için connection string
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

        // Ürünler listesini tutacak model
        public List<Urun> UrunlerListesi { get; set; }

        public void OnGet()
        {
            UrunlerListesi = new List<Urun>();

            // URL'den Search parametrelerini al
            string searchCinsiyet = Request.Query["SearchCinsiyet"];
            string searchKategori = Request.Query["SearchKategori"];

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // SQL sorgusunun temel hali
                string query = "SELECT UrunId, UrunAdý, UrunCinsiyeti, UrunKategori, UrunFiyat, UrunMateryal, UrunMarka, UrunSezon, UrunBeden, UrunRenk, UrunAnaResim FROM Urunler";

                bool hasWhereClause = false;

                // Eðer SearchCinsiyet parametresi varsa, filtrele
                if (!string.IsNullOrEmpty(searchCinsiyet))
                {
                    query += " WHERE UrunCinsiyeti = @SearchCinsiyet";
                    hasWhereClause = true;
                }

                // Eðer SearchKategori parametresi varsa, ve Cinsiyet ile birlikte ise "AND" ile baðla
                if (!string.IsNullOrEmpty(searchKategori))
                {
                    if (hasWhereClause)
                    {
                        query += " AND UrunKategori = @SearchKategori";
                    }
                    else
                    {
                        query += " WHERE UrunKategori = @SearchKategori";
                    }
                }

                SqlCommand command = new SqlCommand(query, connection);

                // Search parametrelerini ekle
                if (!string.IsNullOrEmpty(searchCinsiyet))
                {
                    command.Parameters.AddWithValue("@SearchCinsiyet", searchCinsiyet);
                }

                if (!string.IsNullOrEmpty(searchKategori))
                {
                    command.Parameters.AddWithValue("@SearchKategori", searchKategori);
                }

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Urun urun = new Urun
                    {
                        UrunId = reader.GetInt32(0),
                        UrunAdý = reader.GetString(1),
                        UrunCinsiyeti = reader.GetString(2),
                        UrunKategori = reader.GetString(3),
                        UrunFiyat = reader.GetInt32(4),
                        UrunMateryal = reader.GetString(5),
                        UrunMarka = reader.GetString(6),
                        UrunSezon = reader.GetString(7),
                        UrunBeden = reader.GetString(8),
                        UrunRenk = reader.GetString(9),
                        UrunAnaResim = reader.IsDBNull(10) ? null : (byte[])reader[10]  // Görsel verisi
                    };
                    UrunlerListesi.Add(urun);
                }

                reader.Close();
            }
        }


    }

    // Ürün modelini oluþturun
    public class Urun
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
        public byte[] UrunAnaResim { get; set; }  // Görseli tutacak byte dizisi
    }
}
