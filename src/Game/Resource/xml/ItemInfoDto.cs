using System.Xml.Serialization;

namespace Netsphere.Resource.xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "iteminfo")]
    public class ItemInfoDto
    {
        [XmlElement("category")]
        public ItemInfoCategoryDto[] category { get; set; }

        [XmlAttribute]
        public string string_table { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoCategoryDto
    {
        [XmlElement("sub_category")]
        public ItemInfoSubCategoryDto[] sub_category { get; set; }

        [XmlAttribute]
        public byte id { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoSubCategoryDto
    {
        [XmlElement("item")]
        public ItemInfoItemDto[] item { get; set; }

        [XmlAttribute]
        public byte id { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemDto
    {
        [XmlElement("action")]
        public ItemInfoItemActionDto action { get; set; }

        [XmlElement("attach")]
        public ItemInfoItemAttachDto[] attach { get; set; }

        [XmlElement("base")]
        public ItemInfoItemBaseDto @base { get; set; }

        [XmlElement("client")]
        public ItemInfoItemClientDto client { get; set; }

        [XmlElement("costume")]
        public ItemInfoItemCostumeDto[] costume { get; set; }

        [XmlElement("esper_chip")]
        public ItemInfoItemEsperChipDto[] esper_chip { get; set; }

        [XmlElement("variation")]
        public ItemInfoItemVariationDto[] variation { get; set; }

        [XmlElement("weapon")]
        public ItemInfoItemWeaponDto weapon { get; set; }

        [XmlAttribute]
        public ushort number { get; set; }

        [XmlAttribute]
        public string NAME { get; set; }

        [XmlAttribute]
        public string SEX { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemActionDto
    {
        public ItemInfoItemActionAbilityDto ability { get; set; }

        public ItemInfoItemActionResourcesDto resources { get; set; }

        [XmlElement("scene")]
        public ItemInfoItemActionSceneDto[] scene { get; set; }

        public ItemInfoItemActionIntegerDto[] integer { get; set; }

        public ItemInfoItemActionTextureDto texture { get; set; }

        [XmlElement("float")]
        public ItemInfoItemActionFloatDto[] @float { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemActionAbilityDto
    {
        [XmlAttribute]
        public string required_mp { get; set; }

        [XmlAttribute]
        public string decrement_mp { get; set; }

        [XmlAttribute]
        public string decrement_mp_delay { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemActionResourcesDto
    {
        [XmlAttribute]
        public byte type { get; set; }

        [XmlAttribute]
        public string slot_image_file { get; set; }

        [XmlAttribute]
        public string feature_image_file { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemActionSceneDto
    {
        [XmlAttribute]
        public string value { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemActionIntegerDto
    {
        [XmlAttribute]
        public int value { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemActionTextureDto
    {
        [XmlAttribute]
        public string value { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemActionFloatDto
    {
        [XmlAttribute]
        public string value { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemAttachDto
    {
        [XmlElement("Model_to_part")]
        public ItemInfoItemAttachModelToPartDto[] Model_to_part { get; set; }

        [XmlElement("to_node")]
        public ItemInfoItemAttachToNodeDto[] to_node { get; set; }

        [XmlElement("to_part")]
        public ItemInfoItemAttachToPartDto[] to_part { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemAttachModelToPartDto
    {
        [XmlAttribute]
        public string scene_file { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemAttachToNodeDto
    {
        [XmlAttribute]
        public string scene_file { get; set; }

        [XmlAttribute]
        public string parent_node { get; set; }

        [XmlAttribute]
        public byte animation_part { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemAttachToPartDto
    {
        [XmlAttribute]
        public string scene_file { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemBaseDto
    {
        public ItemInfoItemBaseInfoDto base_info { get; set; }

        public ItemInfoItemBaseOverlapEquipDto overlap_equip { get; set; }

        public ItemInfoItemBaseLicenseDto license { get; set; }

        public ItemInfoItemBaseParentDto parent { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemBaseInfoDto
    {
        [XmlAttribute]
        public string name_key { get; set; }

        [XmlAttribute]
        public byte require_level { get; set; }

        [XmlAttribute]
        public byte require_master { get; set; }

        [XmlAttribute]
        public byte exp_boost_percent { get; set; }

        [XmlAttribute]
        public byte pen_boost_percent { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemBaseOverlapEquipDto
    {
        [XmlAttribute]
        public byte limit { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemBaseLicenseDto
    {
        [XmlAttribute]
        public string require { get; set; }

        [XmlAttribute]
        public string script { get; set; }

        [XmlAttribute]
        public string script_func { get; set; }

        [XmlAttribute]
        public string license_map { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemBaseParentDto
    {
        [XmlAttribute]
        public ushort number { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemClientDto
    {
        public ItemInfoItemClientIconDto icon { get; set; }

        public ItemInfoItemClientAttribDto attrib { get; set; }

        public ItemInfoItemClientFeatureDto feature { get; set; }

        public ItemInfoItemClientOverlapEquipDto overlap_equip { get; set; }

        public ItemInfoItemClientSlaughterTagDto slaughter_tag { get; set; }

        public ItemInfoItemClientShopIconDto shopicon { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemClientIconDto
    {
        [XmlAttribute]
        public string image { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemClientAttribDto
    {
        [XmlAttribute]
        public string comment_key { get; set; }

        [XmlAttribute]
        public string feature_comment { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemClientFeatureDto
    {
        [XmlAttribute]
        public string comment_key { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemClientOverlapEquipDto
    {
        [XmlAttribute]
        public byte limit { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemClientSlaughterTagDto
    {
        [XmlAttribute]
        public byte index { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemClientShopIconDto
    {
        [XmlAttribute]
        public string image { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemCostumeDto
    {
        public ItemInfoItemCostumeWearingDto wearing { get; set; }

        public ItemInfoItemCostumeHidingDto hiding { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemCostumeWearingDto
    {
        [XmlAttribute]
        public string sex { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemCostumeHidingDto
    {
        [XmlAttribute]
        public string option { get; set; }

        [XmlAttribute]
        public string option_kid { get; set; }

        [XmlAttribute]
        public string option_adult { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemEsperChipDto
    {
        public ItemInfoItemEsperChipApplyPartsDto apply_parts { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemEsperChipApplyPartsDto
    {
        [XmlAttribute]
        public byte category_id { get; set; }

        [XmlAttribute]
        public byte sub_category_id { get; set; }

        [XmlAttribute]
        public byte weapon_type { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemVariationDto
    {
        [XmlElement("variation_item")]
        public ItemInfoItemVariationItemDto[] variation_item { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemVariationItemDto
    {
        [XmlAttribute]
        public byte diffuse { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemWeaponDto
    {
        [XmlElement("ability")]
        public ItemInfoItemWeaponAbilityDto ability { get; set; }

        [XmlElement("float")]
        public ItemInfoItemWeaponFloatDto[] @float { get; set; }

        [XmlElement("integer")]
        public ItemInfoItemWeaponIntegerDto[] @integer { get; set; }

        [XmlElement("resources")]
        public ItemInfoItemWeaponResourcesDto[] resources { get; set; }

        [XmlElement("scene")]
        public ItemInfoItemWeaponSceneDto[] scene { get; set; }

        [XmlElement("scene_kid")]
        public ItemInfoItemWeaponSceneKidDto[] scene_kid { get; set; }

        [XmlElement("sequence")]
        public ItemInfoItemWeaponSequenceDto[] sequence { get; set; }

        [XmlElement("string")]
        public ItemInfoItemWeaponStringDto[] @string { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemWeaponAbilityDto
    {
        [XmlAttribute]
        public byte type { get; set; }

        [XmlAttribute]
        public string rate_of_fire { get; set; }

        [XmlAttribute]
        public string power { get; set; }

        [XmlAttribute]
        public string move_speed_rate { get; set; }

        [XmlAttribute]
        public string attack_move_speed_rate { get; set; }

        [XmlAttribute]
        public int magazine_capacity { get; set; }

        [XmlAttribute]
        public int cracked_magazine_capacity { get; set; }

        [XmlAttribute]
        public int max_ammo { get; set; }

        [XmlAttribute]
        public string accuracy { get; set; }

        [XmlAttribute]
        public string range { get; set; }

        [XmlAttribute]
        public byte support_sniper_mode { get; set; }

        [XmlAttribute]
        public byte sniper_mode_fov { get; set; }

        [XmlAttribute]
        public string auto_target_distance { get; set; }

        [XmlAttribute]
        public byte Esper_ref_type { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemWeaponFloatDto
    {
        [XmlAttribute]
        public string value { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemWeaponIntegerDto
    {
        [XmlAttribute]
        public int value { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemWeaponResourcesDto
    {
        [XmlAttribute]
        public string reload_sound_file { get; set; }

        [XmlAttribute]
        public string slot_image_file { get; set; }

        [XmlAttribute]
        public string crosshair_file { get; set; }

        [XmlAttribute]
        public string crosshair_zoomin_file { get; set; }

        [XmlAttribute]
        public string auto_target_distance { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemWeaponSceneDto
    {
        [XmlAttribute]
        public string value { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemWeaponSceneKidDto
    {
        [XmlAttribute]
        public string value { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemWeaponSequenceDto
    {
        [XmlAttribute]
        public string parent_nodename { get; set; }

        [XmlAttribute]
        public string filename { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ItemInfoItemWeaponStringDto
    {
        [XmlAttribute]
        public string value { get; set; }
    }
}
