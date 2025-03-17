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
                Message = "Þifreler uyuþmuyor!";
                return;
            }

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Kod ve e-posta eþleþmesini kontrol et
                string query = "SELECT COUNT(*) FROM Musteri WHERE MusteriMAIL = @Email AND SifreSifirlamaKodu = @ResetCode";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@ResetCode", ResetCode);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    // Yeni þifreyi güncelle
                    query = "UPDATE Musteri SET MusterýSifre = @NewPassword, SifreSifirlamaKodu = NULL WHERE MusteriMAIL = @Email";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@NewPassword", NewPassword);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.ExecuteNonQuery();

                    Message = "Þifreniz baþarýyla güncellendi! <a href='/girisyap'>Giriþ Yap</a>";
                }
                else
                {
                    Message = "Geçersiz kod veya e-posta!";
                }
            }
        }
    }
}
