using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class LeegBotsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_empty"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Verwijder je Bots uit je inventory."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendMessage(new RoomCustomizedAlertComposer("Waarschuwing: Ben je er zeker van dat je je inventaris wilt legen?\n" +
                                         "Om dit te bevestigen toets in ':empty bots ja'"));
            }
            else
            {
                if (Params.Length == 2 && Params[1] == "ja")
                {
                    using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("DELETE FROM bots WHERE (room_id = '0' AND ai_type = generic AND user_id = '" + Session.GetHabbo().Id + "')");
                        // dbClient.SetQuery("DELETE FROM bots WHERE (room_id = '0' AND ai_type = 'bartender' AND user_id = '" + Session.GetHabbo().Id + "')");

                        dbClient.RunQuery();
                        Session.SendMessage(new RoomCustomizedAlertComposer("Je Bot's inventaris is geleegd! (Herlaad het hotel)"));
                    }
                }
                else if (Params.Length == 2 && Params[1] != "ja")
                {
                    Session.SendWhisper("Om te bevestigen moet je ':empty_bots ja' typen.");
                }
            }
        }
    }
}