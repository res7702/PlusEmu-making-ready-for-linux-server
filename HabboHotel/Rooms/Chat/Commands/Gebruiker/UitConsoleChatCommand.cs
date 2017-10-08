using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class UitConsoleChatCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_uit_consolechat"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Het ontvangen van console berichten (aan/uit)."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowConsoleMessages = !Session.GetHabbo().AllowConsoleMessages;
            Session.SendWhisper("Je " + (Session.GetHabbo().AllowConsoleMessages == true ? "ontvangt nu weer" : "ontvangt nu geen") + " console berichten.");
        }
    }
}