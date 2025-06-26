using OpenAI.Chat;

namespace Servicos
{
    public static class InstanciaClienteChat
    {
        private static readonly Lazy<ChatClient> _lazyClienteChat = new(() => new ChatClient("gpt-3.5-turbo", ""));

        public static ChatClient ClienteChat => _lazyClienteChat.Value;
    }
}