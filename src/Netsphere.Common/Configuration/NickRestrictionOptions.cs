namespace Netsphere.Common.Configuration
{
    public class NickRestrictionOptions
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public int MaxRepeat { get; set; }
        public bool WhitespaceAllowed { get; set; }
        public bool AsciiOnly { get; set; }
    }
}
