using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> EnviarMensagem([FromBody] Dominio.ObjetosValor.Chat.Mensagem mensagem, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(mensagem.Conteudo))
                return BadRequest("O conteúdo da mensagem não pode ser vazio.");

            Servicos.Chat chat = new(cancellationToken);

            string resposta = await chat.EnviarMensagem(mensagem.Conteudo);

            return Ok(resposta);
        }
    }
}
