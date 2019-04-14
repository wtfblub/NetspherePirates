using System.Xml.Serialization;

namespace Netsphere.Resource.xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "gameinfo")]
    public class GameInfoDto
    {
        [XmlAttribute]
        public string string_table { get; set; }

        public GameInfoGameModeDto[] game_mode { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class GameInfoGameModeDto
    {
        [XmlAttribute]
        public int id { get; set; }

        [XmlAttribute]
        public bool use { get; set; }

        [XmlAttribute]
        public string image { get; set; }

        [XmlAttribute]
        public string tip_key { get; set; }

        [XmlAttribute]
        public string tag { get; set; }

        [XmlAttribute]
        public int category { get; set; }

        public GameInfoGameModeScoreTimeDto score_time { get; set; }
        public GameInfoGameModeLimitPlayerDto limit_player { get; set; }
        public GameInfoGameModeWeaponDropDto player_weapon_drop { get; set; }
        public GameInfoGameModeWeaponUnlimitDto player_weapon_unlimit { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class GameInfoGameModeScoreTimeDto
    {
        [XmlAttribute]
        public int select { get; set; }

        [XmlElement("data")]
        public GameInfoGameModeScoreTimeDataDto[] data { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class GameInfoGameModeScoreTimeDataDto
    {
        [XmlAttribute]
        public string score { get; set; }

        [XmlAttribute]
        public string score_string_key { get; set; }

        [XmlAttribute]
        public string time { get; set; }

        [XmlAttribute]
        public string time_string_key { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class GameInfoGameModeLimitPlayerDto
    {
        [XmlAttribute]
        public int select { get; set; }

        [XmlElement("data")]
        public GameInfoGameModeLimitPlayerDataDto[] data { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class GameInfoGameModeLimitPlayerDataDto
    {
        [XmlAttribute]
        public int player { get; set; }

        [XmlAttribute]
        public string player_string_key { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class GameInfoGameModeWeaponDropDto
    {
        [XmlAttribute]
        public bool active { get; set; }

        [XmlAttribute]
        public int prob { get; set; }

        [XmlAttribute]
        public int remove_time { get; set; }

        [XmlAttribute]
        public bool respone_reset { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class GameInfoGameModeWeaponUnlimitDto
    {
        [XmlAttribute]
        public bool active { get; set; }
    }
}
