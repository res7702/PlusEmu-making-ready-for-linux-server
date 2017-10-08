using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class GenerateMapCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_generatemap"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Fix foutjes in je kamer."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oeps! Alleen de kamer eigenaar kan deze actie uitvoeren.");
                return;
            }

            Room.GetGameMap().GenerateMaps();
            Session.SendWhisper("De kamer-map is hersteld.");
        }
    }
}
