using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.GameClients;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class KickSpelerCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_kick_speler"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Stuur een kamer weg uit de kamer."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je bent vergeten een gebruiker in te vullen.");
                return;
            }

            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Oeps!De gebruiker kan niet worden gevonden of is offline.");
                return;
            }

            if (TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("Oeps! De gebruiker kan niet worden gevonden of is offline.");
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("J i j h e b t e c h t g e e n le v e n.");
                return;
            }

            if (!TargetClient.GetHabbo().InRoom)
            {
                Session.SendWhisper("Deze gebruiker is op dit moment niet in de kamer aanwezig.");
                return;
            }

            Room TargetRoom;
            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(TargetClient.GetHabbo().CurrentRoomId, out TargetRoom))
                return;

            if (Params.Length > 2)
                TargetClient.SendNotification("Je bent gekickt door een Habbis personeelslid met als reden: " + CommandManager.MergeParams(Params, 2));
            else
                TargetClient.SendNotification("Je bent gekickt door een Habbis personeelslid.");

            TargetRoom.GetRoomUserManager().RemoveUserFromRoom(TargetClient, true, false);
        }
    }
}
