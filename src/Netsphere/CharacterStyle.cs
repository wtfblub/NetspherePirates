namespace Netsphere
{
    public struct CharacterStyle
    {
        public uint Value => this;

        public CharacterGender Gender { get; set; }
        public byte Hair { get; set; }
        public byte Face { get; set; }
        public byte Shirt { get; set; }
        public byte Pants { get; set; }
        public byte Unk { get; set; } // Not used
        public byte Slot { get; set; }

        public CharacterStyle(uint value)
        {
            Gender = (CharacterGender)(value & 1);
            Hair = (byte)(value >> 1 & 63);
            Face = (byte)(value >> 7 & 63);
            Shirt = (byte)(value >> 13 & 31);
            Pants = (byte)(value >> 18 & 1023);
            Unk = (byte)(value >> 23 & 31);
            Slot = (byte)(value >> 28);
        }

        public CharacterStyle(CharacterGender gender, byte slot, byte hair, byte face, byte shirt, byte pants)
        {
            Gender = gender;
            Hair = hair;
            Face = face;
            Shirt = shirt;
            Pants = pants;
            Unk = 0;
            Slot = slot;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator uint(CharacterStyle style)
        {
            var value = (byte)style.Gender | style.Hair << 1 | style.Face << 7 | style.Shirt << 13 | style.Pants << 18 | style.Slot << 28;
            return (uint)value;
        }

        public static implicit operator CharacterStyle(uint value)
        {
            return new CharacterStyle(value);
        }
    }
}
