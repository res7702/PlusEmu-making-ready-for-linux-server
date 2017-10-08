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
using Quasar.Communication.Packets.Outgoing.Availability;
using Quasar.Communication.Packets.Outgoing;
using Quasar.Utilities;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class AboutCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_about"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Weergeeft informatie over het Hotel."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            TimeSpan Uptime = DateTime.Now - QuasarEnvironment.ServerStarted;
            int OnlineUsers = QuasarEnvironment.GetGame().GetClientManager().Count;
            int RoomCount = QuasarEnvironment.GetGame().GetRoomManager().Count;
            Session.SendMessage(new RoomNotificationComposer("Habbis Server - 2.4.8a",
                    "<font color='#000000' style='margin-left: -40px;'>" +
                    "Made with a lot of" + "<font color = '#B40486'>" + "<b> | </b>" + "</font>" + "<font color = '#000000'>" + "for the Retro Community\n\n" +
                    "<b>Hotel Statistieken</b></font>\n" +
                    "Online Leden: " + OnlineUsers + "\n" +
                    "Open Kamers: " + RoomCount + "\n\n" +
                    "<b>Emulator Actief</b>\n</font>" + Uptime.Days + " dagen, " + Uptime.Hours + " uur " + Uptime.Minutes + " minuten.\n\n" +
                    "<b>Credits</b></font>\n" +
                    "Multiple developers all over the world.\n\n" +
                    "'Every <font color='#B40486'> family</font> has a story, welcome to ours.'\n\n" +
                    "</font>", "info", "", ""));

        }
    }
}

