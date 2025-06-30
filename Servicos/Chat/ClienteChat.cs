namespace Servicos
{
    public static class ClienteChat
    {
        private static readonly string _chaveAPI = "";
        private static Dominio.ObjetosValor.Chat.ClienteChat? _clienteChat;

        public static Dominio.ObjetosValor.Chat.ClienteChat BuscarInstancia()
        {
            if (_clienteChat == null)
            {
                HttpClient clienteHTTP = new HttpClient();

                clienteHTTP.BaseAddress = new Uri("https://generativelanguage.googleapis.com");

                _clienteChat = new(clienteHTTP, _chaveAPI);
            }

            return _clienteChat;
        }
    }
}