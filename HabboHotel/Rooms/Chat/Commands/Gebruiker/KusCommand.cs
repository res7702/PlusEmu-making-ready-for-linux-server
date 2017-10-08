using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Engine;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Quests;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class KusCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_kus"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Geef een andere speler een kusje."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
           if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je bent vergeten een gebruikersnaam in te voeren.", 34);
                return;
            }

            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Oeps! Deze Habbis kon niet worden gevonden.", 34);
                return;
            }

            if (Session == TargetClient)
            {
                Session.SendWhisper("Oeps! Je kan jezelf geen kus geven.", 34);
                return;
            }
            RoomUser TargetUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("Oeps! Deze Habbis is niet in de kamer aanwezig.", 34);
                return;
            }

            long nowTime = QuasarEnvironment.CurrentTimeMillis();
            long timeBetween = nowTime - Session.GetHabbo()._lastTimeUsedHelpCommand;
            if (timeBetween < 600000) // Timestamp: 600000(ms) gelijk aan 10 minuten.
            {
                Session.SendWhisper("Wacht minstens 10 minuten met het kussen van een volgende Habbis.");
                return;
            }

            Session.GetHabbo()._lastTimeUsedHelpCommand = nowTime;
            string Request = CommandManager.MergeParams(Params, 1);

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser ThisUser2 = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (!((Math.Abs(TargetUser.X - ThisUser.X) >= 2) || (Math.Abs(TargetUser.Y - ThisUser.Y) >= 2)))
            {
                Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*Geeft een kusje aan " + Params[1] + "*", 0, 16));
                Session.SendWhisper("Gebruik :enable 0 om het effect uit te zetten.", 34);
                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_KusHabbis", 1);
                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(TargetClient, "ACH_KusOntvangen", 1);
                Session.GetHabbo().Effects().ApplyEffect(9);

            }
            else
            {
                Session.SendWhisper("Oeps! Deze Habbis " + Params[1] + " is te ver weg!", 34);
            }
        }
    }
}