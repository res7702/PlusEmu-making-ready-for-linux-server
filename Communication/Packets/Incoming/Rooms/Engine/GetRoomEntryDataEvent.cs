using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Groups;
using Quasar.HabboHotel.Items.Wired;

using Quasar.Communication.Packets.Outgoing.Rooms.Engine;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.Communication.Packets.Outgoing.Users;
using Quasar.Communication.Packets.Outgoing.Navigator;
using Quasar.Communication.Packets.Outgoing.Rooms.Poll;
using Quasar.HabboHotel.Quests;
using Quasar.Database.Interfaces;

using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using System.Data;
using Quasar.HabboHotel.Rooms.AI.Speech;
using Quasar.HabboHotel.Rooms.AI;

namespace Quasar.Communication.Packets.Incoming.Rooms.Engine
{
    class GetRoomEntryDataEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            if (Session.GetHabbo().InRoom)
            {
                Room OldRoom;

                if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out OldRoom))
                    return;

                if (OldRoom.GetRoomUserManager() != null)
                    OldRoom.GetRoomUserManager().RemoveUserFromRoom(Session, false, false);
            }

            if (!Room.GetRoomUserManager().AddAvatarToRoom(Session))
            {
                Room.GetRoomUserManager().RemoveUserFromRoom(Session, false, false);
                return;//TODO: Remove?
            }

            Room.SendObjects(Session);

            //Status updating for messenger, do later as buggy.

            try
            {
                if (Session.GetHabbo().GetMessenger() != null)
                    Session.GetHabbo().GetMessenger().OnStatusChanged(true);
            }
            catch { }

            if (Session.GetHabbo().GetStats().QuestID > 0)
                QuasarEnvironment.GetGame().GetQuestManager().QuestReminder(Session, Session.GetHabbo().GetStats().QuestID);

            Session.SendMessage(new RoomEntryInfoComposer(Room.RoomId, Room.CheckRights(Session, true)));
            Session.SendMessage(new RoomVisualizationSettingsComposer(Room.WallThickness, Room.FloorThickness, QuasarEnvironment.EnumToBool(Room.Hidewall.ToString())));

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);

            if (ThisUser != null && Session.GetHabbo().PetId == 0)
                Room.SendMessage(new UserChangeComposer(ThisUser, false));

            Session.SendMessage(new RoomEventComposer(Room.RoomData, Room.RoomData.Promotion));

			
            if (Room.poolQuestion != string.Empty)
            {
                Session.SendMessage(new QuickPollMessageComposer(Room.poolQuestion));

                if (Room.yesPoolAnswers.Contains(Session.GetHabbo().Id) || Room.noPoolAnswers.Contains(Session.GetHabbo().Id))
                {
                    Session.SendMessage(new QuickPollResultsMessageComposer(Room.yesPoolAnswers.Count, Room.noPoolAnswers.Count));
                }
            }


            if (Room.GetWired() != null)
                Room.GetWired().TriggerEvent(WiredBoxType.TriggerRoomEnter, Session.GetHabbo());

            if (Room.ForSale && Room.SalePrice > 0 && (Room.GetRoomUserManager().GetRoomUserByHabbo(Room.OwnerName) != null))
            {
                Session.SendWhisper("Deze kamer staat te koop voor " + Room.SalePrice + " Duckets. Gebruik de command :koopkamer als je de kamer wilt overnemen.");
            }
            else if (Room.ForSale && Room.GetRoomUserManager().GetRoomUserByHabbo(Room.OwnerName) == null)
            {
                foreach (RoomUser _User in Room.GetRoomUserManager().GetRoomUsers())
                {
                    if (_User.GetClient() != null && _User.GetClient().GetHabbo() != null && _User.GetClient().GetHabbo().Id != Session.GetHabbo().Id)
                    {
                        _User.GetClient().SendWhisper("Oeps! Deze kamer is toevallig net uit de verkoop gehaald.");
                    }
                }
                Room.ForSale = false;
                Room.SalePrice = 0;
            }

            if (QuasarEnvironment.GetUnixTimestamp() < Session.GetHabbo().FloodTime && Session.GetHabbo().FloodTime != 0)
                Session.SendMessage(new FloodControlComposer((int)Session.GetHabbo().FloodTime - (int)QuasarEnvironment.GetUnixTimestamp()));


            QuasarEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_VISIT);

        }
    }
}