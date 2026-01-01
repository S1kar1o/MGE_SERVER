namespace MGE_HEROES.Server
{
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

        public async Task<List<UserCard>> GetUserCards(Guid userId)
        {
            var response = await _client
                .From<UserCard>()
                .Where(uc => uc.OwnerId == userId)
                .Get();

            return response.Models;
        }
        public async Task<Card> GenerateNewCard(Guid userId)
        {
            var havenUnits = await GetUserCards(userId);
            var allUnitsResponse = await _client.From<Card>().Get();
            var allUnits = allUnitsResponse.Models;

            var ownedCardIds = havenUnits.Select(u => u.CardId).ToHashSet();
            List<Card> cards = allUnits.Where(c => !ownedCardIds.Contains(c.Id)).ToList();

            // Перевірка: якщо у гравця вже є всі карти, що існують у грі
            if (cards.Count == 0)
            {
                return null;
              //throw new Exception("У гравця вже є всі доступні карти.");
            }

            Random random = new Random();
            int rand = random.Next(0, cards.Count);
            Card selectedCard = cards[rand];

            // ПРАВИЛЬНО: створюємо список і додаємо туди ID обраної карти
            List<int> indexes = new List<int> { selectedCard.Id };

            // Не забудьте await, якщо метод асинхронний
            await AddCardsToUser(userId, indexes);

            return selectedCard;
        }
        public async Task<List<UserHero>> GetUserHeroes(Guid userId)
        {
            var response = await _client
                .From<UserHero>()
                .Where(uc => uc.OwnerId == userId)
                .Get();

            return response.Models;
        }

        public async Task<List<Card>> GetCardsByIds(List<int> ids)
        {
            var response = await _client
                .From<Card>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.In, ids)
                .Get();

            return response.Models;
        }
        public async Task<List<Hero>> GetHeroesByIds(List<int> ids)
        {
            var response = await _client
                .From<Hero>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.In, ids)
                .Get();

            return response.Models;
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
