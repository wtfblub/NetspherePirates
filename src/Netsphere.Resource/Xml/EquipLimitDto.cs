using System.Xml.Serialization;

namespace Netsphere.Resource.Xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "equip_limit")]
    public class EquipLimitDto
    {
        public EquipLimitPresetsDto preset { get; set; }

        [XmlAttribute]
        public string string_table { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class EquipLimitPresetsDto
    {
        public EquipLimitPresetDto sword { get; set; }
        public EquipLimitPresetDto rookie { get; set; }
        public EquipLimitPresetDto super { get; set; }
        public EquipLimitPresetDto s4 { get; set; }
        public EquipLimitPresetDto arcade { get; set; }
        public EquipLimitPresetDto slaughter { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class EquipLimitPresetDto
    {
        [XmlElement("require_license")]
        public EquipLimitPresetRequireLicenseDto[] require_license { get; set; }

        [XmlAttribute]
        public string feature_key { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class EquipLimitPresetRequireLicenseDto
    {
        [XmlAttribute]
        public string name { get; set; }
    }
}
