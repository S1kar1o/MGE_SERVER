using MGE_HEROES.Server.Models;

namespace MGE_HEROES.Server.ModelDTO
{
    public class CardListResponse
    {
        public Guid UserId { get; set; }
        public List<CardDto> Cards { get; set; }
        public List<HeroDto> Heroes { get; set; }
    }
    public class CardDto
    {
        public string Name { get; set; }
        public short positionInDeck { get; set; }

    }public class HeroDto
    {
        public string Name { get; set; }
        public bool isSelected { get; set; }

    }
}
