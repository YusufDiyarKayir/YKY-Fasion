using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;

namespace YKY.Pages
{
    public class BlogModel : PageModel
    {
        public List<Blog> Blogs { get; set; } = new List<Blog>();

        public void OnGet()
        {
            string connectionString =
                 @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT b.Id, b.BlogBaslik, b.Blog›cerik, b.BlogGorsel, b.PersonelId, b.BlogTarih, b.BlogKonu, p.PersonelAd
                                  FROM Blog b
                                  INNER JOIN Personel p ON b.PersonelId = p.Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Blogs.Add(new Blog
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            BlogBaslik = reader["BlogBaslik"].ToString(),
                            Blog›cerik = reader["Blog›cerik"].ToString(),
                            BlogGorsel = (byte[])reader["BlogGorsel"],
                            PersonelId = reader.GetInt32(reader.GetOrdinal("PersonelId")),
                            BlogTarih = reader.GetDateTime(reader.GetOrdinal("BlogTarih")),
                            BlogKonu = reader["BlogKonu"].ToString(),
                            PersonelAd = reader["PersonelAd"].ToString()
                        });
                    }
                }
            }
        }

        public class Blog
        {
            public int Id { get; set; }
            public string BlogBaslik { get; set; }
            public string Blog›cerik { get; set; }
            public byte[] BlogGorsel { get; set; }
            public int PersonelId { get; set; }
            public DateTime BlogTarih { get; set; }
            public string BlogKonu { get; set; }
            public string PersonelAd { get; set; }
        }

        public string GetImageAsBase64(byte[] image)
        {
            return image != null ? Convert.ToBase64String(image) : string.Empty;
        }
    }
}