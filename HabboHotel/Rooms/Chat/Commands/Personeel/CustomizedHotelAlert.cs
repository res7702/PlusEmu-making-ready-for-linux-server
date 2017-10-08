using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Communication.Packets.Outgoing.Moderation;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class CustomizedHotelAlert : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_hotelalert"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Stuur iedereen in het hotel een bericht."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je hebt geen bericht ingevoerd.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new RoomCustomizedAlertComposer("\n" + Message + "\n\n - "+ Session.GetHabbo().Username +""));
            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new MassEventComposer(Message));
            return;
        }
    }
}
