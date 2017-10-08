using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Handshake;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class ForceerFlagmeCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_forceer_flagme"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Verander de naam van een andere gebruiker."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je bent vergeten een gebruiker in te vullen.", 34);
                return;
            }

            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Oeps! De gebruiker kan niet worden gevonden of is offline.", 34);
                return;
            }

            if (TargetClient.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.", 34);
                return;
            }
            else
            {
                TargetClient.GetHabbo().LastNameChange = 0;
                TargetClient.GetHabbo().ChangingName = true;
                TargetClient.SendNotification("Het personeel van Habbis Hotel heeft uw naam veranderd.");
                TargetClient.SendMessage(new UserObjectComposer(TargetClient.GetHabbo()));
            }

        }
    }
}
