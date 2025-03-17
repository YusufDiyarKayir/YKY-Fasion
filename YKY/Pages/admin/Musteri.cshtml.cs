using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace YKY.Pages.admin
{
    public class MusteriModel : PageModel
    {
        public List<Musteri> Musteriler { get; set; } = new List<Musteri>();

        public void OnGet()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id, MusteriAD, MusteriSOYAD, MusteriCinsiyet, MusteriYas, MusteriMAIL, MusteriTelefon FROM Musteri";
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Musteriler.Add(new Musteri
                        {
                            Id = reader.GetInt32(0),
                            MusteriAD = reader.GetString(1),
                            MusteriSOYAD = reader.GetString(2),
                            MusteriCinsiyet = reader.GetString(3),
                            MusteriYas = reader.GetString(4),
                            MusteriMAIL = reader.GetString(5),
                            MusteriTelefon = reader.GetString(6)
                        });
                    }
                }
            }
        }

        public IActionResult OnPostSil(int id)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Musteri WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToPage();
        }

        public IActionResult OnPostGuncelle(int id, string musteriAD, string musteriSOYAD, string musteriCinsiyet, string musteriYas, string musteriMAIL, string musteriTelefon)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Musteri SET MusteriAD = @MusteriAD, MusteriSOYAD = @MusteriSOYAD, MusteriCinsiyet = @MusteriCinsiyet, MusteriYas = @MusteriYas, MusteriMAIL = @MusteriMAIL, MusteriTelefon = @MusteriTelefon WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@MusteriAD", musteriAD);
                    command.Parameters.AddWithValue("@MusteriSOYAD", musteriSOYAD);
                    command.Parameters.AddWithValue("@MusteriCinsiyet", musteriCinsiyet);
                    command.Parameters.AddWithValue("@MusteriYas", musteriYas);
                    command.Parameters.AddWithValue("@MusteriMAIL", musteriMAIL);
                    command.Parameters.AddWithValue("@MusteriTelefon", musteriTelefon);
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToPage();
        }
    }

    public class Musteri
    {
        public int Id { get; set; }
        public string MusteriAD { get; set; }
        public string MusteriSOYAD { get; set; }
        public string MusteriCinsiyet { get; set; }
        public string MusteriYas { get; set; }
        public string MusteriMAIL { get; set; }
        public string MusteriTelefon { get; set; }
    }
}