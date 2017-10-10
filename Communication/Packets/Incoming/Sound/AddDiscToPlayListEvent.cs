using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Sound;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.Communication.Packets.Incoming.Sound
{
    class AddDiscToPlayListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var room = Session.GetHabbo().CurrentRoom;
            if (!room.CheckRights(Session))
                return;
            //Console.WriteLine(Packet.ToString());

            var itemid = Packet.PopInt();//item id
            var songid = Packet.PopInt();//Song id

            var item = room.GetRoomItemHandler().GetItem(itemid);
            if (item == null)
                return;
            if (!room.GetTraxManager().AddDisc(item))
                Session.SendMessage(new RoomNotificationComposer("", "Oeps! Haal het oude item eerst uit de Jukebox.", "error", "", ""));

           
        }
    }
}
