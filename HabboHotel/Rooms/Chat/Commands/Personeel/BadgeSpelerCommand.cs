using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

using Quasar.HabboHotel.GameClients;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class BadgeSpelerCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_badgespeler"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Geef een speler een badge."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length != 3)
            {
                Session.SendWhisper("Oeps! Je bent vergeten om de naam van de gebruiker waar je de badge naar toe wilt sturen in te vullen!");
                return;
            }

            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient != null)
            {
                if (!TargetClient.GetHabbo().GetBadgeComponent().HasBadge(Params[2]))
                {
                    TargetClient.GetHabbo().GetBadgeComponent().GiveBadge(Params[2], true, TargetClient);
                    if (TargetClient.GetHabbo().Id != Session.GetHabbo().Id)
                        TargetClient.SendMessage(new RoomNotificationComposer("Hotel Management",
                     "Wow, leuk nieuws! \n\nJe hebt een badge ontvangen van het Hotel Management." + "\n\nDe badge (<b>" + " " + Params[2] + "</b>) is gegeven door <b>" + " " + Session.GetHabbo().Username + "</b>!"
                     + "\n\nVeel plezier ermee!",
                     "badges", "Bekijk de badge!", "event:inventory/open/badges"));

                    Session.SendWhisper("Je hebt succesvol de badge (" + Params[2] + ") gegeven aan " + Params[1] + ".");
                }
                else
                    Session.SendWhisper("Oeps! Deze gebruiker heeft de badge (" + Params[2] + ") al!");
                return;
            }
            else
            {
                Session.SendWhisper("Oeps! De gebruiker " + Params[1] + " bestaat niet!");
                return;

            }
         }
    }
}

