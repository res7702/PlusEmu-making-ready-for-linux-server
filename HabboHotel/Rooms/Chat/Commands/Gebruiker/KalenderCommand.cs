using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    internal class KalenderCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "command_kalender";
            }
        }
        public string Parameters
        {
            get
            {
                return "";
            }
        }
        public string Description
        {
            get
            {
                return "Open je cadeaukalender!";
            }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.SendMessage(new MassEventComposer("openView/calendar"));
            return;
         }
    }
}