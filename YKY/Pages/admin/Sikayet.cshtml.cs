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
                // �ikayetleri Getir
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

                // �ikayetle ili�kilendirilmi� bir personel var m� diye kontrol edelim
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM SikayetSorumlusu WHERE SikayetId = @sikayetId", conn))
                {
                    cmd.Parameters.AddWithValue("@sikayetId", sikayetId);
                    int count = (int)cmd.ExecuteScalar();

                    // E�er bir personel atanm��sa, i�lem yap�lmas�n
                    if (count > 0)
                    {
                        ModelState.AddModelError("", "Bu �ikayete zaten bir personel atanm��t�r.");
                        return Page(); // Hata mesaj�n� d�nd�relim
                    }
                }

                // E�er �ikayete atanm�� bir personel yoksa, yeni personeli ata
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
