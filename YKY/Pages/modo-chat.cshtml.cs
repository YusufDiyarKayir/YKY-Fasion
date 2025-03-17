using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace YKY.Pages
{
    public class modo_chatModel : PageModel
    {
        private readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\yusuf\OneDrive\Belgeler\yky-fashion.mdf;Integrated Security=True;Connect Timeout=30";

        [BindProperty]
        public string UserInput { get; set; }
        public List<ChatMessage> ChatHistory { get; set; } = new List<ChatMessage>();
        private static int questionIndex = 0;  // Bu değişkeni ekledik

        private static readonly List<string> questions = new List<string>
        {
            "Hangi cinsiyete yönelik bir ürün arıyorsunuz? (Erkek/Kadın/Unisex)",
            "Hangi türde bir ürün arıyorsunuz? (Mont, Tişört, Gömlek, vb.)",
            "Hangi renkleri tercih edersiniz? (Örneğin, Siyah, Beyaz, Mavi, vb.)",
            "Beden tercihiniz nedir? (XS, S, M, L, XL, XXL)",
            "Hangi sezon için arıyorsunuz? (Yaz, Kış)",
            "Fiyat aralığınız nedir?",
            "Marka tercihiniz var mı?",
            "Ürününüzde hangi materyali tercih edersiniz?"
        };

        public void OnGet()
        {
            if (HttpContext.Session.TryGetValue("ChatHistory", out byte[] chatData))
            {
                ChatHistory = System.Text.Json.JsonSerializer.Deserialize<List<ChatMessage>>(chatData) ?? new List<ChatMessage>();
            }

            // Başlangıçta ilk soruyu sor
            if (questionIndex == 0)
            {
                AskNextQuestion();
            }
        }

        public IActionResult OnPost()
        {
            if (!string.IsNullOrWhiteSpace(UserInput))
            {
                ChatHistory.Add(new ChatMessage { Text = UserInput, IsUser = true });

                // Cevaba göre bir sonraki soruyu sormak için
                questionIndex++;
                if (questionIndex < questions.Count)
                {
                    AskNextQuestion();
                }
                else
                {
                    ChatHistory.Add(new ChatMessage { Text = "Sorular tamamlandı! Şimdi size uygun ürünler arıyorum...", IsUser = false });
                    // Ürünleri önerme kısmı burada yapılabilir
                    string productSuggestions = GetBotResponse(UserInput); // Bu, mevcut cevaba göre ürünü önerir
                    ChatHistory.Add(new ChatMessage { Text = productSuggestions, IsUser = false });
                }

                HttpContext.Session.Set("ChatHistory", System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(ChatHistory));
            }

            return Page();
        }

        private void AskNextQuestion()
        {
            var nextQuestion = questions[questionIndex];
            ChatHistory.Add(new ChatMessage { Text = nextQuestion, IsUser = false });
        }

        private string GetBotResponse(string input)
        {
            string lowerInput = input.ToLower();
            List<UrunBOT> uygunUrunler = new List<UrunBOT>();
            List<string> filters = new List<string>();

            // Cevaba göre filtreler oluşturuluyor
            if (lowerInput.Contains("erkek"))
                filters.Add("UrunCinsiyeti = 'Erkek'");
            else if (lowerInput.Contains("kadın"))
                filters.Add("UrunCinsiyeti = 'Kadın'");
            else if (lowerInput.Contains("unisex"))
                filters.Add("UrunCinsiyeti = 'Unisex'");

            // Fiyat, renk, beden vb. özelliklere göre de filtreler eklenebilir
            string query = "SELECT TOP 5 * FROM Urunler WHERE 1=1";

            // Filtreleri ekle
            if (filters.Any())
            {
                query += " AND " + string.Join(" AND ", filters);
            }

            query += " ORDER BY NEWID()"; // Rastgele ürünler seçiliyor

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    uygunUrunler.Add(new UrunBOT
                    {
                        UrunAdı = reader["UrunAdı"].ToString(),
                        UrunMarka = reader["UrunMarka"].ToString(),
                        UrunFiyat = (int)reader["UrunFiyat"],
                        UrunRenk = reader["UrunRenk"].ToString(),
                        UrunBeden = reader["UrunBeden"].ToString(),
                        UrunSezon = reader["UrunSezon"].ToString(),
                        UrunMateryal = reader["UrunMateryal"].ToString()
                    });
                }
                reader.Close();
            }

            // Ürün önerilerini yanıtla
            string response = "**İşte sizin için birkaç öneri:**\n";
            if (uygunUrunler.Any())
            {
                response += string.Join("\n", uygunUrunler.Select(urun =>
                    $"- **{urun.UrunMarka}** {urun.UrunAdı} | **{urun.UrunFiyat}₺** | Renk: {urun.UrunRenk}, Beden: {urun.UrunBeden}, Sezon: {urun.UrunSezon}, Materyal: {urun.UrunMateryal}"
                ));
            }
            else
            {
                response = "Üzgünüm, sizin için uygun ürün bulamadım. Lütfen başka seçenekler ile tekrar deneyin.";
            }

            return response;
        }
    }

    public class ChatMessage
    {
        public string Text { get; set; }
        public bool IsUser { get; set; }
    }

    public class UrunBOT
    {
        public string UrunAdı { get; set; }
        public string UrunMarka { get; set; }
        public int UrunFiyat { get; set; }
        public string UrunRenk { get; set; }
        public string UrunBeden { get; set; }
        public string UrunSezon { get; set; }
        public string UrunMateryal { get; set; }
    }
}
