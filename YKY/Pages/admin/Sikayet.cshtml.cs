using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace YKY.Pages.admin
{
    public class SikayetModel : PageModel
    {
        public List<Sikayet> Sikayetler { get; set; } = new List<Sikayet>();
        public List<Personel> Personeller { get; set; } = new List<Personel>();

        public void OnGet()
        {
            string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                // Þikayetleri Getir
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Sikayet", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Sikayetler.Add(new Sikayet
                        {
                            SikayetId = (int)reader["SikayetId"],
                            SikayetAD = reader["SikayetAD"].ToString(),
                            SikayetTelefon = reader["SikayetTelefon"].ToString(),
                            SikayetBilgi = reader["SikayetBilgi"].ToString(),
                            SikayetEposta = reader["SikayetEposta"].ToString()
                        });
                    }
                }

                // Personelleri Getir
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Personel", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Personeller.Add(new Personel
                        {
                            Id = (int)reader["Id"],
                            PersonelAd = reader["PersonelAd"].ToString(),
                            PersonelSoyad = reader["PersonelSoyad"].ToString()
                        });
                    }
                }
            }
        }

        public IActionResult OnPostSil(int sikayetId)
        {
            string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Sikayet WHERE SikayetId = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", sikayetId);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage();
        }

        public IActionResult OnPostAtama(int sikayetId, int personelId)
        {
            string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // Þikayetle iliþkilendirilmiþ bir personel var mý diye kontrol edelim
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM SikayetSorumlusu WHERE SikayetId = @sikayetId", conn))
                {
                    cmd.Parameters.AddWithValue("@sikayetId", sikayetId);
                    int count = (int)cmd.ExecuteScalar();

                    // Eðer bir personel atanmýþsa, iþlem yapýlmasýn
                    if (count > 0)
                    {
                        ModelState.AddModelError("", "Bu þikayete zaten bir personel atanmýþtýr.");
                        return Page(); // Hata mesajýný döndürelim
                    }
                }

                // Eðer þikayete atanmýþ bir personel yoksa, yeni personeli ata
                using (SqlCommand cmd = new SqlCommand("INSERT INTO SikayetSorumlusu (SikayetId, PersonelId) VALUES (@sikayetId, @personelId)", conn))
                {
                    cmd.Parameters.AddWithValue("@sikayetId", sikayetId);
                    cmd.Parameters.AddWithValue("@personelId", personelId);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage();
        }

        public class Personel
        {
            public int Id { get; set; }
            public string PersonelAd { get; set; }
            public string PersonelSoyad { get; set; }
        }

        public class Sikayet
        {
            public int SikayetId { get; set; }
            public string SikayetAD { get; set; }
            public string SikayetTelefon { get; set; }
            public string SikayetBilgi { get; set; }
            public string SikayetEposta { get; set; }
        }
    }
}
