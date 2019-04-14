using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("channels")]
    public class ChannelEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column]
        public int PlayerLimit { get; set; }

        [Column]
        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        [Column]
        [Required]
        [MaxLength(40)]
        public string Description { get; set; }

        [Column]
        [Required]
        [MinLength(8)]
        [MaxLength(8)]
        public string Color { get; set; }

        [Column]
        public int MinLevel { get; set; }

        [Column]
        public int MaxLevel { get; set; }
    }
}
