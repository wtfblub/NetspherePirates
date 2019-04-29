namespace Netsphere.Server.Game.Data
{
    public class ItemEffect
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public EffectType EffectType { get; set; }
        public float Value { get; set; }
        public float Rate { get; set; }

        public override string ToString()
        {
            return $"{Id}-{Name}";
        }
    }
}
