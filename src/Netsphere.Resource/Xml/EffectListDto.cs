using System.Xml.Serialization;

namespace Netsphere.Resource.xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "effect_list")]
    public class EffectListDto
    {
        [XmlElement("item_effect")]
        public EffectListItemEffectDto[] item_effect { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class EffectListItemEffectDto
    {
        [XmlAttribute]
        public uint effect_id { get; set; }

        [XmlAttribute]
        public int main_type { get; set; }

        [XmlAttribute]
        public int upper_category { get; set; }

        [XmlAttribute]
        public int lower_category { get; set; }

        [XmlAttribute]
        public int effect_type { get; set; }

        [XmlAttribute]
        public int effect_condition { get; set; }

        [XmlAttribute]
        public int effect_level { get; set; }

        [XmlAttribute]
        public int effect_grade { get; set; }

        [XmlAttribute]
        public int value_min { get; set; }

        [XmlAttribute]
        public int value_max { get; set; }

        [XmlAttribute]
        public int rate_min { get; set; }

        [XmlAttribute]
        public int rate_max { get; set; }

        [XmlAttribute]
        public int select_prob { get; set; }

        [XmlAttribute]
        public int activation_rate { get; set; }

        [XmlAttribute]
        public int mode { get; set; }

        [XmlAttribute]
        public int map { get; set; }

        [XmlAttribute]
        public int gender { get; set; }

        [XmlAttribute]
        public int level_min { get; set; }

        [XmlAttribute]
        public int level_max { get; set; }

        [XmlAttribute]
        public string name_key { get; set; }
    }
}
