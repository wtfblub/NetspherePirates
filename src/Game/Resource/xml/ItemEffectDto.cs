using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Netsphere.Resource.xml
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "item_effect")]
    public class ItemEffectDto
    {
        [XmlAttribute]
        public string string_table { get; set; }

        [XmlElement("item")]
        public ItemEffectItemDto[] item { get; set; }
    }

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class ItemEffectItemDto
    {
        [XmlAttribute]
        public uint id { get; set; }

        [XmlAttribute]
        public string NAME { get; set; }

        [XmlAttribute]
        public string text_key { get; set; }

        [XmlAttribute]
        public string image { get; set; }

        [XmlAttribute]
        public byte pumbi_star { get; set; }

        [XmlAttribute]
        public string desc_key { get; set; }

        [XmlElement("attribute")]
        public ItemEffectItemAttributeDto[] attribute { get; set; }
    }

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class ItemEffectItemAttributeDto
    {
        [XmlAttribute]
        public string effect { get; set; }

        [XmlAttribute]
        public int graph_value { get; set; }

        [XmlAttribute]
        public int value { get; set; }

        [XmlAttribute]
        public string rate { get; set; }
    }
}
