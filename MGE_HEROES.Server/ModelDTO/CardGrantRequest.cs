namespace MGE_HEROES.Server.ModelDTO
{
    public class CardGrantRequest
    {
        public Guid UserId { get; set; }
        public List<int> CardIds { get; set; }
    }
}
