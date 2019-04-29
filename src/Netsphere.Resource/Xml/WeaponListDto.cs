using System.Xml.Serialization;

namespace Netsphere.Resource.xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "weaponlist")]
    public class WeaponListDto
    {
        [XmlElement("weapon")]
        public WeaponListWeaponDto[] weapon { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class WeaponListWeaponDto
    {
        public WeaponListWeaponAbilityDto ability { get; set; }

        public WeaponListWeaponSceneDto scene { get; set; }

        public WeaponListWeaponResourcesDto resources { get; set; }

        public WeaponListWeaponSequenceDto sequence { get; set; }

        public WeaponListWeaponOtherEffectsDto other_effects { get; set; }

        public WeaponListWeaponSequence2Dto sequence2 { get; set; }

        [XmlAttribute]
        public uint item_key { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class WeaponListWeaponAbilityDto
    {
        [XmlAttribute]
        public int type { get; set; }

        [XmlAttribute]
        public int category { get; set; }

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
        public short max_ammo { get; set; }

        [XmlAttribute]
        public short max_ammo_limit { get; set; }

        [XmlAttribute]
        public string accuracy { get; set; }

        [XmlAttribute]
        public string range { get; set; }

        [XmlAttribute]
        public string charge_power_rate { get; set; }

        [XmlAttribute]
        public int support_sniper_mode { get; set; }

        [XmlAttribute]
        public int sniper_mode_fov { get; set; }

        [XmlAttribute]
        public int rand_dmg_min { get; set; }

        [XmlAttribute]
        public int rand_dmg_max { get; set; }

        [XmlAttribute]
        public int rand_dmg_prob { get; set; }

        [XmlAttribute]
        public int new_snipe_mode { get; set; }

        [XmlAttribute]
        public int centry_type { get; set; }

        [XmlAttribute]
        public string auto_target_distance { get; set; }

        [XmlAttribute]
        public int esper_ref_type { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class WeaponListWeaponSceneDto
    {
        [XmlAttribute]
        public string value1 { get; set; }

        [XmlAttribute]
        public string value2 { get; set; }

        [XmlAttribute]
        public string value3 { get; set; }

        [XmlAttribute]
        public string value4 { get; set; }

        [XmlAttribute]
        public string value5 { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class WeaponListWeaponResourcesDto
    {
        [XmlAttribute]
        public string reload_sound_file { get; set; }

        [XmlAttribute]
        public string slot_image_file { get; set; }

        [XmlAttribute]
        public string crosshair_file { get; set; }

        [XmlAttribute]
        public string crosshair_zoomin_file { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class WeaponListWeaponSequenceDto
    {
        [XmlAttribute]
        public int count { get; set; }

        [XmlAttribute]
        public string node_name_1 { get; set; }

        [XmlAttribute]
        public string file_name_1 { get; set; }

        [XmlAttribute]
        public string node_name_2 { get; set; }

        [XmlAttribute]
        public string file_name_2 { get; set; }

        [XmlAttribute]
        public string node_name_3 { get; set; }

        [XmlAttribute]
        public string file_name_3 { get; set; }

        [XmlAttribute]
        public string node_name_4 { get; set; }

        [XmlAttribute]
        public string file_name_4 { get; set; }

        [XmlAttribute]
        public string node_name_5 { get; set; }

        [XmlAttribute]
        public string file_name_5 { get; set; }

        [XmlAttribute]
        public string node_name_6 { get; set; }

        [XmlAttribute]
        public string file_name_6 { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class WeaponListWeaponOtherEffectsDto
    {
        [XmlAttribute]
        public string other_type { get; set; }

        [XmlAttribute]
        public string attack_seq { get; set; }

        [XmlAttribute]
        public string attack_ogg { get; set; }

        [XmlAttribute]
        public string charge_normal_seq { get; set; }

        [XmlAttribute]
        public string charge_normal_ogg { get; set; }

        [XmlAttribute]
        public string charge_max_seq { get; set; }

        [XmlAttribute]
        public string charge_max_ogg { get; set; }

        [XmlAttribute]
        public string charge_normal_parent { get; set; }

        [XmlAttribute]
        public string charge_normal_attack_seq { get; set; }

        [XmlAttribute]
        public string charge_normal_attack_ogg { get; set; }

        [XmlAttribute]
        public string charge_max_parent { get; set; }

        [XmlAttribute]
        public string charge_max_attack_seq { get; set; }

        [XmlAttribute]
        public string charge_max_attack_ogg { get; set; }

        [XmlAttribute]
        public string heal_seq { get; set; }

        [XmlAttribute]
        public string heal_ogg { get; set; }

        [XmlAttribute]
        public string attack_line_1_seq { get; set; }

        [XmlAttribute]
        public string attack_line_2_seq { get; set; }

        [XmlAttribute]
        public string attack_line_3_seq { get; set; }

        [XmlAttribute]
        public string heal_line_1_seq { get; set; }

        [XmlAttribute]
        public string heal_line_2_seq { get; set; }

        [XmlAttribute]
        public string heal_line_3_seq { get; set; }

        [XmlAttribute]
        public string attack_hand_seq { get; set; }

        [XmlAttribute]
        public string heal_hand_seq { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class WeaponListWeaponSequence2Dto
    {
        [XmlAttribute]
        public int count_2 { get; set; }

        [XmlAttribute]
        public string node_name_2_1 { get; set; }

        [XmlAttribute]
        public string file_name_2_1 { get; set; }

        [XmlAttribute]
        public string node_name_2_2 { get; set; }

        [XmlAttribute]
        public string file_name_2_2 { get; set; }

        [XmlAttribute]
        public string node_name_2_3 { get; set; }

        [XmlAttribute]
        public string file_name_2_3 { get; set; }
    }
}
