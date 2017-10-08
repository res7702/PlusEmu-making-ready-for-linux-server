using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class LeegDierenCommand : IChatCommand
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
            get { return "Leeg je dieren en baby's inventaris"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendMessage(new RoomCustomizedAlertComposer("Waarschuwing: Ben je er zeker van dat je je inventaris wilt legen?\n" +
                                         "Om dit te bevestigen toets in ':empty_dieren ja'"));
            }
            else
            {
                if (Params.Length == 2 && Params[1] == "ja")
                {
                    using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("DELETE FROM bots WHERE (room_id = '0' AND ai_type = 'pet' AND user_id = '" + Session.GetHabbo().Id + "')");
                        dbClient.RunQuery();
                        Session.SendMessage(new RoomCustomizedAlertComposer("Je dier en baby inventaris is geleegd! (Herlaad het hotel)"));
                    }
                }
                else if (Params.Length == 2 && Params[1] != "ja")
                {
                    Session.SendWhisper("Om te bevestigen moet je ':empty_dieren ja' typen.");
                }
            }
        }
    }
}