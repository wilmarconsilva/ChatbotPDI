using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> EnviarMensagem([FromBody] Dominio.ObjetosValor.Chat.Mensagem mensagem, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(mensagem.Conteudo))
                return BadRequest("Mensagem não pode ser vazia.");

            Servicos.Chat chat = new(cancellationToken);

            string resposta = await chat.EnviarMensagem(mensagem.Conteudo);

            return Ok(resposta);
        }
    }
}
