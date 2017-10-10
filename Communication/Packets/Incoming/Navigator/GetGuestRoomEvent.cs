using Quasar.Communication.Packets.Outgoing.Navigator;
using Quasar.Communication.Packets.Outgoing.Sound;
using Quasar.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Incoming.Navigator
{
    class GetGuestRoomEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int roomID = Packet.PopInt();

            RoomData roomData = QuasarEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomID);
            if (roomData == null)
                return;

            Boolean isLoading = Packet.PopInt() == 1;
            Boolean checkEntry = Packet.PopInt() == 1;

            Session.SendMessage(new GetGuestRoomResultComposer(Session, roomData, isLoading, checkEntry));
            Room room;
            if (QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(roomID, out room))
                Session.SendMessage(new SetJukeboxNowPlayingComposer(room));
        }
    }
}
