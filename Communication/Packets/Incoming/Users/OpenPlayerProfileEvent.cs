using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.Groups;
using Quasar.Communication.Packets.Outgoing.Users;
using Quasar.Database.Interfaces;


namespace Quasar.Communication.Packets.Incoming.Users
{
    class OpenPlayerProfileEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int userID = Packet.PopInt();
            Boolean IsMe = Packet.PopBoolean();

            Habbo targetData = QuasarEnvironment.GetHabboById(userID);
            if (targetData == null)
            {
                Session.SendNotification("Er is een fout opgetreden bij het willen bekijken van deze Habbis profiel.");
                return;
             }
            
            List<Group> Groups = QuasarEnvironment.GetGame().GetGroupManager().GetGroupsForUser(targetData.Id);
            
            int friendCount = 0;
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT COUNT(0) FROM `messenger_friendships` WHERE (`user_one_id` = @userid OR `user_two_id` = @userid)");
                dbClient.AddParameter("userid", userID);
                friendCount = dbClient.getInteger();
            }

            Session.SendMessage(new ProfileInformationComposer(targetData, Session, Groups, friendCount));
            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CheckUsersProfile", 1);
        }
    }
}
