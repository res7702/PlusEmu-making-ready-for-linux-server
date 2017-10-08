using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.HabboHotel.Items.Interactor
{
    class InteractorJukebox : IFurniInteractor
    {

        public void BeforeRoomLoad(Item item)
        {
            if (item.GetRoom().GetTraxManager().IsPlaying)
                item.ExtraData = "1";
            else
                item.ExtraData = "0";

            item.UpdateState();
        }

        public void OnPlace(GameClient Session, Item Item)
        {
            bool flag = Item.ExtraData == "1";
            if (flag)
            {
                bool flag2 = !Item.GetRoom().GetTraxManager().IsPlaying;
                if (flag2)
                {
                    Item.GetRoom().GetTraxManager().PlayPlaylist();
                }
            }
        }

        public void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
            Item.UpdateState();
        }

        public void OnWiredTrigger(Item Item)
        {
            bool isPlaying = Item.GetRoom().GetTraxManager().IsPlaying;
            if (isPlaying)
            {
                Item.GetRoom().GetTraxManager().StopPlayList();
            }
            else
            {
                Item.GetRoom().GetTraxManager().PlayPlaylist();
            }
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            Room room = Item.GetRoom();
            bool flag = Request == 0 || Request == 1;
            if (flag)
            {
                room.GetTraxManager().TriggerPlaylistState();
            }
        }
    }
}