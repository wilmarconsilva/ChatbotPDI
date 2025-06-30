using System.Text.Json;
using System.Text;

namespace Servicos
{
    public class Chat
    {
        private readonly CancellationToken _cancellationToken;
        private readonly Dominio.ObjetosValor.Chat.ClienteChat _clienteChat = InstanciaClienteChat.ClienteChat;
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

                HttpResponseMessage resposta = await _clienteChat.ClienteHTTP.PostAsync($"/v1beta/models/gemini-2.5-pro:generateContent?key={_clienteChat.ChaveAPI}", conteudoRequisicao, _cancellationToken);

                resposta.EnsureSuccessStatusCode();

                string responseString = await resposta.Content.ReadAsStringAsync(_cancellationToken);

                using (JsonDocument doc = JsonDocument.Parse(responseString))
                {
                    if (doc.RootElement.TryGetProperty("candidates", out JsonElement candidates) &&
                        candidates.EnumerateArray().Any())
                    {
                        if (candidates.EnumerateArray().First().TryGetProperty("content", out JsonElement contentElement) &&
                            contentElement.TryGetProperty("parts", out JsonElement parts) &&
                            parts.EnumerateArray().Any())
                        {
                            if (parts.EnumerateArray().First().TryGetProperty("text", out JsonElement textElement))
                            {
                                return textElement.GetString() ?? "Não foi possível extrair a resposta textual.";
                            }
                        }
                    }
                }

                return "Resposta da IA vazia ou em formato inesperado.";
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
