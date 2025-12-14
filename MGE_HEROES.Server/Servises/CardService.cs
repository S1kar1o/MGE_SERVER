using MGE_HEROES.Server.ModelDTO;

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

            if (!userCards.Any())
                return new CardListResponse
                {
                    UserId = userId,
                    Cards = new List<CardDto>()
                };

            var cardIds = userCards.Select(uc => uc.CardId).Distinct().ToList();
            var cards = await _repo.GetCardsByIds(cardIds);

            var result = userCards.Select(uc =>
            {
                var card = cards.First(c => c.Id == uc.CardId);
                return new CardDto
                {
                    Name = card.Name,
                    positionInDeck = uc.CardPosInDec
                };
            }).ToList();

            return new CardListResponse
            {
                UserId = userId,
                Cards = result
            };
        }


        public async Task<bool> AddCards(Guid userId, List<int> cardIds)
        {
            await _repo.AddCardsToUser(userId, cardIds);
            return true;
        }
    }

}
