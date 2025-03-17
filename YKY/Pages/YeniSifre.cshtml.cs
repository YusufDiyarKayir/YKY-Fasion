using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace YKY.Pages
{
    public class YeniSifreModel : PageModel
    {
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string ResetCode { get; set; }
        [BindProperty] public string NewPassword { get; set; }
        [BindProperty] public string ConfirmPassword { get; set; }
        public string Message { get; set; }

        public void OnPost()
        {
            if (NewPassword != ConfirmPassword)
            {
                Message = "�ifreler uyu�muyor!";
                return;
            }

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Kod ve e-posta e�le�mesini kontrol et
                string query = "SELECT COUNT(*) FROM Musteri WHERE MusteriMAIL = @Email AND SifreSifirlamaKodu = @ResetCode";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@ResetCode", ResetCode);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    // Yeni �ifreyi g�ncelle
                    query = "UPDATE Musteri SET Muster�Sifre = @NewPassword, SifreSifirlamaKodu = NULL WHERE MusteriMAIL = @Email";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@NewPassword", NewPassword);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.ExecuteNonQuery();

                    Message = "�ifreniz ba�ar�yla g�ncellendi! <a href='/girisyap'>Giri� Yap</a>";
                }
                else
                {
                    Message = "Ge�ersiz kod veya e-posta!";
                }
            }
        }
    }
}
