using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> EnviarMensagem([FromBody] Mensagem mensagem)
        {
            if (string.IsNullOrWhiteSpace(mensagem.Conteudo))
                return BadRequest("Mensagem não pode ser vazia.");

            string resposta = mensagem.Conteudo + "/ teste";

            return Ok(resposta);
        }
    }
}
