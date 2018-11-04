using System.Collections.Generic;

namespace Netsphere.Server.Game.Data
{
    public class ItemEffect
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public IList<ItemEffectAttribute> Attributes { get; set; }

        public ItemEffect()
        {
            Attributes = new List<ItemEffectAttribute>();
        }

        public override string ToString()
        {
            return $"{Id}-{Name}";
        }
    }

    public class ItemEffectAttribute
    {
        public Attribute Attribute { get; set; }
        public int Value { get; set; }
        public float Rate { get; set; }
    }
}
