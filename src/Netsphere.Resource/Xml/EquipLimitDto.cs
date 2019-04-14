using System.Xml.Serialization;

namespace Netsphere.Resource.Xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "equip_limit")]
    public class EquipLimitDto
    {
        [XmlArrayItem("limit", IsNullable = false)]
        public EquipLimitLimitDto[] preset { get; set; }

        [XmlAttribute]
        public string string_table { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class EquipLimitLimitDto
    {
        [XmlElement("require_Item")]
        public EquipLimitLimitRequireItemDto[] require_Item { get; set; }

        [XmlAttribute]
        public int id { get; set; }

        [XmlAttribute]
        public string string_key { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class EquipLimitLimitRequireItemDto
    {
        [XmlAttribute]
        public uint Item_Id { get; set; }
    }
}
