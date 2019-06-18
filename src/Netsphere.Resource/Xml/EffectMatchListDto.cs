using System.Xml.Serialization;

namespace Netsphere.Resource.xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "effect_match_list")]
    public class EffectMatchListDto
    {
        [XmlElement("match_key")]
        public EffectMatchListItemEffectDto[] match_key { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class EffectMatchListItemEffectDto
    {
        [XmlAttribute]
        public uint id { get; set; }

        [XmlAttribute]
        public string name_key { get; set; }

        [XmlAttribute]
        public int grade { get; set; }
    }
}
