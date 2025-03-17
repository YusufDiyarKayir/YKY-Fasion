using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace YKY.Pages
{
    public class girisyapModel : PageModel
    {
        // Form Verileri
        [BindProperty] public string Ad { get; set; }
        [BindProperty] public string Soyad { get; set; }
        [BindProperty] public string Cinsiyet { get; set; }
        [BindProperty] public int Yas { get; set; }
        [BindProperty] public int KonumID { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Telefon { get; set; }
        [BindProperty] public string Password { get; set; }
        [BindProperty] public string ConfirmPassword { get; set; }

        // Hata ve Baþarý Mesajlarý
        public string ErrorMessage { get; set; }
        public string SignupErrorMessage { get; set; }
        public List<KonumModel> Konumlar { get; set; } = new List<KonumModel>();

        public void OnGet()
        {
            KonumlariYukle();
        }

        public void OnPost()
        {
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password) && string.IsNullOrEmpty(ConfirmPassword))
            {
                GiriþYap();
            }
            else if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword))
            {
                KayitOl();
            }
        }

        private void GiriþYap()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, MusteriAD FROM Musteri WHERE MusteriMAIL = @Email AND MusterýSifre = @Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@Password", Password);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read()) // Kullanýcý bulundu
                {
                    HttpContext.Session.SetString("MusteriMAIL", Email);
                    HttpContext.Session.SetString("MusteriAD", reader["MusteriAD"].ToString());
                    HttpContext.Session.SetInt32("MusteriID", reader.GetInt32(0));

                    Response.Redirect("/Index");
                }
                else
                {
                    ErrorMessage = "Hatalý giriþ! Lütfen tekrar deneyin.";
                }
            }
        }


        private void KayitOl()
        {
            if (Password != ConfirmPassword)
            {
                SignupErrorMessage = "Þifreler uyuþmuyor!";
                return;
            }

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Musteri (MusteriAD, MusteriSOYAD, MusteriCinsiyet, MusteriYas, MusteriKonumID, MusteriMAIL, MusteriTelefon, MusterýSifre) " +
                               "VALUES (@Ad, @Soyad, @Cinsiyet, @Yas, @KonumID, @Email, @Telefon, @Password)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Ad", Ad);
                cmd.Parameters.AddWithValue("@Soyad", Soyad);
                cmd.Parameters.AddWithValue("@Cinsiyet", Cinsiyet);
                cmd.Parameters.AddWithValue("@Yas", Yas);
                cmd.Parameters.AddWithValue("@KonumID", KonumID);
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@Telefon", Telefon);
                cmd.Parameters.AddWithValue("@Password", Password);

                conn.Open();
                cmd.ExecuteNonQuery();
                Response.Redirect("/girisyap");
            }
        }

        private void KonumlariYukle()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Konumlar";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Konumlar.Add(new KonumModel { KonumID = reader.GetInt32(0), KonumAdi = reader.GetString(1) });
                }
            }
        }
    }

    public class KonumModel
    {
        public int KonumID { get; set; }
        public string KonumAdi { get; set; }
    }
}
