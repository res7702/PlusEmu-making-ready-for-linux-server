using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class KoopKamerCommand : IChatCommand
    {
        public string Description
        {
            get { return "Koop een kamer over van een andere gebruiker."; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string PermissionRequired
        {
            get { return "command_koop_kamer"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Room _Room = Session.GetHabbo().CurrentRoom;
            RoomUser RoomOwner = _Room.GetRoomUserManager().GetRoomUserByHabbo(_Room.OwnerName);
            if (_Room == null)
            {
                return;
            }
            if (_Room.OwnerName == Session.GetHabbo().Username)
            {
                Session.SendNotification("Oeps! Deze kamer is al van jou.");
                return;
            }

            if (!Room.ForSale)
            {
                Session.SendNotification("Oeps! Deze kamer is niet voor de verkoop.");
                return;
            }

            if (Session.GetHabbo().Duckets < _Room.SalePrice)
            {
                Session.SendNotification("Oeps! Je hebt niet genoeg Duckets om deze kamer over te kopen.");
                return;
            }

            if (RoomOwner == null || RoomOwner.GetClient() == null)
            {
                Session.SendNotification("Oeps! Er is een onbekende fout opgetreden.");
                _Room.ForSale = false;
                _Room.SalePrice = 0;
                return;
            }
            GameClient Owner = RoomOwner.GetClient();

            
            Owner.GetHabbo().Duckets += _Room.SalePrice;
            Owner.SendMessage(new HabboActivityPointNotificationComposer(Owner.GetHabbo().Duckets, _Room.SalePrice));
            Session.GetHabbo().Duckets -= _Room.SalePrice;
            Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, _Room.SalePrice));

            _Room.OwnerName = Session.GetHabbo().Username;
            _Room.OwnerId = (int)Session.GetHabbo().Id;
            _Room.RoomData.OwnerName = Session.GetHabbo().Username;
            _Room.RoomData.OwnerId = (int)Session.GetHabbo().Id;
            int RoomId = _Room.RoomId;



            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE rooms SET owner='" + Session.GetHabbo().Id + "' WHERE id='" + Room.RoomId + "' LIMIT 1");
                dbClient.RunQuery("UPDATE items SET user_id='" + Session.GetHabbo().Id + "' WHERE room_id='" + Room.RoomId + "'");
            }
            Session.GetHabbo().UsersRooms.Add(_Room.RoomData);
            Owner.GetHabbo().UsersRooms.Remove(_Room.RoomData);
            QuasarEnvironment.GetGame().GetRoomManager().UnloadRoom(_Room);

            RoomData Data = QuasarEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            Session.GetHabbo().PrepareRoom(Session.GetHabbo().CurrentRoom.RoomId, "");

        }
    }
}
