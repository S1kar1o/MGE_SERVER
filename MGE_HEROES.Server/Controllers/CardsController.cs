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
         
            return Ok(response);
        }
        [HttpPost("generate/{userId}")]
        public async Task<IActionResult> GenerateNewCard(Guid userId)
        {
            try
            {
                var newCard = await _service.GenerateNewCardForPlayer(userId);
                if (newCard != null)
                {
                    return Ok(new GenerateCardResponse { Success = true, Card = newCard });
                }
                else
                {
                    return Ok(new GenerateCardResponse
                    {
                        Success = false,
                        Message = "You already have all cards! hope u like them and enjoying game"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
    [System.Serializable]
    public class GenerateCardResponse
    {
        public bool Success;
        public string Message;
        public CardDto Card; // Буде null, якщо Success = false
    }

}
