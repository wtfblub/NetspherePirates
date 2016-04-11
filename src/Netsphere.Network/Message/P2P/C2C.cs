using System;
using System.Collections.Generic;
using System.IO;
using BlubLib.IO;
using BlubLib.Serialization;
using Netsphere.Network.Data.P2P;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;
using SlimMath;

namespace Netsphere.Network.Message.P2P
{
    public class PlayerSpawnReqMessage : P2PMessage
    {
        [Serialize(0)]
        public CharacterDto Character { get; set; }

        public PlayerSpawnReqMessage()
        {
            Character = new CharacterDto();
        }

        public PlayerSpawnReqMessage(CharacterDto character)
        {
            Character = character;
        }
    }

    public class PlayerSpawnAckMessage : P2PMessage
    {
        [Serialize(0)]
        public CharacterDto Character { get; set; }

        public PlayerSpawnAckMessage()
        {
            Character = new CharacterDto();
        }

        public PlayerSpawnAckMessage(CharacterDto character)
        {
            Character = character;
            //Character.Unk4 = 1;
        }
    }

    public class AbilitySyncMessage : P2PMessage
    {
        [Serialize(0, typeof(CompressedFloatSerializer))]
        public float Unk { get; set; }

        [Serialize(1, typeof(ArrayWithIntPrefixSerializer))]
        public ValueDto[] Values { get; set; }

        public AbilitySyncMessage()
        {
            Values = Array.Empty<ValueDto>();
        }
    }

    public class EquippingItemSyncMessage : P2PMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Costumes { get; set; }

