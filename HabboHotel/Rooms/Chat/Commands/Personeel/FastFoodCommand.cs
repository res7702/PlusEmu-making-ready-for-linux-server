using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;



namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    internal class FastFoodCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "command_fastfood";
            }
        }
        public string Parameters
        {
            get { return ""; }
        }
        public string Description
        {
            get
            {
                return "Stuur een bericht voor een FastFood Evenement.";
            }
        }
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);
            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("FastFood Evenement",
             "<b>Beste Habbis,</b>\n\nOmdat we FastFood wat drukker willen maken sturen we dit bericht!\n\nWees de beste ober van Habbis, lever al het drinken en eten netjes bij de klanten..zonder te kliederen! Gebruik je power-ups om het spel te winnen!\n\n(Het verslaan van een medewerker wordt beloond met Diamanten!)<b>\n\n- Habbis Management</b>", "fastfood", "Doe mee!", "event:games/play/baseJump"));

        }
    }
}

