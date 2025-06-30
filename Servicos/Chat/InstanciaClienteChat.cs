namespace Servicos
{
    public static class InstanciaClienteChat
    {
        private static readonly string _chaveAPI = "";

        private static readonly Lazy<Dominio.ObjetosValor.Chat.ClienteChat> _lazyClienteChat = new(() =>
        {
            HttpClient clienteHTTP = new HttpClient();

            clienteHTTP.BaseAddress = new Uri("https://generativelanguage.googleapis.com");

            Dominio.ObjetosValor.Chat.ClienteChat clienteChat = new(clienteHTTP, _chaveAPI);

            return clienteChat;
        });

        public static Dominio.ObjetosValor.Chat.ClienteChat ClienteChat => _lazyClienteChat.Value;
    }
}