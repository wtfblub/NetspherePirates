using System.Xml.Serialization;

namespace Netsphere.Resource.xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "string_table")]
    public class StringTableDto
    {
        [XmlElement("string")]
        public StringTableStringDto[] @string { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class StringTableStringDto
    {
        [XmlAttribute]
        public string key { get; set; }

        [XmlAttribute]
        public string kor { get; set; }

        [XmlAttribute]
        public string ger { get; set; }

        [XmlAttribute]
        public string eng { get; set; }

        [XmlAttribute]
        public string fre { get; set; }

        [XmlAttribute]
        public string spa { get; set; }

        [XmlAttribute]
        public string ita { get; set; }

        [XmlAttribute]
        public string rus { get; set; }

        [XmlAttribute]
        public string ame { get; set; }

        [XmlAttribute]
        public string cns { get; set; }

        public override string ToString()
        {
            return key;
        }
    }
}
