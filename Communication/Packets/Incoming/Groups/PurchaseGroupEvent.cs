using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Groups;
using Quasar.Communication.Packets.Outgoing.Groups;
using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.Communication.Packets.Outgoing.Rooms.Session;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Communication.Packets.Outgoing.Moderation;

namespace Quasar.Communication.Packets.Incoming.Groups
{
    class PurchaseGroupEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().Credits < QuasarStaticGameSettings.GroupPurchaseAmount)
            {
                Session.SendMessage(new BroadcastMessageAlertComposer("Oeps! De groep kost " + QuasarStaticGameSettings.GroupPurchaseAmount + " credits! Jij hebt helaas maar " + Session.GetHabbo().Credits + " credits!"));
                return;
            }
            else
            {
                Session.GetHabbo().Credits -= QuasarStaticGameSettings.GroupPurchaseAmount;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }
            string word;
            string Name = Packet.PopString();
            Name = QuasarEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Name, out word) ? "Spam" : Name;
            string Description = Packet.PopString();
            Description = QuasarEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Description, out word) ? "Spam" : Description;
            int RoomId = Packet.PopInt();
            int Colour1 = Packet.PopInt();
            int Colour2 = Packet.PopInt();
            int groupID3 = Packet.PopInt();
            int groupID4 = Packet.PopInt();
            int groupID5 = Packet.PopInt();
            int groupID6 = Packet.PopInt();
            int groupID7 = Packet.PopInt();
            int groupID8 = Packet.PopInt();
            int groupID9 = Packet.PopInt();
            int groupID10 = Packet.PopInt();
            int groupID11 = Packet.PopInt();
            int groupID12 = Packet.PopInt();
            int groupID13 = Packet.PopInt();
            int groupID14 = Packet.PopInt();
            int groupID15 = Packet.PopInt();
            int groupID16 = Packet.PopInt();
            int groupID17 = Packet.PopInt();
            int groupID18 = Packet.PopInt();

            RoomData Room = QuasarEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (Room == null || Room.OwnerId != Session.GetHabbo().Id || Room.Group != null)
                return;

            string Base = "b" + ((groupID4 < 10) ? "0" + groupID4.ToString() : groupID4.ToString()) + ((groupID5 < 10) ? "0" + groupID5.ToString() : groupID5.ToString()) + groupID6;
            string Symbol1 = "s" + ((groupID7 < 10) ? "0" + groupID7.ToString() : groupID7.ToString()) + ((groupID8 < 10) ? "0" + groupID8.ToString() : groupID8.ToString()) + groupID9;
            string Symbol2 = "s" + ((groupID10 < 10) ? "0" + groupID10.ToString() : groupID10.ToString()) + ((groupID11 < 10) ? "0" + groupID11.ToString() : groupID11.ToString()) + groupID12;
            string Symbol3 = "s" + ((groupID13 < 10) ? "0" + groupID13.ToString() : groupID13.ToString()) + ((groupID14 < 10) ? "0" + groupID14.ToString() : groupID14.ToString()) + groupID15;
            string Symbol4 = "s" + ((groupID16 < 10) ? "0" + groupID16.ToString() : groupID16.ToString()) + ((groupID17 < 10) ? "0" + groupID17.ToString() : groupID17.ToString()) + groupID18;

            Symbol1 = QuasarEnvironment.GetGame().GetGroupManager().CheckActiveSymbol(Symbol1);
            Symbol2 = QuasarEnvironment.GetGame().GetGroupManager().CheckActiveSymbol(Symbol2);
            Symbol3 = QuasarEnvironment.GetGame().GetGroupManager().CheckActiveSymbol(Symbol3);
            Symbol4 = QuasarEnvironment.GetGame().GetGroupManager().CheckActiveSymbol(Symbol4);

            string Badge = Base + Symbol1 + Symbol2 + Symbol3 + Symbol4;

            Group Group = null;
            if (!QuasarEnvironment.GetGame().GetGroupManager().TryCreateGroup(Session.GetHabbo(), Name, Description, RoomId, Badge, Colour1, Colour2, out Group))
            {
                Session.SendNotification("Oeps! Je groep kon niet worden aangemaakt. Meld dit even bij een medewerker.");
                return;
            }

            Session.SendMessage(new PurchaseOKComposer());

            Room.Group = Group;

            if (Session.GetHabbo().CurrentRoomId != Room.Id)
                Session.SendMessage(new RoomForwardComposer(Room.Id));

            Session.SendMessage(new NewGroupInfoComposer(RoomId, Group.Id));
            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_ExploreMakeGroup", 1);

        }
    }
}