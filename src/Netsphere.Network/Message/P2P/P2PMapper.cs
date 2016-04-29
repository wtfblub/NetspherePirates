using System;
using System.Collections.Generic;
using System.IO;
using BlubLib.Serialization;

namespace Netsphere.Network.Message.P2P
{
    public static class P2PMapper
    {
        private static readonly Dictionary<P2POpCode, Type> s_typeLookup = new Dictionary<P2POpCode, Type>();
        private static readonly Dictionary<Type, P2POpCode> s_opCodeLookup = new Dictionary<Type, P2POpCode>();

        static P2PMapper()
        {
            Create<PlayerSpawnReqMessage>(P2POpCode.PlayerSpawnReq);
            Create<PlayerSpawnAckMessage>(P2POpCode.PlayerSpawnAck);
            Create<AbilitySyncMessage>(P2POpCode.AbilitySync);
            Create<EquippingItemSyncMessage>(P2POpCode.EquippingItemSync);
            Create<DamageInfoMessage>(P2POpCode.DamageInfo);
            Create<DamageRemoteInfoMessage>(P2POpCode.DamageRemoteInfo);
            Create<SnapShotMessage>(P2POpCode.SnapShot);
            Create<StateSyncMessage>(P2POpCode.StateSync);
            Create<BGEffectMessage>(P2POpCode.BGEffect);
            Create<DefensivePowerMessage>(P2POpCode.DefensivePower);
            Create<BlastObjectDestroyMessage>(P2POpCode.BlastObjectDestroy);
            Create<BlastObjectRespawnMessage>(P2POpCode.BlastObjectRespawn);
            Create<MindEnergyMessage>(P2POpCode.MindEnergy);
            Create<DamageShieldMessage>(P2POpCode.DamageShield);
            Create<AimedPointMessage>(P2POpCode.AimedPoint);
            Create<OnOffMessage>(P2POpCode.OnOff);
            Create<SentryGunSpawnMessage>(P2POpCode.SentryGunSpawn);
            Create<SentryGunStateMessage>(P2POpCode.SentryGunState);
            Create<SentryGunDestructionMessage>(P2POpCode.SentryGunDestruction);
            Create<SentryGunDestruction2Message>(P2POpCode.SentryGunDestruction2);
            Create<GrenadeSpawnMessage>(P2POpCode.GrenadeSpawn);
            Create<GrenadeSnapShotMessage>(P2POpCode.GrenadeSnapShot);
            Create<GrenadeSnapShot2Message>(P2POpCode.GrenadeSnapShot2);
            Create<ObstructionSpawnMessage>(P2POpCode.ObstructionSpawn);
            Create<ObstructionDestroyMessage>(P2POpCode.ObstructionDestroy);
            Create<ObstructionDamageMessage>(P2POpCode.ObstructionDamage);
            Create<SyncObjectObstructionMessage>(P2POpCode.SyncObjectObstruction);
            Create<BlastObjectSyncMessage>(P2POpCode.BlastObjectSync);
            Create<BallSyncMessage>(P2POpCode.BallSync);
            Create<BallSnapShotMessage>(P2POpCode.BallSnapShot);
            Create<ArcadeFinMessage>(P2POpCode.ArcadeFin);
            Create<AttachArcadeItemMessage>(P2POpCode.AttachArcadeItem);
            Create<HPSyncMessage>(P2POpCode.HPSync);
            Create<Unk38Message>(P2POpCode.Unk38);
            Create<ExposeClubMarkMessage>(P2POpCode.ExposeClubMark);
            Create<ReflectRateMessage>(P2POpCode.ReflectRate);
            Create<ConditionInfoMessage>(P2POpCode.ConditionInfo);
            Create<AbilityChangeSyncMessage>(P2POpCode.AbilityChangeSync);
            Create<HealHPMessage>(P2POpCode.HealHP);
        }

        public static void Create<T>(P2POpCode opCode)
            where T : P2PMessage, new()
        {
            var type = typeof (T);
            s_opCodeLookup.Add(type, opCode);
            s_typeLookup.Add(opCode, type);
        }

        public static IEnumerable<P2PMessage> GetMessage(byte[] data)
        {
            using (var r = data.ToBinaryReader())
            {
                var test = new List<P2PMessage>();
                while (r.BaseStream.Position != r.BaseStream.Length)
                {
                    var opCode = r.ReadEnum<P2POpCode>();

                    Type type;
                    if (s_typeLookup.TryGetValue(opCode, out type))
                    {
                        var msg = (P2PMessage)Serializer.Deserialize(r, type);
                        test.Add(msg);
                    }
                    else
                    {
                        throw new Exception($"Invalid opCode {opCode}");
                    }
                }

                return test;
            }
        }

        public static P2POpCode GetOpCode<T>()
            where T : P2PMessage
        {
            return GetOpCode(typeof (T));
        }

        public static P2POpCode GetOpCode(Type type)
        {
            return s_opCodeLookup[type];
        }
    }
}