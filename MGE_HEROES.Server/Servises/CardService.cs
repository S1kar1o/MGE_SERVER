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
            var userCards = await _repo.GetUserCards(userId); // List<string>

            return new CardListResponse
            {
                UserId = userId,
                Cards = userCards // просто передаємо список назв
            };
        }

        public async Task<bool> AddCards(Guid userId, List<int> cardIds)
        {
            await _repo.AddCardsToUser(userId, cardIds);
            return true;
        }
    }

}
