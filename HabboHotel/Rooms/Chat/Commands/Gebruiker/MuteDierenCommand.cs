using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;


namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class MuteDierenCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_mute_dieren"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Chat van dieren en baby's worden weergegeven (aan/uit)."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowPetSpeech = !Session.GetHabbo().AllowPetSpeech;
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `users` SET `pets_muted` = '" + ((Session.GetHabbo().AllowPetSpeech) ? 1 : 0) + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            if (Session.GetHabbo().AllowPetSpeech)
                Session.SendWhisper("Verandering aangebracht! Je ontvangt nu geen berichten meer van dieren en baby's.", 34);
            else
                Session.SendWhisper("Verandering aangebracht! Je ontvangt nu weer berichten van dieren en baby's.", 34);
        }
    }
}
