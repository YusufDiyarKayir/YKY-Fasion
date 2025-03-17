// AnaSayfaDuzen.cshtml.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace YKY.Pages.admin
{
    public class AnaSayfaDuzenModel : PageModel
    {
        [BindProperty]
        [Required]
        public IFormFile AnaSayfaGorsel { get; set; }

        public List<AnaSayfaItem> AnaSayfaVerileri { get; set; } = new List<AnaSayfaItem>();

        private readonly string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

        public void OnGet()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, AnaSayfaGorsel FROM AnaSayfa";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AnaSayfaVerileri.Add(new AnaSayfaItem
                        {
                            Id = reader.GetInt32(0),
                            GorselBase64 = Convert.ToBase64String((byte[])reader[1])
                        });
                    }
                }
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (var ms = new MemoryStream())
            {
                AnaSayfaGorsel.CopyTo(ms);
                byte[] imageData = ms.ToArray();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO AnaSayfa (AnaSayfaGorsel) VALUES (@Gorsel)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Gorsel", imageData);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return RedirectToPage("/admin/AnaSayfaDuzen");
        }

        public IActionResult OnPostDelete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM AnaSayfa WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToPage("/admin/AnaSayfaDuzen");
        }

        public class AnaSayfaItem
        {
            public int Id { get; set; }
            public string GorselBase64 { get; set; }
        }
    }
}