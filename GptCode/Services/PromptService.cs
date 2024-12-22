using GptCode.Models;
using System.Text;
using System.Text.Json;

namespace GptCode.Services {
    public interface IPromptService {
        public Task<OllamaResponse> CreateProjectPrompt(string prompt);
        public Task<OllamaResponse> PrepareCodePrompt(string prompt);
        public Task<OllamaResponse> SendUserPrompt(string prompt, string sysprompt = null);
    }
    public class PromptService : IPromptService {
        string modelEndpoint = "http://127.0.0.1:11434/api/generate";

        public async Task<OllamaResponse> CreateProjectPrompt(string prompt) {
            string createPrompt = $"Bu kod için proje oluşturma komutunu ver. Sadece komut satır olsun. Markup kullanmanı istemiyorum: {prompt}";
            var sendAction = await SendUserPrompt(createPrompt);
            return sendAction;
        }

        public async Task<OllamaResponse> PrepareCodePrompt(string prompt) {
            string sysPrompt = $"C# programlama dilini kullan. Konsol uygulamasında top level statement uygula. Strateji desenine uygun olarak dosyaları ayır ve isimlerini açıklama olmadan sade şekilde dizinleri ile beraber belirt. Referans nuget kütüphaneleri için csproj dosyasını değiştir";
            var sendAction = await SendUserPrompt(prompt, sysPrompt);
            return sendAction;
        }

        public async Task<OllamaResponse> SendUserPrompt(string prompt, string sysprompt = null) {
            var ollamaRequest = new OllamaRequest {
                Model = "deepseek-coder-v2:latest",
                System = sysprompt,
                Prompt = prompt,
                Stream = false
            };

            using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(15) };
            var ollamaJsonContent = JsonSerializer.Serialize(ollamaRequest);
            var ollamaContent = new StringContent(ollamaJsonContent, Encoding.UTF8, "application/json");

            var ollamaResponse = await client.PostAsync(modelEndpoint, ollamaContent);
            if (!ollamaResponse.IsSuccessStatusCode) {
                throw new Exception($"Error: {ollamaResponse.StatusCode}");
            }

            var ollamaResponseJson = await ollamaResponse.Content.ReadAsStringAsync();
            var ollamaOllamaResponse = JsonSerializer.Deserialize<OllamaResponse>(ollamaResponseJson);
            if (ollamaOllamaResponse == null || string.IsNullOrEmpty(ollamaOllamaResponse.response)) {
                throw new Exception($"Error: {ollamaResponse.StatusCode}");
            }

            return ollamaOllamaResponse;
        }
    }
}
