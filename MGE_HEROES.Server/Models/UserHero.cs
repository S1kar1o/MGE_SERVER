using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace MGE_HEROES.Server.Models
{
    [Table("user_heroes")]
    public class UserHero : BaseModel
    {
        [Column("user_id")]
        public Guid OwnerId { get; set; }
        [Column("hero_id")]
        public int HeroId { get; set; }
        [Column("is_selected")]
        public bool isSelected { get; set; }
    }
}
