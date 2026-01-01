using MGE_HEROES.Server.ModelDTO;
using MGE_HEROES.Server.Models;

namespace MGE_HEROES.Server.Servises
{
    public class CardService
    {
        private readonly CardRepository _repo;

        public CardService(CardRepository repo)
        {
            _repo = repo;
        }

        public async Task<CardListResponse> GetUserCards(Guid userId)
        {
            var userCards = await _repo.GetUserCards(userId);
            var userHeroes = await _repo.GetUserHeroes(userId);
            if (!userCards.Any() && !userHeroes.Any())
                return new CardListResponse
                {
                    UserId = userId,
                    Cards = new List<CardDto>(),
                    Heroes = new List<HeroDto>()
                };

            var cardIds = userCards.Select(uc => uc.CardId).Distinct().ToList();
            var heroIds = userHeroes.Select(uc => uc.HeroId).Distinct().ToList();

            var cards = await _repo.GetCardsByIds(cardIds);
            var heroes = await _repo.GetHeroesByIds(heroIds);

            var cardsResult = userCards.Select(uc =>
            {
                var card = cards.First(c => c.Id == uc.CardId);
                return new CardDto
                {
                    Name = card.Name,
                    positionInDeck = uc.CardPosInDec
                };
            }).ToList();

            var heroResult = userHeroes.Select(uc =>
            {
                var hero = heroes.First(c => c.Id == uc.HeroId);
                return new HeroDto
                {
                    Name = hero.Name,
                    isSelected = uc.isSelected
                };

            }).ToList();

            return new CardListResponse
            {
                UserId = userId,
                Cards = cardsResult,
                Heroes = heroResult
            };
        }

        public async Task<CardDto> GenerateNewCardForPlayer(Guid userId)
        {
            var cardName = await _repo.GenerateNewCard(userId);
            short cardPosition = 0;//defaul non select position
            return new CardDto { Name = cardName.Name,
                positionInDeck = cardPosition
            };
        }
     
    }

}
