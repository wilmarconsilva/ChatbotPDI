namespace Dominio.ObjetosValor.Chat;

public class ClienteChat
{
    public ClienteChat(HttpClient clienteHTTP, string chaveAPI)
    {
        ClienteHTTP = clienteHTTP;
        ChaveAPI = chaveAPI;
    }

    public HttpClient ClienteHTTP { get; }
    public string ChaveAPI { get; }
}