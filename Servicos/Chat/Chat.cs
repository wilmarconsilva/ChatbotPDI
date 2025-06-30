using System.Text.Json;
using System.Text;

namespace Servicos
{
    public class Chat
    {
        private readonly CancellationToken _cancellationToken;
        private readonly string _promptIA = @"Você é um assistente que deverá ajudar o usuário a construir seu Plano de Desenvolvimento Individual (PDI).
                                            O usuário primeiramente irá informar o seu nome, pois ele já sabe que você precisa do nome, e que você irá assistenciar o mesmo a construir o seu próprio PDI.
                                            Você deve auxiliar na construção seguindo as etapas: 1 - Identificação Pessoal e Profissional, 2 - Autoavaliação de Competências, 3 - Objetivos de Curto, Médio e Longo Prazo, 4 - Análise de Gaps, 5 - Ações de Desenvolvimento, 6 - Prazos e Prioridades.
                                            A conversa com o usuário deve ser simples e objetiva para agilizar uma construção rápida do PDI.
                                            Ao final da construção do PDI, você deverá retornar um html agradável visualmente, com estilização moderna de um PDI com as informações do usuário.";

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
