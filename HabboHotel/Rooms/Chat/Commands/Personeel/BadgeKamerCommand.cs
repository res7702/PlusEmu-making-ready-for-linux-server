using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.HabboHotel.Rooms;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class BadgeKamerCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_kamerbadge"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Geef iedereen in de kamer een badge."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je bent vergeten een badge code in te vullen.");
                return;
            }

            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                    continue;

                if (!User.GetClient().GetHabbo().GetBadgeComponent().HasBadge(Params[1]))
                {
                    User.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(Params[1], true, User.GetClient());
                    User.GetClient().SendMessage(new RoomNotificationComposer("Hotel Management",
                     "Wow, leuk nieuws! \n\nJe hebt een badge ontvangen van het Hotel Management." + "\n\nDe badge (<b>" + " " + Params[2] + "</b>) is gegeven door <b>" + " " + Session.GetHabbo().Username + "</b>!"
                     + "\n\nVeel plezier ermee!",
                     "badges", "Bekijk de badge!", "event:inventory/open/badges"));
                }
                else
                    User.GetClient().SendMessage(new RoomNotificationComposer("Hotel Management",
                     "Wow, leuk nieuws! \n\nJe hebt een badge ontvangen van het Hotel Management." + "\n\nDe badge (<b>" + " " + Params[1] + "</b>) is gegeven door <b>" + " " + Session.GetHabbo().Username + "</b>!" +
                     "\n\nOpmerking: Je had de gegeven badge al in je bezit.</b>",
                     "badges", "Bekijk de badge!", "event:inventory/open/badges"));
            }

            Session.SendWhisper("Je hebt succesvol de badge  " + Params[2] + " gegeven aan iedereen in de kamer!");
        }
    }
}
