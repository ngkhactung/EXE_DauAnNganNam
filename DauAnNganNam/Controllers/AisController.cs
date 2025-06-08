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
        private readonly string _geminiApiKey = "AIzaSyB1f6_ab5qqLUazt3mbyh6XOd8mkc9E_XQ";
        private readonly string _azureSpeechKey = "AeKxhe4TvBEtmnLw1vE7tE54TbLcLkqvz8YtwXjyAoqkuRSEjunBJQQJ99BFACqBBLyXJ3w3AAAYACOGXE8J";
        private readonly string _azureSpeechRegion = "southeastasia";

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
        public async Task<IActionResult> ProcessVoiceChat(IFormFile audioFile)
        {
            if (audioFile == null || audioFile.Length == 0)
            {
                return BadRequest(new { error = "Không có file âm thanh." });
            }

            try
            {
                // STT: Audio -> Text
                string transcript = await ConvertSpeechToText(audioFile);
                if (string.IsNullOrWhiteSpace(transcript))
                    return BadRequest(new { error = "Không nhận diện được giọng nói." });

                // AI: Text -> Response
                string aiResponse = await GetGeminiResponse(transcript);
                if (string.IsNullOrWhiteSpace(aiResponse))
                    return BadRequest(new { error = "Không có phản hồi từ AI." });

                // TTS: Text -> Audio
                byte[] audioBytes = await ConvertTextToSpeech(aiResponse);
                if (audioBytes?.Length == 0)
                    return BadRequest(new { error = "Không tạo được âm thanh." });

                return File(audioBytes, "audio/wav");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AskGemini([FromBody] QuestionVM model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Question))
            {
                return BadRequest(new { error = "Câu hỏi không được để trống." });
            }

            try
            {
                string answer = await GetGeminiResponse(model.Question);
                return Json(new { answer = answer });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Lỗi: {ex.Message}" });
            }
        }

        private async Task<string> ConvertSpeechToText(IFormFile audioFile)
        {
            var endpoint = $"https://{_azureSpeechRegion}.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language=vi-VN";

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _azureSpeechKey);
            request.Content = new StreamContent(audioFile.OpenReadStream());
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/wav");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("DisplayText").GetString() ?? "";
        }

        private async Task<string> GetGeminiResponse(string question)
        {
            question = AddContextQuestion(question);

            var geminiModel = "gemini-2.5-flash-preview-05-20";
            var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{geminiModel}:generateContent?key={_geminiApiKey}";

            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = question } } } }
            };

            var response = await _httpClient.PostAsync(endpoint,
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var answer = doc.RootElement.GetProperty("candidates")[0]
                .GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

            return answer?.Replace("*", "").Replace("#", "").Trim() ?? "Không có phản hồi.";
        }

        private string AddContextQuestion(string question)
        {
            question = question + "\n Hãy trả lời câu hỏi bằng tiếng Việt, trả lời câu hỏi một cách ngắn gọn, đầy đủ."
                                + "\n Nếu câu hỏi yêu cầu giới thiệu về bản thân thì hãy trả lời như sau:"
                                + "\n Chào mừng bạn đến với Dấu Ấn Ngàn Năm – nơi lưu giữ những câu chuyện lịch sử và dấu ấn văn hóa Việt Nam qua các thời kỳ.\r\n\r\nChúng tôi mong muốn truyền tải vẻ đẹp và giá trị của những câu chuyện lịch sử qua các triển lãm, sự kiện và các nghiên cứu chuyên sâu. "
                                + "\n Mỗi phần của lịch sử là một mảnh ghép tạo nên sự vĩ đại của nền văn hóa dân tộc.\r\n\r\nĐược xây dựng từ niềm đam mê và sự tận tâm, Dấu ấn ngàn năm sẽ là nơi để bạn khám phá, học hỏi và kết nối với quá khứ của dân tộc.";
            return question;
        }

        private async Task<byte[]> ConvertTextToSpeech(string text)
        {
            var endpoint = $"https://{_azureSpeechRegion}.tts.speech.microsoft.com/cognitiveservices/v1";

            var ssml = $@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='vi-VN'>
                <voice name='vi-VN-HoaiMyNeural'>
                    <prosody rate='0.9' pitch='medium'>{System.Security.SecurityElement.Escape(text)}</prosody>
                </voice>
            </speak>";

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _azureSpeechKey);
            request.Headers.Add("X-Microsoft-OutputFormat", "riff-24khz-16bit-mono-pcm");
            request.Headers.Add("User-Agent", "DauAnNganNam");
            request.Content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}