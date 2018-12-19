using System.Xml.Serialization;

namespace Netsphere.Resource.Xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "Constant_info")]
    public class ConstantInfoDto
    {
        [XmlArrayItem("param_s32", IsNullable = false)]
        public ConstantInfoParamS32Dto[] s32 { get; set; }

        [XmlArrayItem("param_float", IsNullable = false)]
        public ConstantInfoParamFloatDto[] @float { get; set; }

        [XmlArrayItem("param_string", IsNullable = false)]
        public ConstantInfoParamStringDto[] @string { get; set; }

        [XmlArrayItem("GAMETEMPO", IsNullable = false)]
        public ConstantInfoGameTempoDto[] GAMEINFOLIST { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ConstantInfoParamS32Dto
    {
        [XmlAttribute]
        public uint data { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ConstantInfoParamFloatDto
    {
        [XmlAttribute]
        public string data { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ConstantInfoParamStringDto
    {
        [XmlAttribute]
        public string name { get; set; }

        [XmlAttribute]
        public string data { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ConstantInfoGameTempoDto
    {
        public ConstantInfoGameTempoTempValueDto TEMPVALUE { get; set; }

        public ConstantInfoGameTempoCommonTotalValueDto GAMETEPMO_COMMON_TOTAL_VALUE { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ConstantInfoGameTempoTempValueDto
    {
        [XmlAttribute]
        public string value { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ConstantInfoGameTempoCommonTotalValueDto
    {
        [XmlAttribute]
        public byte GAMETEMPO_fastrun_required_mp { get; set; }

        [XmlAttribute]
        public byte GAMETEMPO_fastrun_decrement_mp { get; set; }

        [XmlAttribute]
        public byte GAMETEMPO_fastrun_decrement_mp_delay { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_fastrun_speed_rate { get; set; }

        [XmlAttribute]
        public byte GAMETEMPO_dodge_required_mp { get; set; }

        [XmlAttribute]
        public ushort GAMETEMPO_dodge_speed { get; set; }

        [XmlAttribute]
        public ushort GAMETEMPO_dodge_decel { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_dodge_dodge_time { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_dodge_delay { get; set; }

        [XmlAttribute]
        public byte GAMETEMPO_dodge_after_stun_required_mp { get; set; }

        [XmlAttribute]
        public byte GAMETEMPO_walljump_required_mp { get; set; }

        [XmlAttribute]
        public ushort GAMETEMPO_walljump_jump_force { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_walljump_delay_before_jump { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_walljump_jump_time { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_walljump_landing_time { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_foot_sound_volume { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_actor_default_hp_max { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_actor_default_mp_max { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_gravity { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_max_gravity_velocity { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_backhit_bonus { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_actor_default_move_speed { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_actor_default_animation_move_speed { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_front_speed_rate { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_side_speed_rate { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_back_speed_rate { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_sit_speed_rate { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_jump_force { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_jump_move_rate { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_land_move_rate { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_jump_time { get; set; }

        [XmlAttribute]
        public byte GAMETEMPO_jump_predelay { get; set; }

        [XmlAttribute]
        public ushort GAMETEMPO_jump_landdelay { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_ray_maxdistance { get; set; }

        [XmlAttribute]
        public string GAMETEMPO_damage_multiplier { get; set; }
    }
}
