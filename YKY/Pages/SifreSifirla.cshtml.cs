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
                    // 6 haneli bir kod olu�tur
                    string resetCode = new Random().Next(100000, 999999).ToString();

                    // Ge�ici kodu veritaban�na kaydet
                    query = "UPDATE Musteri SET SifreSifirlamaKodu = @ResetCode WHERE MusteriMAIL = @Email";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ResetCode", resetCode);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.ExecuteNonQuery();

                    // �ifre s�f�rlama kodunu e-posta ile g�nder
                    bool emailSent = SendEmail(Email, resetCode);

                    if (emailSent)
                    {
                        Message = $"�ifre s�f�rlama kodunuz e-posta adresinize g�nderildi.";
                    }
                    else
                    {
                        Message = "E-posta g�nderme s�ras�nda bir hata olu�tu. L�tfen tekrar deneyin.";
                    }
                }
                else
                {
                    Message = "Bu e-posta adresiyle kay�tl� bir hesap bulunamad�.";
                }
            }
        }

        // E-posta g�nderme fonksiyonu
        public bool SendEmail(string recipientEmail, string resetCode)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("ykyfashion@gmail.com"),  // G�nderen e-posta adresi
                    Subject = "�ifre S�f�rlama Kodu",
                    Body = $"�ifre s�f�rlama kodunuz: {resetCode}. Yeni �ifrenizi belirlemek i�in <a href='https://localhost:7164/YeniSifre'>buraya</a> t�klay�n.",
                    IsBodyHtml = true  // HTML format�nda mail g�nderilece�ini belirtir
                };
                mailMessage.To.Add(recipientEmail);

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587, // Gmail SMTP portu
                    Credentials = new NetworkCredential("ykyfashion@gmail.com", "eiad yyyq wtid awmt"), // E-posta ve �ifre
                    EnableSsl = true
                };

                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // Hata durumu i�in log veya mesaj g�sterebilirsiniz
                Console.WriteLine($"E-posta g�nderme hatas�: {ex.Message}");
                return false;
            }
        }

    }
}
