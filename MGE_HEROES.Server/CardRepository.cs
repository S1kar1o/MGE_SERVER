namespace MGE_HEROES.Server
{
    using MGE_HEROES.Server.Models;

    public class CardRepository
    {
        private readonly Supabase.Client _client;

        public CardRepository(Supabase.Client client)
        {
            _client = client;
        }

        public async Task<List<Card>> GetAllCards()
        {
            var result = await _client.From<Card>().Get();
            return result.Models;
        }

        public async Task<List<UserCard>> GetUserCards(Guid userId)
        {
            var result = await _client.From<UserCard>()
                .Where(x => x.OwnerId == userId)
                .Get();

            return result.Models;
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
