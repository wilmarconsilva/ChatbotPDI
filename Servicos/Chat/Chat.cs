using OpenAI.Chat;

namespace Servicos
{
    public class Chat
    {
        private readonly CancellationToken _cancellationToken;
        private readonly ChatClient _clienteChat = InstanciaClienteChat.ClienteChat;
        private readonly string _promptIA = "Você é um assistente útil que precisa cumprimentar o usuário, e se despedir após a próxima mensagem do mesmo";

        public Chat() : this(default) { }
        public Chat(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
        }
        public async Task<string> EnviarMensagem(string mensagem)
        {
            ChatMessage[] mensagens =
            [
                new SystemChatMessage(_promptIA),
                new UserChatMessage(mensagem),
            ];
            
            ChatCompletion resposta = await _clienteChat.CompleteChatAsync(mensagens, cancellationToken: _cancellationToken);

            return resposta.Content[0].Text;
        }
    }
}
