using System.Xml.Serialization;

namespace Netsphere.Resource.Xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "maplist")]
    public class MapListDto
    {
        [XmlElement("map")]
        public MapListMapDto[] map { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class MapListMapDto
    {
        [XmlAttribute]
        public ushort id { get; set; }

        public MapListMapBaseDto @base { get; set; }
        public MapListMapResourseDto resourse { get; set; }
        public MapListMapSwitchDto @switch { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class MapListMapBaseDto
    {
        [XmlAttribute]
        public string map_name_key { get; set; }

        [XmlAttribute]
        public int mode_number { get; set; }

        [XmlAttribute]
        public int limit_player { get; set; }

        [XmlAttribute]
        public int index_number { get; set; }

        [XmlAttribute]
        public int weapon_recommend { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class MapListMapResourseDto
    {
        [XmlAttribute]
        public string bginfo_path { get; set; }

        [XmlAttribute]
        public string previewinfo_path { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class MapListMapSwitchDto
    {
        [XmlAttribute]
        public string kr { get; set; }

        [XmlAttribute]
        public string eu { get; set; }

        [XmlAttribute]
        public string cn { get; set; }

        [XmlAttribute]
        public string th { get; set; }

        [XmlAttribute]
        public string tw { get; set; }

        [XmlAttribute]
        public string jp { get; set; }

        [XmlAttribute]
        public string id { get; set; }

        [XmlAttribute]
        public string sa { get; set; }
    }
}
