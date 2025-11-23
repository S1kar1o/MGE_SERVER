namespace MGE_HEROES.Server
{
    using MGE_HEROES.Server.ModelDTO;
    using MGE_HEROES.Server.Models;

    public class CardRepository
    {
        private readonly Supabase.Client _client;

        public CardRepository(GameDbContext gameDb)
        {
            _client = gameDb.GetClient();
        }

        public async Task<List<Card>> GetAllCards()
        {
            var result = await _client.From<Card>().Get();
            return result.Models;
        }

        public async Task<List<string>> GetUserCards(Guid userId)
        {
            // 1️⃣ Отримуємо всі записи user_cards для користувача
            var userCardsResponse = await _client
                .From<UserCard>()
                .Where(uc => uc.OwnerId == userId)
                .Get();

            var userCards = userCardsResponse.Models;

            if (!userCards.Any())
                return new List<string>();

            // 2️⃣ Створюємо список назв карт
            var cardNames = new List<string>();

            foreach (var uc in userCards)
            {
                var cardResponse = await _client
                    .From<Card>()
                    .Where(c => c.Id == uc.CardId)
                    .Get();

                cardNames.AddRange(cardResponse.Models.Select(c => c.Name));
            }

            return cardNames;
        }


        public async Task AddCardsToUser(Guid userId, List<int> cardIds)
        {
            foreach (var id in cardIds)
            {
                await _client.From<UserCard>().Insert(new UserCard
                {
                    OwnerId = userId,
                    CardId = id
                });
            }
        }
    }

}
