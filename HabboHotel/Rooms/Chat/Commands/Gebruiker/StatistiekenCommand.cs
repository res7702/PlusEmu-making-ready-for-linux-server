#region

using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.GameClients;

using Quasar.Database.Interfaces;
#endregion

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    internal class StatistiekenCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_stats"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Check je account statistieken!"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            DataRow UserData = null;
            DataRow UserInfo = null;

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`username`,`mail`,`rank`,`motto`,`credits`,`activity_points`,`vip_points`,`gotw_points`,`online`,`rank_vip` FROM users WHERE `username` = @Username LIMIT 1");

                UserData = dbClient.getRow();
            }

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`username`,`mail`,`rank`,`motto`,`credits`,`activity_points`,`vip_points`,`gotw_points`,`online`,`rank_vip` FROM users WHERE `username` = @Username LIMIT 1");

                UserData = dbClient.getRow();
            }


            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `user_info` WHERE `user_id` = '" + Convert.ToInt32(UserData["id"]) + "' LIMIT 1");
                UserInfo = dbClient.getRow();
                if (UserInfo == null)
                {
                    dbClient.RunQuery("INSERT INTO `user_info` (`user_id`) VALUES ('" + Convert.ToInt32(UserData["id"]) + "')");

                    dbClient.SetQuery("SELECT * FROM `user_info` WHERE `user_id` = '" + Convert.ToInt32(UserData["id"]) + "' LIMIT 1");
                    UserInfo = dbClient.getRow();
                }
            }



            {
                var Minutes = Session.GetHabbo().GetStats().OnlineTime / 60;
                var Hours = Minutes / 60;
                var OnlineTime = Convert.ToInt32(Hours);
                var s = OnlineTime == 1 ? "" : "s";

                var HabboInfo = new StringBuilder();
                HabboInfo.Append("Account statistieken:\r\r");


                HabboInfo.Append("Credits: " + Session.GetHabbo().Credits + "\r");
                HabboInfo.Append("Duckets: " + Session.GetHabbo().Duckets + "\r");
                HabboInfo.Append("Overwinningspunten: " + Session.GetHabbo().GOTWPoints + "\r");
                HabboInfo.Append("Onlinetijd: " + OnlineTime + " uur" + "\r");
                HabboInfo.Append("Respect: " + Session.GetHabbo().GetStats().Respect + "\r\r");

                Session.SendNotification(HabboInfo.ToString());
            }
        }
    }
}