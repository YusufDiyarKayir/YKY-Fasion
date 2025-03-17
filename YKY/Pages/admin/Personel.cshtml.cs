using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace YKY.Pages.admin
{
    public class PersonelModel : PageModel
    {
        [BindProperty]
        [Required]
        public string PersonelAd { get; set; }

        [BindProperty]
        [Required]
        public string PersonelSoyad { get; set; }

        [BindProperty]
        [Required]
        public int PersonelStatu { get; set; }

        public List<StatuModel> StatuList { get; set; }

        public string Message { get; set; }

        private readonly string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

        public void OnGet()
        {
            StatuList = new List<StatuModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Id, Ad FROM Statu", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                StatuList.Add(new StatuModel
                                {
                                    Id = reader.GetInt32(0),
                                    Ad = reader.GetString(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Message = "Veritabaný baðlantý hatasý: " + ex.Message;
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Personel (PersonelAd, PersonelSoyad, PersonelStatu) VALUES (@Ad, @Soyad, @Statu)", conn))
                    {
                        cmd.Parameters.AddWithValue("@Ad", PersonelAd);
                        cmd.Parameters.AddWithValue("@Soyad", PersonelSoyad);
                        cmd.Parameters.AddWithValue("@Statu", PersonelStatu);

                        cmd.ExecuteNonQuery();
                    }
                }

                Message = "Personel baþarýyla eklendi!";
                return RedirectToPage("/admin/Personel");
            }
            catch (SqlException ex)
            {
                Message = "Hata oluþtu: " + ex.Message;
                return Page();
            }
        }
    }

    public class StatuModel
    {
        public int Id { get; set; }
        public string Ad { get; set; }
    }
}
