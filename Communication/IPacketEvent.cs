using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.GameClients;

namespace Quasar.Communication.Packets
{
    public interface IPacketEvent
    {
        void Parse(GameClient Session, ClientPacket Packet);
    }
}