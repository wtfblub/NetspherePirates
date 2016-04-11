using System.Xml.Serialization;

namespace Netsphere.Resource.xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "default_item")]
    public class DefaultItemDto
    {
        public DefaultItemMaleFemaleDto male { get; set; }

        public DefaultItemMaleFemaleDto female { get; set; }

        [XmlAttribute]
        public string string_table { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class DefaultItemMaleFemaleDto
    {
        [XmlElement("item")]
        public DefaultItemItemDto[] item { get; set; }

        [XmlAttribute]
        public string name_key { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class DefaultItemItemDto
    {
        [XmlAttribute]
        public byte category { get; set; }

        [XmlAttribute]
        public byte sub_category { get; set; }

        [XmlAttribute]
        public byte number { get; set; }

        [XmlAttribute]
        public byte variation { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
