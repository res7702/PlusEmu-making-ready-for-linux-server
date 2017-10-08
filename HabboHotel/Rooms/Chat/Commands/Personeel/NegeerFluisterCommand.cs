using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class NegeerFluisterCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_negeerfluister"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Negeer al het fluister in de kamer."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().IgnorePublicWhispers = !Session.GetHabbo().IgnorePublicWhispers;
            Session.SendWhisper("Je negeert " + (Session.GetHabbo().IgnorePublicWhispers ? "nu" : "nu niet meer") + " al het fluister in de kamer.");
        }
    }
}