        [Serialize(1, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Skills { get; set; }

        [Serialize(2, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Weapons { get; set; }

        [Serialize(3, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Values { get; set; }

        public EquippingItemSyncMessage()
        {
            Costumes = Array.Empty<ItemDto>();
            Skills = Array.Empty<ItemDto>();
            Weapons = Array.Empty<ItemDto>();
            Values = Array.Empty<ItemDto>();
        }
    }

    public class DamageInfoMessage : P2PMessage
    {
        public PeerId Target { get; set; }
        public AttackAttribute AttackAttribute { get; set; }
        public uint GameTime { get; set; }
        public PeerId Source { get; set; }
        public byte Unk5 { get; set; }
        public Vector2 Rotation { get; set; }
        public Vector3 Position { get; set; }
        public float Unk6 { get; set; }
        public float Damage { get; set; }
        public short Unk8 { get; set; }
        public ushort Unk9 { get; set; }

        public byte Flag1 { get; set; } // needs to be 2
        public byte Flag2 { get; set; }
        public byte Flag3 { get; set; }
        public byte Flag4 { get; set; }
        public byte Flag5 { get; set; }

        public byte Flag6 { get; set; }
        public byte Flag7 { get; set; }

        public byte IsCritical { get; set; }
        public byte Flag9 { get; set; }

        public DamageInfoMessage()
        {
            Target = 0;
            Source = 0;
            Position = Vector3.Zero;
            Rotation = Vector2.Zero;
            Unk5 = 18;
            Unk6 = 22.0625f;
            Flag1 = 2;
            Flag2 = 2;
        }
    }

    public class DamageRemoteInfoMessage : P2PMessage
    {
        [Serialize(0)]
        public PeerId Target { get; set; }

        [Serialize(1, typeof(EnumSerializer))]
        public AttackAttribute AttackAttribute { get; set; }

        [Serialize(2)]
        public uint GameTime { get; set; }

        [Serialize(3)]
        public PeerId Source { get; set; }

        [Serialize(4, typeof(RotationVectorSerializer))]
        public Vector2 Rotation { get; set; }

        [Serialize(5, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [Serialize(6, typeof(CompressedFloatSerializer))]
        public float Unk { get; set; }

        [Serialize(7, typeof(CompressedFloatSerializer))]
        public float Damage { get; set; }

        public DamageRemoteInfoMessage()
        {
            Target = 0;
            Source = 0;
            Position = Vector3.Zero;
            Rotation = Vector2.Zero;
        }
    }

    public class SnapShotMessage : P2PMessage
    {
        [Serialize(0)]
        public uint Time { get; set; }

        [Serialize(1)]
        public byte Unk { get; set; }

        [Serialize(2, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [Serialize(3, typeof(RotationVectorSerializer))]
        public Vector2 Rotation { get; set; }

        public SnapShotMessage()
        {
            Position = Vector3.Zero;
            Rotation = Vector2.Zero;
        }

        public SnapShotMessage(uint time, Vector3 position, Vector2 rotation, byte unk)
        {
            Time = time;
            Unk = unk;
            Position = position;
            Rotation = rotation;
        }
    }

    public class StateSyncMessage : P2PMessage
    {
        [Serialize(0)]
        public uint GameTime { get; set; }

        [Serialize(1)]
        public int Value { get; set; }

        [Serialize(2, typeof(EnumSerializer))]
        public ActorState State { get; set; }

        [Serialize(3)]
        public byte CurrentWeapon { get; set; }

        public StateSyncMessage()
        { }

        public StateSyncMessage(ActorState state)
        {
            State = state;
        }

        public StateSyncMessage(ActorState state, uint gameTime, int value, byte currentWeapon)
        {
            State = state;
            GameTime = gameTime;
            Value = value;
            CurrentWeapon = currentWeapon;
        }
    }

    public class BGEffectMessage : P2PMessage
    {
        [Serialize(0)]
        public int Unk1 { get; set; }

        [Serialize(1)]
        public byte Unk2 { get; set; }

        [Serialize(2, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [Serialize(3)]
        public byte Unk3 { get; set; }

        [Serialize(4)]
        public byte Unk4 { get; set; }

        [Serialize(5)]
        public byte Unk5 { get; set; }

        [Serialize(6)]
        public short Unk6 { get; set; }

        [Serialize(7)]
        public byte Unk7 { get; set; }

        [Serialize(8)]
        public byte Unk8 { get; set; }

        public BGEffectMessage()
        {
            Position = Vector3.Zero;
        }
    }

    public class DefensivePowerMessage : P2PMessage
    {
        [Serialize(0)]
        public PeerId PeerId { get; set; }

        [Serialize(1, typeof(CompressedFloatSerializer))]
        public float Value { get; set; }

        public DefensivePowerMessage()
        {
            PeerId = 0;
        }
    }

    public class BlastObjectDestroyMessage : P2PMessage
    {
        [Serialize(0)]
        public PeerId Player { get; set; }

        [Serialize(1, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk { get; set; }

        public BlastObjectDestroyMessage()
        {
            Player = 0;
            Unk = Array.Empty<int>();
        }
    }

    public class BlastObjectRespawnMessage : P2PMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk { get; set; }

        public BlastObjectRespawnMessage()
        {
            Unk = Array.Empty<int>();
        }
    }

    public class MindEnergyMessage : P2PMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public PeerId Target { get; set; }

        [Serialize(2)]
        public short Unk3 { get; set; }

        [Serialize(3, typeof(CompressedFloatSerializer))]
        public float Unk4 { get; set; }

        [Serialize(4, typeof(CompressedFloatSerializer))]
        public float Unk5 { get; set; }

        [Serialize(5)]
        public byte Unk6 { get; set; }

        public MindEnergyMessage()
        {
            Target = 0;
        }
    }

    public class DamageShieldMessage : P2PMessage
    {
        [Serialize(0, typeof(CompressedFloatSerializer))]
        public float Unk { get; set; }
    }

    public class AimedPointMessage : P2PMessage
    {
        [Serialize(0, typeof(CompressedVectorSerializer))]
        public Vector3 Unk1 { get; set; }

        [Serialize(1, typeof(CompressedVectorSerializer))]
        public Vector3 Unk2 { get; set; }

        public AimedPointMessage()
        {
            Unk1 = Vector3.Zero;
            Unk2 = Vector3.Zero;
        }
    }

    public class OnOffMessage : P2PMessage
    {
        [Serialize(0)]
        public byte Action { get; set; }

        [Serialize(1)]
        public bool IsEnabled { get; set; }

        [Serialize(2)]
        public byte Value { get; set; }
    }

    public class SentryGunSpawnMessage : P2PMessage
    {
        [Serialize(0)]
        public LongPeerId Id { get; set; }

        [Serialize(1)]
        public float Unk2 { get; set; }

        [Serialize(2)]
        public float Unk3 { get; set; }

        [Serialize(3)]
        public float Unk4 { get; set; }

        [Serialize(4, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [Serialize(5)]
        public byte Unk5 { get; set; }

        [Serialize(6)]
        public int Unk6 { get; set; }

        [Serialize(7, typeof(CompressedFloatSerializer))]
        public float Unk7 { get; set; }

        [Serialize(8, typeof(CompressedFloatSerializer))]
        public float Unk8 { get; set; }

        [Serialize(9, typeof(CompressedFloatSerializer))]
        public float Unk9 { get; set; }
    }

    public class SentryGunStateMessage : P2PMessage
    {
        [Serialize(0)]
        public PeerId Id { get; set; }

        [Serialize(1)]
        public byte Unk1 { get; set; }

        [Serialize(2)]
        public PeerId Unk2 { get; set; }
    }

    public class SentryGunDestructionMessage : P2PMessage
    {
        [Serialize(0)]
        public PeerId Id { get; set; }
    }

    public class SentryGunDestruction2Message : P2PMessage
    {
        [Serialize(0)]
        public int Unk1 { get; set; }

        [Serialize(1)]
        public int Unk2 { get; set; }
    }

    public class GrenadeSpawnMessage : P2PMessage
    {
        [Serialize(0)]
        public PeerId Id { get; set; }

        [Serialize(1)]
        public PeerId Owner { get; set; }

        [Serialize(2, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [Serialize(3, typeof(CompressedVectorSerializer))]
        public Vector3 Unk4 { get; set; }

        [Serialize(4, typeof(CompressedFloatSerializer))]
        public float Unk5 { get; set; }

        [Serialize(5, typeof(CompressedFloatSerializer))]
        public float Unk6 { get; set; }

        [Serialize(6, typeof(StringSerializer))]
        public string Unk7 { get; set; }

        public GrenadeSpawnMessage()
        {
            Id = 0;
            Owner = 0;
            Position = Vector3.Zero;
            Unk4 = Vector3.Zero;
            Unk7 = "";
        }
    }

    public class GrenadeSnapShotMessage : P2PMessage
    {
        [Serialize(0)]
        public PeerId Id { get; set; }

        [Serialize(1, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [Serialize(2, typeof(CompressedVectorSerializer))]
        public Vector3 Unk2 { get; set; }

        [Serialize(3)]
        public byte Unk3 { get; set; }

        public GrenadeSnapShotMessage()
        {
            Id = 0;
            Position = Vector3.Zero;
            Unk2 = Vector3.Zero;
        }
    }

    public class GrenadeSnapShot2Message : P2PMessage
    {
        [Serialize(0)]
        public PeerId Id { get; set; }

        [Serialize(1, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        public GrenadeSnapShot2Message()
        {
            Id = 0;
            Position = Vector3.Zero;
        }
    }

    public class ObstructionSpawnMessage : P2PMessage
    {
        [Serialize(0)]
        public PeerId Owner { get; set; }

        [Serialize(1)]
        public PeerId Id { get; set; }

        [Serialize(2, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [Serialize(3, typeof(RotationVectorSerializer))]
        public Vector2 Rotation { get; set; }

        [Serialize(4)]
        public int Unk2 { get; set; }

        [Serialize(5)]
        public int Unk3 { get; set; }

        [Serialize(6)]
        public byte Unk4 { get; set; }

        public ObstructionSpawnMessage()
        {
            Owner = 0;
            Id = 0;
            Position = Vector3.Zero;
            Rotation = Vector2.Zero;
        }
    }

    public class ObstructionDestroyMessage : P2PMessage
    {
        [Serialize(0)]
        public PeerId Id { get; set; }

        public ObstructionDestroyMessage()
        { }

        public ObstructionDestroyMessage(PeerId id)
        {
            Id = id;
        }
    }

    public class ObstructionDamageMessage : P2PMessage
    {
        [Serialize(0)]
        public PeerId Id { get; set; }

        [Serialize(1)]
        public uint Damage { get; set; }
    }

    public class SyncObjectObstructionMessage : P2PMessage
    {
        [Serialize(0)]
        public PeerId Owner { get; set; }

        [Serialize(1)]
        public PeerId Id { get; set; }

        [Serialize(2)]
        public uint GameTime { get; set; }

        [Serialize(3, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [Serialize(4, typeof(RotationVectorSerializer))]
        public Vector2 Rotation { get; set; }

        [Serialize(5)]
        public uint Count { get; set; }

        [Serialize(6)]
        public uint HP { get; set; }

        [Serialize(7)]
        public uint Time { get; set; }

        public SyncObjectObstructionMessage()
        {
            Owner = 0;
            Id = 0;
            Position = Vector3.Zero;
            Rotation = Vector2.Zero;
        }

        public SyncObjectObstructionMessage(PeerId owner, PeerId id, uint gameTime, Vector3 position, Vector2 rotation, uint count, uint hp, uint time)
        {
            Owner = owner;
            Id = id;
            GameTime = gameTime;
            Position = position;
            Rotation = rotation;
            Count = count;
            HP = hp;
            Time = time;
        }
    }

    public class BlastObjectSyncMessage : P2PMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk { get; set; }

        public BlastObjectSyncMessage()
        {
            Unk = Array.Empty<int>();
        }
    }

    public class BallSyncMessage : P2PMessage
    {
        [Serialize(0)]
        public PeerId Player { get; set; }

        [Serialize(1, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [Serialize(2, typeof(CompressedVectorSerializer))]
        public Vector3 Unk { get; set; }

        public BallSyncMessage()
        {
            Position = Vector3.Zero;
            Unk = Vector3.Zero;
        }

        public BallSyncMessage(PeerId player, Vector3 position)
        {
            Player = player;
            Position = position;
            //Unk = position;
        }
    }

    public class BallSnapShotMessage : P2PMessage
    {
        [Serialize(0, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [Serialize(1, typeof(CompressedVectorSerializer))]
        public Vector3 Unk { get; set; }

        public BallSnapShotMessage()
        {
            Position = Vector3.Zero;
            Unk = Vector3.Zero;
        }

        public BallSnapShotMessage(Vector3 position)
        {
            Position = position;
            Unk = position;
        }
    }

    public class ArcadeFinMessage : P2PMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }
    }

    public class AttachArcadeItemMessage : P2PMessage
    {
        [Serialize(0)]
        public PeerId Id { get; set; }

        [Serialize(1)]
        public int Unk { get; set; }
    }

    public class HPSyncMessage : P2PMessage
    {
        [Serialize(0, typeof(CompressedFloatSerializer))]
        public float Value { get; set; }

        [Serialize(1, typeof(CompressedFloatSerializer))]
        public float Max { get; set; }

        public HPSyncMessage()
        { }

        public HPSyncMessage(float value, float max)
        {
            Value = value;
            Max = max;
        }
    }

    public class Unk38Message : P2PMessage
    {
        [Serialize(0)]
        public PeerId Unk1 { get; set; }

        [Serialize(1)]
        public PeerId Unk2 { get; set; }

        [Serialize(2)]
        public int Unk3 { get; set; }

        [Serialize(3, typeof(CompressedFloatSerializer))]
        public float Unk4 { get; set; }
    }

    public class ExposeClubMarkMessage : P2PMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public byte Unk2 { get; set; }
    }

    public class ReflectRateMessage : P2PMessage
    {
        [Serialize(0)]
        public PeerId Unk1 { get; set; }

        [Serialize(1, typeof(CompressedFloatSerializer))]
        public float Unk2 { get; set; }
    }

    public class ConditionInfoMessage : P2PMessage
    {
        [Serialize(0)]
        public PeerId Unused { get; set; }

        [Serialize(1)]
        public PeerId Target { get; set; }

        [Serialize(2, typeof(EnumSerializer))]
        public Condition Condition { get; set; }

        [Serialize(3, typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; }

        public ConditionInfoMessage()
        {
            Data = Array.Empty<byte>();
        }

        public ConditionInfoMessage(PeerId target, Condition condition, byte[] data)
        {
            Unused = new PeerId(0, 11, 4);
            Target = target;
            Condition = condition;
            Data = data;
        }
    }

    public class AbilityChangeSyncMessage : P2PMessage
    {
        [Serialize(0)]
        public int Unk1 { get; set; }

        [Serialize(1, typeof(CompressedFloatSerializer))]
        public float Unk2 { get; set; }

        [Serialize(2, typeof(CompressedFloatSerializer))]
        public float HP { get; set; }

        [Serialize(3, typeof(CompressedFloatSerializer))]
        public float Unk3 { get; set; }

        [Serialize(4, typeof(CompressedFloatSerializer))]
        public float MP { get; set; }

        [Serialize(5, typeof(CompressedFloatSerializer))]
        public float Unk4 { get; set; }

        [Serialize(6)]
        public short Unk5 { get; set; }

        [Serialize(7)]
        public short Unk6 { get; set; }

        [Serialize(8)]
        public short Unk7 { get; set; }
    }

    public class HealHPMessage : P2PMessage
    {
        [Serialize(0)]
        public PeerId Unk1 { get; set; }

        [Serialize(1, typeof(CompressedFloatSerializer))]
        public float Unk2 { get; set; }
    }
}