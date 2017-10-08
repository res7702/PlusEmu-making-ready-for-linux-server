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


namespace Quasar.HabboHotel.Rooms.Chat.Commands.Events
{
    internal class HelpCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "command_help";
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
                return "Vraag een Ambassadeur om hulp.";
            }
        }
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            long nowTime = QuasarEnvironment.CurrentTimeMillis();
            long timeBetween = nowTime - Session.GetHabbo()._lastTimeUsedHelpCommand;
            if (timeBetween < 3600) // 1 uur
            {
                Session.SendWhisper("Wacht minstens 60 minuten met het nogmaals gebruik maken van de Habbis Helper tool. (Of gebruik het Ticket Systeem).");
                return;
            }

            Session.GetHabbo()._lastTimeUsedHelpCommand = nowTime;
            string Request = CommandManager.MergeParams(Params, 1);

            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je bent vergeten om het probleem te omschrijven.");
            }

            else
            {

                QuasarEnvironment.GetGame().GetClientManager().GuideAlert(new RoomNotificationComposer("Helper Systeem",
                     "De gebruiker <b>" + Session.GetHabbo().Username + "</b> heeft een vraag of probleem!<br><br><b>Oproep omschrijving</b><br>"
                     + Request + "</b>", "alert_helper", "Bezoek " + Session.GetHabbo().Username + "", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));

                Session.SendWhisper("Je verzoek om hulp is verzonden! Deze wordt z.s.m door een Habbis Helper behandeld.");
            }
        }
    }
}



