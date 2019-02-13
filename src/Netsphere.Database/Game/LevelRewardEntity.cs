using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("level_rewards")]
    public class LevelRewardEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Level { get; set; }

        [Column]
        public byte MoneyType { get; set; }

        [Column]
        public int Money { get; set; }
    }
}
