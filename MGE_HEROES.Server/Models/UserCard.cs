using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace MGE_HEROES.Server.Models
{
    [Table("user_cards")]
    public class UserCard: BaseModel
    {
        [Column("user_id")]
        public Guid OwnerId { get; set; }
        [Column("card_id")]
        public int CardId {  get; set; }
    }
}
