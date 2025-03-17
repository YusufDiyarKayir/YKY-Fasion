using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;

namespace YKY.Pages
{
    public class SifreSifirlaModel : PageModel
    {
        [BindProperty] public string Email { get; set; }
        public string Message { get; set; }

        public void OnPost()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Musteri WHERE MusteriMAIL = @Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", Email);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    // 6 haneli bir kod oluþtur
                    string resetCode = new Random().Next(100000, 999999).ToString();

                    // Geçici kodu veritabanýna kaydet
                    query = "UPDATE Musteri SET SifreSifirlamaKodu = @ResetCode WHERE MusteriMAIL = @Email";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ResetCode", resetCode);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.ExecuteNonQuery();

                    // Þifre sýfýrlama kodunu e-posta ile gönder
                    bool emailSent = SendEmail(Email, resetCode);

                    if (emailSent)
                    {
                        Message = $"Þifre sýfýrlama kodunuz e-posta adresinize gönderildi.";
                    }
                    else
                    {
                        Message = "E-posta gönderme sýrasýnda bir hata oluþtu. Lütfen tekrar deneyin.";
                    }
                }
                else
                {
                    Message = "Bu e-posta adresiyle kayýtlý bir hesap bulunamadý.";
                }
            }
        }

        // E-posta gönderme fonksiyonu
        public bool SendEmail(string recipientEmail, string resetCode)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("ykyfashion@gmail.com"),  // Gönderen e-posta adresi
                    Subject = "Þifre Sýfýrlama Kodu",
                    Body = $"Þifre sýfýrlama kodunuz: {resetCode}. Yeni þifrenizi belirlemek için <a href='https://localhost:7164/YeniSifre'>buraya</a> týklayýn.",
                    IsBodyHtml = true  // HTML formatýnda mail gönderileceðini belirtir
                };
                mailMessage.To.Add(recipientEmail);

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587, // Gmail SMTP portu
                    Credentials = new NetworkCredential("ykyfashion@gmail.com", "eiad yyyq wtid awmt"), // E-posta ve þifre
                    EnableSsl = true
                };

                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // Hata durumu için log veya mesaj gösterebilirsiniz
                Console.WriteLine($"E-posta gönderme hatasý: {ex.Message}");
                return false;
            }
        }

    }
}
