using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using BlubLib.IO;
using BlubLib.Serialization;
using Netsphere.Network.Data.P2P;
using Netsphere.Network.Serializers;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Message.P2P
{
    [BlubContract]
    public class PlayerSpawnReqMessage : IP2PMessage
    {
        [BlubMember(0)]
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

    [BlubContract]
    public class PlayerSpawnAckMessage : IP2PMessage
    {
        [BlubMember(0)]
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

    [BlubContract]
    public class AbilitySyncMessage : IP2PMessage
    {
        [BlubMember(0, typeof(CompressedFloatSerializer))]
        public float Unk { get; set; }

        [BlubMember(1, typeof(ArrayWithIntPrefixSerializer))]
        public ValueDto[] Values { get; set; }

        public AbilitySyncMessage()
        {
            Values = Array.Empty<ValueDto>();
        }
    }

    [BlubContract]
    public class EquippingItemSyncMessage : IP2PMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Costumes { get; set; }

        [BlubMember(1, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Skills { get; set; }

        [BlubMember(2, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Weapons { get; set; }

        [BlubMember(3, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Values { get; set; }

        public EquippingItemSyncMessage()
        {
            Costumes = Array.Empty<ItemDto>();
            Skills = Array.Empty<ItemDto>();
            Weapons = Array.Empty<ItemDto>();
            Values = Array.Empty<ItemDto>();
        }
    }

    [BlubContract(typeof(DamageInfoMessage.Serializer))]
    public class DamageInfoMessage : IP2PMessage
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

        internal class Serializer : ISerializer<DamageInfoMessage>
        {
            public bool CanHandle(Type type) => type == typeof(DamageInfoMessage);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Serialize(BinaryWriter writer, DamageInfoMessage value)
            {
                writer.Write(value.Target);
                writer.WriteEnum(value.AttackAttribute);
                writer.Write(value.GameTime);
                writer.Write(value.Source);
                writer.Write(value.Unk5);
                writer.WriteRotation(value.Rotation);
                writer.WriteCompressed(value.Position);
                writer.WriteCompressed(value.Unk6);
                writer.WriteCompressed(value.Damage);
                writer.Write(value.Unk8);
                writer.Write(value.Unk9);

                var ls = new List<byte>();
                var bw = new BitStreamWriter(ls);
                bw.Write(value.Flag1, 3);
                bw.Write(value.Flag2, 2);
                bw.Write(value.Flag3, 1);
                bw.Write(value.Flag4, 1);
                bw.Write(value.Flag5, 1);

                bw.Write(value.Flag6, 1);
                bw.Write(value.Flag7, 7);

                bw.Write(value.IsCritical, 4);
                bw.Write(value.Flag9, 4);

                writer.Write(ls);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DamageInfoMessage Deserialize(BinaryReader reader)
            {
                var message = new DamageInfoMessage();
                message.Target = reader.ReadUInt16();
                message.AttackAttribute = reader.ReadEnum<AttackAttribute>();
                message.GameTime = reader.ReadUInt32();
                message.Source = reader.ReadUInt16();
                message.Unk5 = reader.ReadByte();
                message.Rotation = reader.ReadRotation();
                message.Position = reader.ReadCompressedVector3();
                message.Unk6 = reader.ReadCompressedFloat();
                message.Damage = reader.ReadCompressedFloat();
                message.Unk8 = reader.ReadInt16();
                message.Unk9 = reader.ReadUInt16();

                var br = new BitStreamReader(reader.ReadBytes(3));
                message.Flag1 = br.ReadByte(3);
                message.Flag2 = br.ReadByte(2);
                message.Flag3 = br.ReadByte(1);
                message.Flag4 = br.ReadByte(1);
                message.Flag5 = br.ReadByte(1);

                message.Flag6 = br.ReadByte(1);
                message.Flag7 = br.ReadByte(7);

                message.IsCritical = br.ReadByte(4);
                message.Flag9 = br.ReadByte(4);

                return message;
            }
        }
    }

    [BlubContract(typeof(DamageRemoteInfoMessage.Serializer))]
    public class DamageRemoteInfoMessage : IP2PMessage
    {
        public PeerId Target { get; set; }
        public AttackAttribute AttackAttribute { get; set; }
        public uint GameTime { get; set; }
        public PeerId Source { get; set; }
        public Vector2 Rotation { get; set; }
        public Vector3 Position { get; set; }
        public float Unk { get; set; }
        public float Damage { get; set; }
        public byte Flag1 { get; set; }
        public byte Flag2 { get; set; }
        public byte Flag3 { get; set; }
        public byte Flag4 { get; set; }
        public byte Flag5 { get; set; }
        public byte Flag6 { get; set; }
        public byte Flag7 { get; set; }

        public DamageRemoteInfoMessage()
        {
            Target = 0;
            Source = 0;
            Position = Vector3.Zero;
            Rotation = Vector2.Zero;
        }

        internal class Serializer : ISerializer<DamageRemoteInfoMessage>
        {
            public bool CanHandle(Type type) => type == typeof(DamageRemoteInfoMessage);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Serialize(BinaryWriter writer, DamageRemoteInfoMessage value)
            {
                writer.Write(value.Target);
                writer.WriteEnum(value.AttackAttribute);
                writer.Write(value.GameTime);
                writer.Write(value.Source);
                writer.WriteRotation(value.Rotation);
                writer.WriteCompressed(value.Position);
                writer.WriteCompressed(value.Unk);
                writer.WriteCompressed(value.Damage);

                var ls = new List<byte>();
                var bw = new BitStreamWriter(ls);
                bw.Write(value.Flag1, 2);
                bw.Write(value.Flag2, 1);
                bw.Write(value.Flag3, 1);
                bw.Write(value.Flag4, 1);
                bw.Write(value.Flag5, 3);

                bw.Write(value.Flag6, 4);
                bw.Write(value.Flag7, 4);

                writer.Write(ls);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DamageRemoteInfoMessage Deserialize(BinaryReader reader)
            {
                var message = new DamageRemoteInfoMessage();
                message.Target = reader.ReadUInt16();
                message.AttackAttribute = reader.ReadEnum<AttackAttribute>();
                message.GameTime = reader.ReadUInt32();
                message.Source = reader.ReadUInt16();
                message.Rotation = reader.ReadRotation();
                message.Position = reader.ReadCompressedVector3();
                message.Unk = reader.ReadCompressedFloat();
                message.Damage = reader.ReadCompressedFloat();

                var br = new BitStreamReader(reader.ReadBytes(2));
                message.Flag1 = br.ReadByte(2);
                message.Flag2 = br.ReadByte(1);
                message.Flag3 = br.ReadByte(1);
                message.Flag4 = br.ReadByte(1);
                message.Flag5 = br.ReadByte(3);

                message.Flag6 = br.ReadByte(4);
                message.Flag7 = br.ReadByte(4);

                return message;
            }
        }
    }

    [BlubContract]
    public class SnapShotMessage : IP2PMessage
    {
        [BlubMember(0)]
        public uint Time { get; set; }

        [BlubMember(1)]
        public byte Unk { get; set; } // 3 bits, 5 bits

        [BlubMember(2, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [BlubMember(3, typeof(RotationVectorSerializer))]
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

    [BlubContract]
    public class StateSyncMessage : IP2PMessage
    {
        [BlubMember(0)]
        public uint GameTime { get; set; }

        [BlubMember(1)]
        public int Value { get; set; }

        [BlubMember(2)]
        public ActorState State { get; set; }

        [BlubMember(3)]
        public byte CurrentWeapon { get; set; } // 4 bits, 4 bits

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

    [BlubContract]
    public class BGEffectMessage : IP2PMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        [BlubMember(2, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [BlubMember(3)]
        public byte Unk3 { get; set; }

        [BlubMember(4)]
        public byte Unk4 { get; set; }

        [BlubMember(5)]
        public byte Unk5 { get; set; }

        [BlubMember(6)]
        public short Unk6 { get; set; }

        [BlubMember(7)]
        public byte Unk7 { get; set; }

        [BlubMember(8)]
        public byte Unk8 { get; set; } // 4 bits, 4 bits

        public BGEffectMessage()
        {
            Position = Vector3.Zero;
        }
    }

    [BlubContract]
    public class DefensivePowerMessage : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId PeerId { get; set; }

        [BlubMember(1, typeof(CompressedFloatSerializer))]
        public float Value { get; set; }

        public DefensivePowerMessage()
        {
            PeerId = 0;
        }
    }

    [BlubContract]
    public class BlastObjectDestroyMessage : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId Player { get; set; }

        [BlubMember(1, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk { get; set; }

        public BlastObjectDestroyMessage()
        {
            Player = 0;
            Unk = Array.Empty<int>();
        }
    }

    [BlubContract]
    public class BlastObjectRespawnMessage : IP2PMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk { get; set; }

        public BlastObjectRespawnMessage()
        {
            Unk = Array.Empty<int>();
        }
    }

    [BlubContract]
    public class MindEnergyMessage : IP2PMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public PeerId Target { get; set; }

        [BlubMember(2)]
        public short Unk3 { get; set; }

        [BlubMember(3, typeof(CompressedFloatSerializer))]
        public float Unk4 { get; set; }

        [BlubMember(4, typeof(CompressedFloatSerializer))]
        public float Unk5 { get; set; }

        [BlubMember(5)]
        public byte Unk6 { get; set; }

        public MindEnergyMessage()
        {
            Target = 0;
        }
    }

    [BlubContract]
    public class DamageShieldMessage : IP2PMessage
    {
        [BlubMember(0, typeof(CompressedFloatSerializer))]
        public float Unk { get; set; }
    }

    [BlubContract]
    public class AimedPointMessage : IP2PMessage
    {
        [BlubMember(0, typeof(CompressedVectorSerializer))]
        public Vector3 Unk1 { get; set; }

        [BlubMember(1, typeof(CompressedVectorSerializer))]
        public Vector3 Unk2 { get; set; }

        public AimedPointMessage()
        {
            Unk1 = Vector3.Zero;
            Unk2 = Vector3.Zero;
        }
    }

    [BlubContract]
    public class OnOffMessage : IP2PMessage
    {
        [BlubMember(0)]
        public byte Action { get; set; }

        [BlubMember(1)]
        public bool IsEnabled { get; set; }

        [BlubMember(2)]
        public byte Value { get; set; }
    }

    [BlubContract]
    public class SentryGunSpawnMessage : IP2PMessage
    {
        [BlubMember(0)]
        public LongPeerId Id { get; set; }

        // TODO Position?
        [BlubMember(1)]
        public float Unk2 { get; set; }

        [BlubMember(2)]
        public float Unk3 { get; set; }

        [BlubMember(3)]
        public float Unk4 { get; set; }

        [BlubMember(4, typeof(RotationVectorSerializer))]
        public Vector2 Rotation { get; set; }

        [BlubMember(5)]
        public byte Unk5 { get; set; }

        [BlubMember(6)]
        public int Unk6 { get; set; }

        [BlubMember(7, typeof(CompressedFloatSerializer))]
        public float Unk7 { get; set; }

        [BlubMember(8, typeof(CompressedFloatSerializer))]
        public float Unk8 { get; set; }

        [BlubMember(9, typeof(CompressedFloatSerializer))]
        public float Unk9 { get; set; }
    }

    [BlubContract]
    public class SentryGunStateMessage : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId Id { get; set; }

        [BlubMember(1)]
        public byte Unk1 { get; set; }

        [BlubMember(2)]
        public PeerId Unk2 { get; set; }
    }

    [BlubContract]
    public class SentryGunDestructionMessage : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId Id { get; set; }
    }

    [BlubContract]
    public class SentryGunDestruction2Message : IP2PMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class GrenadeSpawnMessage : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId Id { get; set; }

        [BlubMember(1)]
        public PeerId Owner { get; set; }

        [BlubMember(2, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [BlubMember(3, typeof(CompressedVectorSerializer))]
        public Vector3 Unk4 { get; set; }

        [BlubMember(4, typeof(CompressedFloatSerializer))]
        public float Unk5 { get; set; }

        [BlubMember(5, typeof(CompressedFloatSerializer))]
        public float Unk6 { get; set; }

        [BlubMember(6, typeof(StringSerializer))]
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

    [BlubContract]
    public class GrenadeSnapShotMessage : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId Id { get; set; }

        [BlubMember(1, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [BlubMember(2, typeof(CompressedVectorSerializer))]
        public Vector3 Unk2 { get; set; }

        [BlubMember(3)]
        public byte Unk3 { get; set; }

        public GrenadeSnapShotMessage()
        {
            Id = 0;
            Position = Vector3.Zero;
            Unk2 = Vector3.Zero;
        }
    }

    [BlubContract]
    public class GrenadeSnapShot2Message : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId Id { get; set; }

        [BlubMember(1, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        public GrenadeSnapShot2Message()
        {
            Id = 0;
            Position = Vector3.Zero;
        }
    }

    [BlubContract]
    public class ObstructionSpawnMessage : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId Owner { get; set; }

        [BlubMember(1)]
        public PeerId Id { get; set; }

        [BlubMember(2, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [BlubMember(3, typeof(RotationVectorSerializer))]
        public Vector2 Rotation { get; set; }

        [BlubMember(4)]
        public int Unk2 { get; set; }

        [BlubMember(5)]
        public int Unk3 { get; set; }

        [BlubMember(6)]
        public byte Unk4 { get; set; }

        public ObstructionSpawnMessage()
        {
            Owner = 0;
            Id = 0;
            Position = Vector3.Zero;
            Rotation = Vector2.Zero;
        }
    }

    [BlubContract]
    public class ObstructionDestroyMessage : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId Id { get; set; }

        public ObstructionDestroyMessage()
        { }

        public ObstructionDestroyMessage(PeerId id)
        {
            Id = id;
        }
    }

    [BlubContract]
    public class ObstructionDamageMessage : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId Id { get; set; }

        [BlubMember(1)]
        public uint Damage { get; set; }
    }

    [BlubContract]
    public class SyncObjectObstructionMessage : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId Owner { get; set; }

        [BlubMember(1)]
        public PeerId Id { get; set; }

        [BlubMember(2)]
        public uint GameTime { get; set; }

        [BlubMember(3, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [BlubMember(4, typeof(RotationVectorSerializer))]
        public Vector2 Rotation { get; set; }

        [BlubMember(5)]
        public uint Count { get; set; }

        [BlubMember(6)]
        public uint HP { get; set; }

        [BlubMember(7)]
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

    [BlubContract]
    public class BlastObjectSyncMessage : IP2PMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk { get; set; }

        public BlastObjectSyncMessage()
        {
            Unk = Array.Empty<int>();
        }
    }

    [BlubContract]
    public class BallSyncMessage : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId Player { get; set; }

        [BlubMember(1, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [BlubMember(2, typeof(CompressedVectorSerializer))]
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

    [BlubContract]
    public class BallSnapShotMessage : IP2PMessage
    {
        [BlubMember(0, typeof(CompressedVectorSerializer))]
        public Vector3 Position { get; set; }

        [BlubMember(1, typeof(CompressedVectorSerializer))]
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

    [BlubContract]
    public class ArcadeFinMessage : IP2PMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class AttachArcadeItemMessage : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId Id { get; set; }

        [BlubMember(1)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class HPSyncMessage : IP2PMessage
    {
        [BlubMember(0, typeof(CompressedFloatSerializer))]
        public float Value { get; set; }

        [BlubMember(1, typeof(CompressedFloatSerializer))]
        public float Max { get; set; }

        public HPSyncMessage()
        { }

        public HPSyncMessage(float value, float max)
        {
            Value = value;
            Max = max;
        }
    }

    [BlubContract]
    public class Unk38Message : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId Unk1 { get; set; }

        [BlubMember(1)]
        public PeerId Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3, typeof(CompressedFloatSerializer))]
        public float Unk4 { get; set; }
    }

    [BlubContract]
    public class ExposeClubMarkMessage : IP2PMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class ReflectRateMessage : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId Unk1 { get; set; }

        [BlubMember(1, typeof(CompressedFloatSerializer))]
        public float Unk2 { get; set; }
    }

    [BlubContract]
    public class ConditionInfoMessage : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId Unused { get; set; }

        [BlubMember(1)]
        public PeerId Target { get; set; }

        [BlubMember(2)]
        public Condition Condition { get; set; }

        [BlubMember(3, typeof(ArrayWithScalarSerializer))]
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

    [BlubContract]
    public class AbilityChangeSyncMessage : IP2PMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1, typeof(CompressedFloatSerializer))]
        public float Unk2 { get; set; }

        [BlubMember(2, typeof(CompressedFloatSerializer))]
        public float HP { get; set; }

        [BlubMember(3, typeof(CompressedFloatSerializer))]
        public float Unk3 { get; set; }

        [BlubMember(4, typeof(CompressedFloatSerializer))]
        public float MP { get; set; }

        [BlubMember(5, typeof(CompressedFloatSerializer))]
        public float Unk4 { get; set; }
    }

    [BlubContract]
    public class HealHPMessage : IP2PMessage
    {
        [BlubMember(0)]
        public PeerId Unk1 { get; set; }

        [BlubMember(1, typeof(CompressedFloatSerializer))]
        public float Unk2 { get; set; }
    }
}
