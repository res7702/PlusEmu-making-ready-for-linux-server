using Quasar.Communication.Packets.Outgoing;
using Quasar.HabboHotel.GameClients;

namespace Quasar.Communication.Packets.Incoming.Rooms.Camera
{
    public class RenderRoomMessageComposer : ServerPacket
    {
        public RenderRoomMessageComposer()
            : base(ServerPacketHeader.TakedRoomPhoto)
        {

        }
    }

    public class RenderRoomMessageComposerEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket paket)
        {
            Session.SendMessage(new RenderRoomMessageComposer());
        }
    }
}