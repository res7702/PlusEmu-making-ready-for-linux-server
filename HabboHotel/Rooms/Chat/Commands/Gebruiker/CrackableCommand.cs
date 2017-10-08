using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    internal class CrackableCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "command_crackable";
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
                return "Prijzenlijst van de kraakbare Piñatas.";
            }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.SendMessage(new MassEventComposer("habbopages/crackable.txt"));
            return;
        }
    }
}