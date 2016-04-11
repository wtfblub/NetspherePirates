using System.Xml.Serialization;

namespace Netsphere.Resource.xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "experience")]
    public class ExperienceDto
    {
        [XmlElement("exp")]
        public ExperienceEXPDto[] exp { get; set; }

        [XmlAttribute]
        public string string_table { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ExperienceEXPDto
    {
        [XmlAttribute]
        public uint require { get; set; }

        [XmlAttribute]
        public uint accumulate { get; set; }

        [XmlAttribute]
        public string name_key { get; set; }
    }
}
