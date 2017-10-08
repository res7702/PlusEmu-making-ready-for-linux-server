using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
using Quasar.HabboHotel.Rooms.Chat.Commands.Personeel;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class LeegCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_empty"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Leeg je inventaris."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendMessage(new RoomCustomizedAlertComposer("Waarschuwing: Ben je er zeker van dat je je inventaris wilt legen?\n" +
                                         "Om het te bevestigen type ':empty ja'"));
            }
            else
            {
                if (Params.Length == 2 && Params[1] == "ja")
                {
                    Session.GetHabbo().GetInventoryComponent().ClearItems();
                    Session.SendMessage(new RoomCustomizedAlertComposer("Je inventaris is geleegd!"));
                }
                else if (Params.Length == 2 && Params[1] != "ja")
                {
                    Session.SendWhisper("Om te bevestigen moet je ':empty ja' typen.");
                }
            }
        }
    }
}