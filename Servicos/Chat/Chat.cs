namespace Servicos;

public class Chat
{
    private readonly CancellationToken _cancellationToken;

    public Chat() : this(default) { }
    public Chat(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
    }
    public async Task<string> EnviarMensagem(string mensagem)
    {
        return "";
    }
}
