using BlubLib.Network.Pipes;
using Netsphere.Network.Message.P2P;

namespace Netsphere.Network.Services
{
    internal class PeerService : MessageHandler
    {
        [MessageHandler(typeof(PlayerSpawnReqMessage))]
        public void PlayerSpawnReq(RelaySession session, PlayerSpawnReqMessage message)
        {
            var plr = session.Player;
            var roomInfo = plr.RoomInfo;

            if (roomInfo.PeerId != 0)
                return;

            roomInfo.PeerId = new LongPeerId(plr.Account.Id, message.Character.Id.PeerId);
        }

        [MessageHandler(typeof(PlayerSpawnAckMessage))]
        public void PlayerSpawnAck()
        { }

        [MessageHandler(typeof(StateSyncMessage))]
        public void StateSync(RelaySession session, StateSyncMessage message)
        {
            var plr = session.Player;

            if (plr.RoomInfo.Mode == PlayerGameMode.Normal && plr.RoomInfo.State != PlayerState.Lobby)
            {
                switch (message.State)
                {
                    case ActorState.Death:
                        plr.RoomInfo.State = PlayerState.Dead;
                        break;

                    case ActorState.Ghost:
                        if (plr.RoomInfo.State == PlayerState.Dead)
                            plr.RoomInfo.State = PlayerState.Dead;
                        break;

                    case ActorState.Respawn:
                        plr.RoomInfo.State = PlayerState.Alive;
                        break;
                }
            }
        }
    }
}
