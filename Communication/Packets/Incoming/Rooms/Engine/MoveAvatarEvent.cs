using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.Communication.Packets.Outgoing;
using Quasar.Communication.Packets.Outgoing.Nux;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.Communication.Packets.Incoming.Rooms.Engine
{
    class MoveAvatarEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null || !User.CanWalk)
                return;

            int MoveX = Packet.PopInt();
            int MoveY = Packet.PopInt();

            if (MoveX == User.X && MoveY == User.Y)
                return;

            if (User.RidingHorse)
            {
                RoomUser Horse = Room.GetRoomUserManager().GetRoomUserByVirtualId(User.HorseID);
                if (Horse != null)
                    Horse.MoveTo(MoveX, MoveY);
            }


            User.MoveTo(MoveX, MoveY);
            if (Session.GetHabbo()._NUX)
            {
                var nuxStatus = new ServerPacket(ServerPacketHeader.NuxUserStatus);
                nuxStatus.WriteInteger(2);
                Session.SendMessage(nuxStatus);
                Session.SendMessage(new NuxAlertComposer("nux/lobbyoffer/hide"));
                Session.SendMessage(new NuxAlertComposer("helpBubble/add/HC_JOIN_BUTTON/Hier kun je lidmaatschap aanvragen voor Habbis Club."));
            }
        }
    }
}