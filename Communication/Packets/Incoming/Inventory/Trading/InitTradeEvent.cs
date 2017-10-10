using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Utilities;
using Quasar.HabboHotel.Rooms;
using Quasar.Communication.Packets.Outgoing.Inventory.Trading;

using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Quests;

namespace Quasar.Communication.Packets.Incoming.Inventory.Trading
{
    class InitTradeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CanTradeInRoom)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Session.GetHabbo().TradingLockExpiry > 0)
            {
                if (Session.GetHabbo().TradingLockExpiry > QuasarEnvironment.GetUnixTimestamp())
                {
                    Session.SendNotification("Oeps! Je kunt niet ruilen want je hebt een ruil blokkering.");
                    return;
                }
                else
                {
                    Session.GetHabbo().TradingLockExpiry = 0;
                    Session.SendNotification("De ruilban is opgeheven, je kan weer ruilen!.");

                    using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '0' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }
                }
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByVirtualId(Packet.PopInt());

            if (TargetUser == null || TargetUser.GetClient() == null || TargetUser.GetClient().GetHabbo() == null)
                return;

            if (TargetUser.IsTrading)
            {
                Session.SendMessage(new TradingErrorComposer(8, TargetUser.GetUsername()));
                return;
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("room_trade_override"))
            {
                if (Room.TradeSettings == 1 && Room.OwnerId != Session.GetHabbo().Id)//Owner only.
                {
                    Session.SendMessage(new TradingErrorComposer(6, TargetUser.GetUsername()));
                    return;
                }
                else if (Room.TradeSettings == 0 && Room.OwnerId != Session.GetHabbo().Id)//Trading is disabled.
                {
                    Session.SendMessage(new TradingErrorComposer(6, TargetUser.GetUsername()));
                    return;
                }
            }

            if (TargetUser.GetClient().GetHabbo().TradingLockExpiry > 0)
            {
                Session.SendNotification("Oeps! Deze gebruiker heeft een verbod op ruilen!");
                return;
            }

            Room.TryStartTrade(User, TargetUser);
            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_ExploreTrade", 1);
        }
    }
}