using System.Xml.Serialization;

namespace Netsphere.Resource.xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "channel_setting")]
    public class ChannelSettingDto
    {
        public ChannelSettingSettingDto setting { get; set; }

        [XmlElement("channel_info")]
        public ChannelSettingChannelInfoDto[] channel_info { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ChannelSettingSettingDto
    {
        [XmlAttribute]
        public byte channel_cnt { get; set; }

        [XmlAttribute]
        public ushort limit_player { get; set; }

        [XmlAttribute]
        public byte default_category_id { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ChannelSettingChannelInfoDto
    {
        [XmlAttribute]
        public byte id { get; set; }

        [XmlAttribute]
        public byte type { get; set; }

        [XmlAttribute]
        public byte category { get; set; }

        [XmlAttribute]
        public string name_key { get; set; }

        [XmlAttribute]
        public string text1_key { get; set; }

        [XmlAttribute]
        public string text2_key { get; set; }

        [XmlAttribute]
        public uint color { get; set; }

        [XmlAttribute]
        public byte speed_channel { get; set; }

        [XmlAttribute]
        public byte club_channel { get; set; }

        [XmlAttribute]
        public string game_tempo { get; set; }

        public override string ToString()
        {
            return name_key;
        }
    }
}
