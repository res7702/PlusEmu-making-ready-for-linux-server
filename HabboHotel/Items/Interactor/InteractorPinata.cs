using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.HabboHotel.Items.Interactor
{
    class InteractorPinata : IFurniInteractor
    {
        public void OnPlace(GameClients.GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
        }

        public void OnRemove(GameClients.GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClients.GameClient Session, Item Item, int Request, bool HasRights)
        {
            Room Room;
            if((Room = Session?.GetHabbo()?.CurrentRoom) != null || Item != null || Item.ExtraData == "1")
            {
                return;
            }

            if(Session.GetHabbo().Id != Item.UserID)
            {
                Session.SendWhisper("Oeps! Dit is niet jouw Pinata - Je kunt dit niet openen!");
                return;
            }

            RoomUser Actor;
            if ((Actor = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id)) == null ||
                Gamemap.TileDistance(Actor.X, Actor.Y, Item.GetX, Item.GetY) > 1)
            {
                return;
            }

            QuasarEnvironment.GetGame().GetPinataManager().ReceiveCrackableReward(Actor, Room, Item);

        }

        public void OnWiredTrigger(Item Item)
        {
           
        }
    }
}
