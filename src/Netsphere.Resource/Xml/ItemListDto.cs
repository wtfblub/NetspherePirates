using System.Xml.Serialization;

namespace Netsphere.Resource.xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "itemlist")]
    public class ItemListDto
    {
        [XmlElement("item")]
        public ItemListItemDto[] item { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemListItemDto
    {
        public ItemListItemBaseDto @base { get; set; }

        public ItemListItemGraphicDto graphic { get; set; }

        public ItemListItemEsperchipDto esperchip { get; set; }

        public ItemListItemEtcDto etc { get; set; }

        public ItemListItemSequenceDto sequence { get; set; }

        [XmlAttribute]
        public uint item_key { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemListItemBaseDto
    {
        [XmlAttribute]
        public string name { get; set; }

        [XmlAttribute]
        public string name_key { get; set; }

        [XmlAttribute]
        public string attrib_comment_key { get; set; }

        [XmlAttribute]
        public string sex { get; set; }

        [XmlAttribute]
        public string feature_comment_key { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemListItemGraphicDto
    {
        [XmlAttribute]
        public string icon_image { get; set; }

        [XmlAttribute]
        public string to_node_scene_file1 { get; set; }

        [XmlAttribute]
        public string to_node_parent_node1 { get; set; }

        [XmlAttribute]
        public int to_node_animation_part1 { get; set; }

        [XmlAttribute]
        public string to_part_scene_file { get; set; }

        [XmlAttribute]
        public string hiding_option { get; set; }

        [XmlAttribute]
        public string to_node_scene_file2 { get; set; }

        [XmlAttribute]
        public string to_node_parent_node2 { get; set; }

        [XmlAttribute]
        public int to_node_animation_part2 { get; set; }

        [XmlAttribute]
        public string to_node_scene_file3 { get; set; }

        [XmlAttribute]
        public string to_node_scene_file4 { get; set; }

        [XmlAttribute]
        public string to_node_parent_node3 { get; set; }

        [XmlAttribute]
        public string to_node_parent_node4 { get; set; }

        [XmlAttribute]
        public int to_node_animation_part3 { get; set; }

        [XmlAttribute]
        public int to_node_animation_part4 { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemListItemEsperchipDto
    {
        [XmlAttribute]
        public int apply_category { get; set; }

        [XmlAttribute]
        public int apply_sub_category { get; set; }

        [XmlAttribute]
        public int weapon_type { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemListItemEtcDto
    {
        [XmlAttribute]
        public int equip_limit { get; set; }

        [XmlAttribute]
        public int parent_number { get; set; }

        [XmlAttribute]
        public int chaser_tag { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemListItemSequenceDto
    {
        [XmlAttribute]
        public string node_name1 { get; set; }

        [XmlAttribute]
        public string file_name1 { get; set; }

        [XmlAttribute]
        public string node_name2 { get; set; }

        [XmlAttribute]
        public string file_name2 { get; set; }

        [XmlAttribute]
        public string node_name3 { get; set; }

        [XmlAttribute]
        public string file_name3 { get; set; }
    }
}
