namespace MGE_HEROES.Server.ModelDTO
{
    public class CardListResponse
    {
        public Guid UserId { get; set; }
        public List<int> Cards { get; set; }
    }
}
