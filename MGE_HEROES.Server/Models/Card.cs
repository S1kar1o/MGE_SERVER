using Supabase.Postgrest.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace MGE_HEROES.Server.Models
{
    [Table("cards")]

    public class Card : BaseModel
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("description")]
        public string Description { get; set; }
    }
}
