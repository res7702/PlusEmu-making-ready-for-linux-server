using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class UitFluisterCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_uit_fluister"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Ontvang fluister berichten van andere gebruikers (aan/uit)."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().ReceiveWhispers = !Session.GetHabbo().ReceiveWhispers;
            Session.SendWhisper("Je kan " + (Session.GetHabbo().ReceiveWhispers ? "nu weer" : "nu geen") +
                                " fluister ontvangen!");
        }
    }
}
