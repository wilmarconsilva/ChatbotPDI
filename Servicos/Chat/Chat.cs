using System.Text.Json;
using System.Text;

namespace Servicos
{
    public class Chat
    {
        private readonly CancellationToken _cancellationToken;
        private readonly string _promptIA = "Você é um assistente que precisa cumprimentar o usuário com a seguinte mensagem: Olá tudo bem, eu sou a IA do Google";

        public Chat() : this(default) { }
        public Chat(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
        }

        public async Task<string> EnviarMensagem(string mensagem)
        {
            try
            {
                dynamic corpoRequisicao = new
                {
                    contents = new[]
                    {
                        new
                        {
                            role = "user",
                            parts = new[]
                            {
                                new { text = _promptIA }
                            }
                        },
                        new
                        {
                            role = "user",
                            parts = new[]
                            {
                                new { text = mensagem }
                            }
                        }
                    }
                };

                string jsonRequisicao = JsonSerializer.Serialize(corpoRequisicao, new JsonSerializerOptions { WriteIndented = false });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                Dominio.ObjetosValor.Chat.ClienteChat clienteChat = ClienteChat.BuscarInstancia();
                HttpResponseMessage resposta = await clienteChat.ClienteHTTP.PostAsync($"/v1beta/models/gemini-1.5-flash-latest:generateContent?key={clienteChat.ChaveAPI}", conteudoRequisicao, _cancellationToken);

                resposta.EnsureSuccessStatusCode();

                string responseString = await resposta.Content.ReadAsStringAsync(_cancellationToken);

                using (JsonDocument doc = JsonDocument.Parse(responseString))
                {
                    if (doc.RootElement.TryGetProperty("candidates", out JsonElement candidates) &&
                        candidates.ValueKind == JsonValueKind.Array &&
                        candidates.GetArrayLength() > 0)
                    {
                        JsonElement firstCandidate = candidates[0];

                        if (firstCandidate.TryGetProperty("content", out JsonElement content) &&
                            content.TryGetProperty("parts", out JsonElement parts) &&
                            parts.ValueKind == JsonValueKind.Array &&
                            parts.GetArrayLength() > 0)
                        {
                            JsonElement firstPart = parts[0];

                            if (firstPart.TryGetProperty("text", out JsonElement text))
                            {
                                return text.GetString() ?? "Não foi possível extrair a resposta textual.";
                            }
                        }
                    }
                }

                return "Resposta da IA malformada ou vazia.";
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erro HTTP: {ex.StatusCode} - {ex.Message}");
                return $"Erro na comunicação com a API da IA: {ex.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                return $"Ocorreu um erro inesperado: {ex.Message}";
            }
        }
    }
}
