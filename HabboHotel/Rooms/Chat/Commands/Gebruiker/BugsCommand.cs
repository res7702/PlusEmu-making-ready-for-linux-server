using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Data;

using Quasar.HabboHotel.Items;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Database.Interfaces;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class BugsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_bugs"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Fout gevonden? Meld hier de bugg!"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            Session.SendMessage(new RoomNotificationComposer("Problemen en Fouten", "<b>Problemen ervaren?</b>\n\nWe hebben een speciaal forum gemaakt om problemen en andere fouten te melden. Kies de categorie uit waar jouw probleem het beste inpast, maak een bericht aan en omschrijf zo goed mogelijk je fouten.", "alert_bugg", "Ga naar forum", "event:groupforum/1"));
        }
    }
}
