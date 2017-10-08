using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class UitDiagonaalCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_uit_diagonaal"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Zet schuin lopen in je kamer (aan/uit)."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oeps! Alleen de kamer eigenaar heeft de bevoegdheid om deze actie uit te voeren.");
                return;
            }

            Room.GetGameMap().DiagonalEnabled = !Room.GetGameMap().DiagonalEnabled;
            Session.SendWhisper("Gelukt! Diagonaal lopen is nu gewijzigd.");
        }
    }
}
