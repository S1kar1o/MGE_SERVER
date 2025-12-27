using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
namespace MGE_HEROES.Server.Models
{
    [Table("heroes")]

    public class Hero : BaseModel
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("description")]
        public string Description { get; set; }
    }
}
