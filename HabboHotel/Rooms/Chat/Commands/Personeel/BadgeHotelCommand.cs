using System.Linq;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

using Quasar.HabboHotel.GameClients;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class BadgeHotelCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_mass_badge"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Geef een badge aan het gehele hotel."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Je bent vergeten een badge code in te vullen.");
                return;
            }

            foreach (GameClient Client in QuasarEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().Username == Session.GetHabbo().Username)
                    continue;
                if (!Client.GetHabbo().GetBadgeComponent().HasBadge(Params[1]))
                {
                    Client.GetHabbo().GetBadgeComponent().GiveBadge(Params[1], true, Client);

                    Client.SendMessage(new RoomNotificationComposer("Hotel Management",
                     "Wow, leuk nieuws! \n\nJe hebt een badge ontvangen van het Hotel Management." + "\n\nDe badge (<b>" + " " + Params[1] + "</b>) is gegeven door <b>" + " " + Session.GetHabbo().Username + "</b>!",
                     "badges", "Bekijk de badge!", "event:inventory/open/badges"));
                }
                else
               
                    Client.SendMessage(new RoomNotificationComposer("Hotel Management",
                     "Wow, leuk nieuws! \n\nJe hebt een badge ontvangen van het Hotel Management." + "\n\nDe badge (<b>" + " " + Params[1] + "</b>) is gegeven door <b>" + " " + Session.GetHabbo().Username + "</b>!" +
                     "\n\nOpmerking: Je had de gegeven badge al in je bezit.</b>",
                     "badges", "Bekijk de badge!", "event:inventory/open/badges"));
                
            }

            Session.SendWhisper("Je hebt iedereen succesvol de badge " + Params[1] + " gegeven!");
        }
    }
}
