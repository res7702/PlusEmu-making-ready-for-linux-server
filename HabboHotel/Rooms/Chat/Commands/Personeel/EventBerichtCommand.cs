using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Quasar.Communication.Packets.Outgoing.Users;
using Quasar.Communication.Packets.Outgoing.Notifications;


using Quasar.Communication.Packets.Outgoing.Handshake;
using Quasar.Communication.Packets.Outgoing.Quests;
using Quasar.HabboHotel.Items;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.HabboHotel.Quests;
using Quasar.HabboHotel.Rooms;
using System.Threading;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Avatar;
using Quasar.Communication.Packets.Outgoing.Pets;
using Quasar.Communication.Packets.Outgoing.Messenger;
using Quasar.HabboHotel.Users.Messenger;
using Quasar.Communication.Packets.Outgoing.Rooms.Polls;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.Communication.Packets.Outgoing.Availability;
using Quasar.Communication.Packets.Outgoing;
using Quasar.Communication.Packets.Outgoing.Rooms.Engine;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Events
{
    internal class EventBerichtCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "command_eventbericht";
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
                return "Stuur een bericht wanneer een evenement begint.";
            }
        }

        
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);
          
            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("Tijd voor een event!",
                 "\nEr wordt een event gehouden door:<b> " + "  " 
                 + Session.GetHabbo().Username +
                 "</b>\n\nWil jij meedoen met dit event?\nWaar wacht je op? Klik op 'Ga naar het event!'"
                 +
                 "<b>\n\nWat is een event?</b>\nEen event is een willekeurig spel geselecteerd door een stafflid. Tijdens het event speel je tegen andere Habbis om kans te maken op coole prijzen!\n\n<b>Event details: </b>\n"
                 + Message + "", "\nevent", "Ga naar het event!", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));

        }
    }
}

