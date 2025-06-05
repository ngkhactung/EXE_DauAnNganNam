using DauAnNganNam.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DauAnNganNam.Controllers
{
    public class AisController : Controller
    {
        private readonly string _apiKey = "AIzaSyB1f6_ab5qqLUazt3mbyh6XOd8mkc9E_XQ"; // Store securely
        private readonly HttpClient _httpClient;

        public AisController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Chatbot()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AskGemini([FromBody] QuestionVM model)
        {
            Console.WriteLine($"Received question: {model?.Question}");
            if (model == null || string.IsNullOrWhiteSpace(model.Question))
            {
                return BadRequest(new { error = "Câu hỏi không được để trống." });
            }

            try
            {
                model = LimitAnswer(model);

                var geminiModel = "gemini-2.5-flash-preview-05-20";
                var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{geminiModel}:generateContent?key={_apiKey}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = model.Question }
                            }
                        }
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { error = $"Lỗi từ Gemini API: {errorContent}" });
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(responseBody);
                string? answer = jsonDoc.RootElement.GetProperty("candidates")[0]
                    .GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

                answer = answer?.Replace("*", "")
                              ?.Replace("#", "")
                              ?.Replace("**", "")
                              ?.Trim();

                return Json(new { answer = answer ?? "Không nhận được phản hồi hợp lệ từ Gemini." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Lỗi: {ex.Message}" });
            }
        }

        private QuestionVM LimitAnswer(QuestionVM model)
        {
            model.Question = model.Question + "\n Hãy trả lời câu hỏi bằng tiếng Việt, trả lời câu hỏi một cách ngắn gọn, đầy đủ.";
            model.Question = model.Question + "\n Nếu câu hỏi yêu cầu giới thiệu về bản thân thì hãy trả lời như sau:"
                            + "\n Chào mừng bạn đến với Dấu Ấn Ngàn Năm – nơi lưu giữ những câu chuyện lịch sử và dấu ấn văn hóa Việt Nam qua các thời kỳ.\r\n\r\nChúng tôi mong muốn truyền tải vẻ đẹp và giá trị của những câu chuyện lịch sử qua các triển lãm, sự kiện và các nghiên cứu chuyên sâu. Mỗi phần của lịch sử là một mảnh ghép tạo nên sự vĩ đại của nền văn hóa dân tộc.\r\n\r\nĐược xây dựng từ niềm đam mê và sự tận tâm, Dấu ấn ngàn năm sẽ là nơi để bạn khám phá, học hỏi và kết nối với quá khứ của dân tộc.";

            return model;
        }

    }
}