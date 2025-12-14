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

        public async Task<List<Card>> GetCardsByIds(List<int> ids)
        {
            var response = await _client
                .From<Card>()
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
