using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Communication.Packets.Outgoing.Moderation;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class StaffBerichtCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_staffbericht"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Stuur een bericht naar al de online personeels leden."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je bent vergeten een bericht in te voeren.", 34);
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            QuasarEnvironment.GetGame().GetClientManager().StaffAlert(new BroadcastMessageAlertComposer("Personeels bericht:\r\r" + Message + "\r\n" + "- " + Session.GetHabbo().Username));
            return;
        }
    }
}
