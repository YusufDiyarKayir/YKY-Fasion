using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace YKY.Pages.admin
{
    public class Blog_managementModel : PageModel
    {
        private readonly string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

        public List<Blog> Bloglar { get; set; }

        public void OnGet()
        {
            Bloglar = new List<Blog>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Id, BlogBaslik, Blog›cerik, BlogGorsel, BlogTarih, BlogKonu FROM Blog";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Bloglar.Add(new Blog
                            {
                                Id = reader.GetInt32(0),
                                BlogBaslik = reader.GetString(1),
                                Blog›cerik = reader.GetString(2),
                                BlogGorsel = reader.IsDBNull(3) ? null : (byte[])reader[3],
                                BlogTarih = reader.GetDateTime(4),
                                BlogKonu = reader.GetString(5)
                            });
                        }
                    }
                }
            }
        }

        public IActionResult OnPostEkle(string BlogBaslik, string Blog›cerik, DateTime BlogTarih, string BlogKonu, IFormFile BlogGorsel)
        {
            byte[] imageData = null;

            if (BlogGorsel != null && BlogGorsel.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    BlogGorsel.CopyTo(ms);
                    imageData = ms.ToArray();
                }
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Blog (BlogBaslik, Blog›cerik, BlogGorsel, BlogTarih, BlogKonu) VALUES (@BlogBaslik, @Blog›cerik, @BlogGorsel, @BlogTarih, @BlogKonu)";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@BlogBaslik", BlogBaslik);
                    cmd.Parameters.AddWithValue("@Blog›cerik", Blog›cerik);
                    cmd.Parameters.AddWithValue("@BlogGorsel", (object)imageData ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BlogTarih", BlogTarih);
                    cmd.Parameters.AddWithValue("@BlogKonu", BlogKonu);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage();
        }
        public IActionResult OnPostDelete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Blog WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToPage();
        }


        public class Blog
        {
            public int Id { get; set; }
            public string BlogBaslik { get; set; }
            public string Blog›cerik { get; set; }
            public byte[] BlogGorsel { get; set; }
            public DateTime BlogTarih { get; set; }
            public string BlogKonu { get; set; }
        }
    }
}
