using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.Communication.Packets.Outgoing.Navigator;

using Quasar.Database.Interfaces;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.HabboHotel.GameClients;

namespace Quasar.Communication.Packets.Incoming.Rooms.Action
{
    class GiveRoomScoreEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (Session.GetHabbo().RatedRooms.Contains(Room.RoomId) || Room.CheckRights(Session, true))
                return;

            int Rating = Packet.PopInt();
            switch (Rating)
            {
                case -1:

                    Room.Score--;
                    Room.RoomData.Score--;
                    break;

                case 1:

                    Room.Score++;
                    Room.RoomData.Score++;
                    break;

                default:

                    return;
            }


            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE rooms SET score = '" + Room.Score + "' WHERE id = '" + Room.RoomId + "' LIMIT 1");
            }

            Session.GetHabbo().RatedRooms.Add(Room.RoomId);
            Session.SendMessage(new RoomRatingComposer(Room.Score, !(Session.GetHabbo().RatedRooms.Contains(Room.RoomId) || Room.CheckRights(Session, true))));
            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_StemOpKamer", 1);
           
            Room.SendMessage(RoomNotificationComposer.SendBubble("bubble_kamerstem", "Stemmer: " + Session.GetHabbo().Username + "\n\nDe kamer heeft nu totaal " + Room.Score + " stemmen ontvangen!"));
            return;

           

        }
    }
}
