using MGE_HEROES.Server.ModelDTO;
using MGE_HEROES.Server.Servises;
using Microsoft.AspNetCore.Mvc;

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
            var cards = await _service.GetUserCards(userId);
            return Ok(cards);
        }

        [HttpPost("grant")]
        public async Task<IActionResult> GrantCards([FromBody] CardGrantRequest req)
        {
            var ok = await _service.AddCards(req.UserId, req.CardIds);
            return ok ? Ok() : BadRequest();
        }

    }

}
