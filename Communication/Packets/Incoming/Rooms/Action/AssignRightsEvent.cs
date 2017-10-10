using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Core;
using Quasar.HabboHotel.Rooms;

using Quasar.Communication.Packets.Outgoing.Rooms.Permissions;
using Quasar.Communication.Packets.Outgoing.Rooms.Settings;
using Quasar.HabboHotel.Users;

using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Cache;
using Quasar.HabboHotel.Global;

namespace Quasar.Communication.Packets.Incoming.Rooms.Action
{
    class AssignRightsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            int UserId = Packet.PopInt();

            Room Room = null;
            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session, true))
                return;

            if (Room.UsersWithRights.Contains(UserId))
            {
                ///Session.SendNotification(LanguageLocal.G("room_rights_has_rights_error"));
                return;
            }

            Room.UsersWithRights.Add(UserId);

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("INSERT INTO `room_rights` (`room_id`,`user_id`) VALUES ('" + Room.RoomId + "','" + UserId + "')");
            }

            RoomUser RoomUser = Room.GetRoomUserManager().GetRoomUserByHabbo(UserId);
            if (RoomUser != null && !RoomUser.IsBot)
            {
                RoomUser.SetStatus("flatctrl 1", "");
                RoomUser.UpdateNeeded = true;
                if (RoomUser.GetClient() != null)
                    RoomUser.GetClient().SendMessage(new YouAreControllerComposer(1));

                Session.SendMessage(new FlatControllerAddedComposer(Room.RoomId, RoomUser.GetClient().GetHabbo().Id, RoomUser.GetClient().GetHabbo().Username));
            }
            else
            {
                UserCache User =  QuasarEnvironment.GetGame().GetCacheManager().GenerateUser(UserId);
                if (User != null)
                    Session.SendMessage(new FlatControllerAddedComposer(Room.RoomId, User.Id, User.Username));
            }
        }

        private string LanguageLocal(string v)
        {
            throw new NotImplementedException();
        }
    }
}
