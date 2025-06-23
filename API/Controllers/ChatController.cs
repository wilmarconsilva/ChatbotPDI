using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> EnviarMensagem([FromBody] Requisicao requisicao)
        {
            if (string.IsNullOrWhiteSpace(requisicao.Mensagem))
                return BadRequest("Mensagem não pode ser vazia.");

            string resposta = requisicao.Mensagem + "/ teste";

            return Ok(resposta);
        }
    }

    public class Requisicao
    {
        public required string Mensagem { get; set; }
    }
}
