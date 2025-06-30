namespace Dominio.ObjetosValor.Chat;

public class Mensagem
{
    public Mensagem(string conteudo)
    {
        Conteudo = conteudo;
    }

    public string Conteudo { get; }
}