using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Session;

namespace Quasar.HabboHotel.Items.Interactor
{
    public class InteractorHabboWheel : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "-1";
            Item.RequestUpdate(10, true);
        }

        public void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "-1";
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (!HasRights)
            {
                return;
            }

            if (Item.ExtraData != "-1")
            {
                Item.ExtraData = "-1";
                Item.UpdateState();
                Item.RequestUpdate(10, true);
                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_HabbisWheel", 1, false);
            }
        }

        public void OnWiredTrigger(Item Item)
        {
            if (Item.ExtraData != "-1")
            {
                Item.ExtraData = "-1";
                Item.UpdateState();
                Item.RequestUpdate(10, true);
            }
       }
    }
}