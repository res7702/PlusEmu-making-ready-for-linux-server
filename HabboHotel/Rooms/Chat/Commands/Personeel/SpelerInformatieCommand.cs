using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.GameClients;

using Quasar.Database.Interfaces;


namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class SpelerInformatieCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_spelerinformatie"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Bekijk de algemene informatie van een speler."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je bent vergeten een gebruikersnaam in te voeren.");
                return;
            }

            DataRow UserData = null;
            DataRow UserInfo = null;
            string Username = Params[1];

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`username`,`mail`,`rank`,`motto`,`credits`,`activity_points`,`vip_points`,`gotw_points`,`online`,`rank_vip` FROM users WHERE `username` = @Username LIMIT 1");
                dbClient.AddParameter("Username", Username);
                UserData = dbClient.getRow();
            }

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`username`,`mail`,`rank`,`motto`,`credits`,`activity_points`,`vip_points`,`gotw_points`,`online`,`rank_vip` FROM users WHERE `username` = @Username LIMIT 1");
                dbClient.AddParameter("Username", Username);
                UserData = dbClient.getRow();
            }

            if (UserData == null)
            {
                Session.SendNotification("Oeps! De gebruiker " + Username + " bestaat niet!");
                return;
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

            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToDouble(UserInfo["trading_locked"]));

            StringBuilder HabboInfo = new StringBuilder();
            HabboInfo.Append("<b>Algemene Informatie</b>\r");
            HabboInfo.Append("Naam: " + Convert.ToString(UserData["username"]) + " (" + Convert.ToInt32(UserData["id"]) + ")\r");
          
            HabboInfo.Append("Rank: " + Convert.ToInt32(UserData["rank"]) + "\r");
            HabboInfo.Append("Premium rank: " + Convert.ToInt32(UserData["rank_vip"]) + "\r");
            HabboInfo.Append("Email: " + Convert.ToString(UserData["mail"]) + "\r");
            HabboInfo.Append("Status: " + (TargetClient != null ? "Online" : "Offline") + "\r\r");

            HabboInfo.Append("<b>Wisselkoers Informatie</b>\r");
            HabboInfo.Append("Credits: " + Convert.ToInt32(UserData["credits"]) + "\r");
            HabboInfo.Append("Duckets: " + Convert.ToInt32(UserData["activity_points"]) + "\r");
            HabboInfo.Append("Diamanten: " + Convert.ToInt32(UserData["vip_points"]) + "\r\r");

            HabboInfo.Append("<b>Moderator Informatie</b>\r");
            HabboInfo.Append("Bans: " + Convert.ToInt32(UserInfo["bans"]) + "\r");
            HabboInfo.Append("Tickets aangemaakt: " + Convert.ToInt32(UserInfo["cfhs"]) + "\r");
            HabboInfo.Append("Tickets misbruik: " + Convert.ToInt32(UserInfo["cfhs_abusive"]) + "\r");
            HabboInfo.Append("Ruil blokkering: " + (Convert.ToInt32(UserInfo["trading_locked"]) == 0 ? "Nee" : "Ja [" + (origin.ToString("dd/MM/yyyy")) + "]") + "\r\r");
            
            if (TargetClient != null)
            {
                HabboInfo.Append("<b>Kamer Informatie</b>\r");
                if (!TargetClient.GetHabbo().InRoom)
                    HabboInfo.Append(Convert.ToString(UserData["username"]) + "is niet in een kamer.\r");
                else
                {
                    HabboInfo.Append("Kamer naam: " + TargetClient.GetHabbo().CurrentRoom.Name + " (" + TargetClient.GetHabbo().CurrentRoom.RoomId + ")\r");
                    HabboInfo.Append("Kamer eigenaar: " + TargetClient.GetHabbo().CurrentRoom.OwnerName + "\r");
                    HabboInfo.Append("Kamer populatie: " + TargetClient.GetHabbo().CurrentRoom.UserCount + "/" + TargetClient.GetHabbo().CurrentRoom.UsersMax);
                }
            }
            Session.SendNotification(HabboInfo.ToString());
        }
    }
}
