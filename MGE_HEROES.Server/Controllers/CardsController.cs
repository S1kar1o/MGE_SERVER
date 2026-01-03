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
                    GenerateCardResponse unswer = new GenerateCardResponse
                    {
                        success = true,
                        card = new[] { newCard }
                    };
                    Console.WriteLine(unswer);

                    return Ok(unswer);
                }
                else
                {
                    return Ok(new GenerateCardResponse
                    {
                        success = false,
                        message = "You already have all cards! hope u like them and enjoying game"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("generateTenCards/{userId}")]
        public async Task<IActionResult> GenerateNewTenCard(Guid userId)
        {
            try
            {
                var newCard = await _service.GenerateNewTenCards(userId);
                if (newCard != null)
                {
                    Console.WriteLine(newCard);

                    return Ok(new GenerateCardResponse { success = true, card = newCard });
                }
                else
                {
                    return Ok(new GenerateCardResponse
                    {
                        success = false,
                        message = "You already have all cards! hope u like them and enjoying game"
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
        public bool success;
        public string message;
        public CardDto[] card; // Буде null, якщо Success = false
    }

}
