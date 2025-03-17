using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace YKY.Pages.admin
{
    public class StatusModel : PageModel
    {
        private readonly string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

        [BindProperty]
        public string Ad { get; set; }

        public List<Status> StatusList { get; set; }

        public void OnGet()
        {
            StatusList = GetStatusList();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Ad))
            {
                ModelState.AddModelError(string.Empty, "Statü adý boþ olamaz.");
                return Page();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO [dbo].[Statu] (Ad) VALUES (@Ad)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Ad", Ad);
                        command.ExecuteNonQuery();
                    }
                }

                // Statü ekledikten sonra listeyi tekrar alýyoruz
                StatusList = GetStatusList();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Bir hata oluþtu: " + ex.Message);
                return Page();
            }
        }

        public IActionResult OnPostDelete(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM [dbo].[Statu] WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }

                // Silme iþlemi sonrasýnda listeyi tekrar alýyoruz
                StatusList = GetStatusList();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Bir hata oluþtu: " + ex.Message);
                return Page();
            }
        }

        private List<Status> GetStatusList()
        {
            List<Status> statusList = new List<Status>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM [dbo].[Statu]";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        statusList.Add(new Status
                        {
                            Id = (int)reader["Id"],
                            Ad = reader["Ad"].ToString()
                        });
                    }
                }
            }

            return statusList;
        }
    }

    public class Status
    {
        public int Id { get; set; }
        public string Ad { get; set; }
    }
}
