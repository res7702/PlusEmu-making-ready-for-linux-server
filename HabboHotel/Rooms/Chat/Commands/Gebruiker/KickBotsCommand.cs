using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms.AI;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.GameClients;

using Quasar.HabboHotel.Users.Inventory.Bots;
using Quasar.Communication.Packets.Outgoing.Inventory.Bots;

using Quasar.Database.Interfaces;


namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class KickRobotsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_kick_bots"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Stuur al de bots in je kamer weg."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.", 34);
                return;
            }

            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (User == null || User.IsPet || !User.IsBot)
                    continue;

                RoomUser BotUser = null;
                if (!Room.GetRoomUserManager().TryGetBot(User.BotData.Id, out BotUser))
                    return;

                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `bots` SET `room_id` = '0' WHERE `id` = @id LIMIT 1");
                    dbClient.AddParameter("id", User.BotData.Id);
                    dbClient.RunQuery();
                }

                Session.GetHabbo().GetInventoryComponent().TryAddBot(new Bot(Convert.ToInt32(BotUser.BotData.Id), Convert.ToInt32(BotUser.BotData.ownerID), BotUser.BotData.Name, BotUser.BotData.Motto, BotUser.BotData.Look, BotUser.BotData.Gender));
                Session.SendMessage(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
                Room.GetRoomUserManager().RemoveBot(BotUser.VirtualId, false);
            }

            Session.SendWhisper("Jeej! De bots zijn de kamer uitgestuurd!", 34);
        }
    }
}
