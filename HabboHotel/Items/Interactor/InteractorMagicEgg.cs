using Quasar.HabboHotel.Catalog;
using Quasar.HabboHotel.Rooms;
using System;

namespace Quasar.HabboHotel.Items.Interactor
{
    class InteractorMagicEgg : IFurniInteractor
    {
        public void OnPlace(GameClients.GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClients.GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClients.GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Session == null || Session.GetHabbo() == null || Item == null)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser Actor = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (Actor == null)
                return;

           
            var tick = int.Parse(Item.ExtraData);

            if (Actor.CurrentEffect == 186)
            {
                if (Gamemap.TileDistance(Actor.X, Actor.Y, Item.GetX, Item.GetY) < 2)
                {
                    tick++;
                    Item.ExtraData = tick.ToString();
                   
                    if (Item.GetBaseItem().Id == 3390 && tick == 5)
                    {
                        Item.UpdateState(true, true);
                        Console.WriteLine("Weer vijf klik gedaan");
                    }
                    else
                    {
                        Console.WriteLine("te");
                    }
                        int X = Item.GetX, Y = Item.GetY, Rot = Item.Rotation;
                        Double Z = Item.GetZ;
                        if (tick == 20)
                        {
                            QuasarEnvironment.GetGame().GetPinataManager().ReceiveCrackableReward(Actor, Room, Item);
                            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Actor.GetClient(), "ACH_PinataBreaker", 1);
                        }
                    }
                }
            }
        

        public void OnWiredTrigger(Item Item)
        {

        }
    }
}
