using MGE_HEROES.Server.ModelDTO;
using MGE_HEROES.Server.Servises;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MGE_HEROES.Server.Controllers
{
    [ApiController]
    [Route("cards")]
    public class CardsController : ControllerBase
    {
        private readonly CardService _service;

        public CardsController(CardService service)
        {
            _service = service;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserCards(Guid userId)
        {
            var response = await _service.GetUserCards(userId);
            Console.WriteLine("HERRRRREEE");

            Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
            return Ok(response);
        }
        [HttpPost("grant")]
        public async Task<IActionResult> GrantCards([FromBody] CardGrantRequest req)
        {
            var ok = await _service.AddCards(req.UserId, req.CardIds);
            return ok ? Ok() : BadRequest();
        }

    }

}
