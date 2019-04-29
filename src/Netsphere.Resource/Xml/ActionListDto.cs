using System.Xml.Serialization;

namespace Netsphere.Resource.xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "Actionlist")]
    public class ActionListDto
    {
        [XmlElement("Action")]
        public ActionListActionDto[] Action { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ActionListActionDto
    {
        public ActionListActionAbilityDto ability { get; set; }

        public ActionListActionResourcesDto resources { get; set; }

        public ActionListActionSceneDto Scene { get; set; }

        public ActionListActionSeqDto Seq { get; set; }

        public ActionListActionSeqsoundDto Seqsound { get; set; }

        public ActionListActionTextureDto Texture { get; set; }

        public ActionListActionIntegerDto Integer { get; set; }

        public ActionListActionFloatDto Float { get; set; }

        [XmlAttribute]
        public uint name { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ActionListActionAbilityDto
    {
        [XmlAttribute]
        public string required_mp { get; set; }

        [XmlAttribute]
        public string decrement_mp { get; set; }

        [XmlAttribute]
        public string decrement_mp_delay { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ActionListActionResourcesDto
    {
        [XmlAttribute]
        public int type { get; set; }

        [XmlAttribute]
        public string slot_image_file { get; set; }

        [XmlAttribute]
        public string feature_image_file { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ActionListActionSceneDto
    {
        [XmlAttribute]
        public string Scene_Value1 { get; set; }

        [XmlAttribute]
        public string Scene_Value2 { get; set; }

        [XmlAttribute]
        public string Scene_Value3 { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ActionListActionSeqDto
    {
        [XmlAttribute]
        public string Seq_Value1 { get; set; }

        [XmlAttribute]
        public string Seq_Value2 { get; set; }

        [XmlAttribute]
        public string Seq_Value3 { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ActionListActionSeqsoundDto
    {
        [XmlAttribute]
        public string Seqsound_Value1 { get; set; }

        [XmlAttribute]
        public string Seqsound_Value3 { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ActionListActionTextureDto
    {
        [XmlAttribute]
        public string Texture_Value1 { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ActionListActionIntegerDto
    {
        [XmlAttribute]
        public int Integer_Value1 { get; set; }

        [XmlAttribute]
        public int Integer_Value2 { get; set; }

        [XmlAttribute]
        public int Integer_Value3 { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ActionListActionFloatDto
    {
        [XmlAttribute]
        public string Float_Value1 { get; set; }

        [XmlAttribute]
        public string Float_Value2 { get; set; }

        [XmlAttribute]
        public string Float_Value3 { get; set; }

        [XmlAttribute]
        public string Float_Value4 { get; set; }

        [XmlAttribute]
        public string Float_Value5 { get; set; }

        [XmlAttribute]
        public string Float_Value6 { get; set; }
    }
}
