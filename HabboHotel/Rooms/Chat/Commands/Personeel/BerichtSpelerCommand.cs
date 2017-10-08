using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class BerichtSpelerCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_berichtspeler"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Stuur een bericht naar een gebruiker"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je bent vergeten een gebruikersnaam in te vullen.", 34);
                return;
            }

            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Oeps! Deze gebruiker bestaat niet of is op dit moment offline.", 34);
                return;
            }

            if (TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("Oeps! Deze gebruiker bestaat niet of is op dit moment offline.", 34);
                return;
            }

            string Message = CommandManager.MergeParams(Params, 2);

            TargetClient.SendMessage(new RoomCustomizedAlertComposer("Privé bericht van een medewerker:\n\n" + Message));
            Session.SendWhisper("Het verstuurde bericht is door " + TargetClient.GetHabbo().Username + " ontvangen.");

        }
    }
}
