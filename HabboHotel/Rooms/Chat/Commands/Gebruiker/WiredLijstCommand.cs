using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Communication.Packets.Outgoing.Moderation;
using Quasar.Communication.Packets.Outgoing.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class WiredLijstCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_wiredlijst"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Variables die je kunt gebruiken voor je wired."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            StringBuilder List = new StringBuilder("");
            List.AppendLine("<b>Lijst van variables</b>");
            List.AppendLine("%KAMERNAAM% - Toont de huidige kamernaam.");
            List.AppendLine("%SETMAX% - Toont het aantal Habbis in je kamer.");
            List.AppendLine("%ONLINE% - Toont het aantal Habbis in het hotel.");
            List.AppendLine("%ID% - Toont het id van de Habbis die de wired gebruikt.");
            List.AppendLine("%DUCKETS% - Toont het aantal Duckets van de Habbis die de wired gebruikt.");
            List.AppendLine("%DIAMANTEN% - Toont het aantal Diamanten van de Habbis die de wired gebruikt.");
            List.AppendLine("%CREDITS% - Toont het aantal Credits van de Habbis die de wired gebruikt.");
            List.AppendLine("%RANK% - Toont de functie van de Habbis die de wired gebruikt.");
            List.AppendLine("%STEMMEN% - Toont het aantal stemmen op de kamer.");
            Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
            return;

        }
    }
}
