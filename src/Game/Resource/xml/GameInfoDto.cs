using System.Xml.Serialization;

namespace Netsphere.Resource.xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "gameinfo")]
    public class GameInfoDto
    {
        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoMapDto[] map { get; set; }

        public GameInfoTutorialDto tutorial { get; set; }

        public GameInfoScoreDto score { get; set; }

        public GameInfoTimeDto time { get; set; }

        [XmlAttribute]
        public string string_table { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class GameInfoMapDto
    {
        [XmlAttribute]
        public short id { get; set; }

        [XmlAttribute]
        public string map_name_key { get; set; }

        [XmlAttribute]
        public byte require_level { get; set; }

        [XmlAttribute]
        public byte require_server { get; set; }

        [XmlAttribute]
        public byte require_channel { get; set; }

        [XmlAttribute]
        public byte respawn_type { get; set; }

        [XmlAttribute]
        public string bginfo_path { get; set; }

        [XmlAttribute]
        public bool dev_mode { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class GameInfoTutorialDto
    {
        [XmlAttribute]
        public byte map_id { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class GameInfoScoreDto
    {
        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoScoreDataDto[] death_match { get; set; }

        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoScoreDataDto[] touch_down { get; set; }

        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoScoreDataDto[] survival { get; set; }

        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoScoreDataDto[] mission { get; set; }

        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoScoreDataDto[] semi_td { get; set; }

        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoScoreDataDto[] arcade { get; set; }

        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoScoreDataDto[] slaughter { get; set; }

        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoScoreDataDto[] free_for_all { get; set; }

        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoScoreDataDto[] captain { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class GameInfoScoreDataDto
    {
        [XmlAttribute]
        public short score { get; set; }

        [XmlAttribute]
        public string score_string_key { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class GameInfoTimeDto
    {
        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoTimeDataDto[] death_match { get; set; }

        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoTimeDataDto[] touch_down { get; set; }

        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoTimeDataDto[] survival { get; set; }

        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoTimeDataDto[] mission { get; set; }

        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoTimeDataDto[] semi_td { get; set; }

        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoTimeDataDto[] arcade { get; set; }

        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoTimeDataDto[] slaughter { get; set; }

        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoTimeDataDto[] free_for_all { get; set; }

        [XmlArrayItem("data", IsNullable = false)]
        public GameInfoTimeDataDto[] captain { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class GameInfoTimeDataDto
    {
        [XmlAttribute]
        public short time { get; set; }

        [XmlAttribute]
        public string time_string_key { get; set; }
    }
}
