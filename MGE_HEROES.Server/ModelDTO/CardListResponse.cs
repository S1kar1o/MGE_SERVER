namespace MGE_HEROES.Server.ModelDTO
{
    public class CardListResponse
    {
        public Guid UserId { get; set; }
        public List<string> Cards { get; set; }
    }
     public class CardDto
    {
        public string Name { get; set; }
    }
}
