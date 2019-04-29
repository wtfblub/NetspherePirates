using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("shop_items")]
    public class ShopItemEntity
    {
        [Key]
        public long Id { get; set; }

        [Column]
        public byte RequiredGender { get; set; }

        [Column]
        public byte RequiredLicense { get; set; }

        [Column]
        public byte Colors { get; set; }

        [Column]
        public byte UniqueColors { get; set; }

        [Column]
        public byte RequiredLevel { get; set; }

        [Column]
        public byte LevelLimit { get; set; }

        [Column]
        public byte RequiredMasterLevel { get; set; }

        [Column]
        public bool IsOneTimeUse { get; set; }

        [Column]
        public bool IsDestroyable { get; set; }

        [Column]
        public byte MainTab { get; set; }

        [Column]
        public byte SubTab { get; set; }

        public List<ShopItemInfoEntity> ItemInfos { get; set; } = new List<ShopItemInfoEntity>();
    }
}
