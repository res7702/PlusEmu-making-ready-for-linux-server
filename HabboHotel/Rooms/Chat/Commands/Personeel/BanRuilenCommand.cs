using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;
using Quasar.Utilities;
using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.GameClients;



namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class BanRuilenCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_banruilen"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Leg een gebruiker een ruilverbod op. "; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je bent vergeten een gebruikersnaam en/of lengte van het ruilverbod in te vullen. (min 1 dag, max 365 dagen).");
                return;
            }

            Habbo Habbo = QuasarEnvironment.GetHabboByUsername(Params[1]);
            if (Habbo == null)
            {
                Session.SendWhisper("Oeps! Deze gebruiker bestaat niet.");
                return;
            }

            if (Convert.ToDouble(Params[2]) == 0)
            {
                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '0' WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1");
                }

                if (Habbo.GetClient() != null)
                {
                    Habbo.TradingLockExpiry = 0;
                    Habbo.GetClient().SendNotification("Je ruilverbod is opgeheven, ruilen is nu dus weer mogelijk!");
                }

                Session.SendWhisper("Je hebt de ruilverbod van " + Habbo.Username + " opgeheven.");
                return;
            }

            double Days;
            if (double.TryParse(Params[2], out Days))
            {
                if (Days < 1)
                    Days = 1;

                if (Days > 365)
                    Days = 365;

                double Length = (QuasarEnvironment.GetUnixTimestamp() + (Days * 86400));
                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '" + Length + "', `trading_locks_count` = `trading_locks_count` + '1' WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1");
                }

                if (Habbo.GetClient() != null)
                {
                    Habbo.TradingLockExpiry = Length;
                    Habbo.GetClient().SendNotification("Je hebt een ruil-ban van " + Days + " dag(en) gekregen!");
                }

                Session.SendWhisper("Gelukt! Je hebt een ruilverbod gegeven aan " + Habbo.Username + " voor " + Days +
                                    " dag(en).");
            }
            else
                Session.SendWhisper("Vul een geldig nummer in.");
        }
    }
}
