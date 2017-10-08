using Quasar.Communication.Packets.Outgoing.Sound;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Rooms;
using System;
using System.Collections.Generic;

namespace Quasar.HabboHotel.Items.Interactor
{
    internal class InteractorMusicDisc : IFurniInteractor
    {
        public Item Item;

        public bool CanRemove(GameClient User, Item Item)
        {
            bool flag = Item.GetRoom().GetTraxManager().GetDiscItem(Item.Id) != null && Item.GetRoom().GetTraxManager().IsPlaying;
            bool result;
            if (flag)
            {
                User.SendNotification("Oeps! Er is wat mis gegaan - probeer het nog een keer.");
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }

        public void OnRemove(GameClient Session, Item Item)
        {
            Room room = Item.GetRoom();
            Item discItem = Item.GetRoom().GetTraxManager().GetDiscItem(Item.Id);
            bool flag = discItem != null;
            if (flag)
            {
                room.GetTraxManager().StopPlayList();
                room.GetTraxManager().RemoveDisc(Item);
            }
            List<Item> avaliableSongs = room.GetTraxManager().GetAvaliableSongs();
            avaliableSongs.Remove(Item);
            room.SendMessage(new LoadJukeboxUserMusicItemsComposer(avaliableSongs));
        }

        public void OnPlace(GameClient Session, Item Item)
        {
            Room room = Item.GetRoom();
            List<Item> avaliableSongs = room.GetTraxManager().GetAvaliableSongs();
            bool flag = !avaliableSongs.Contains(Item) && !room.GetTraxManager().Playlist.Contains(Item);
            if (flag)
            {
                avaliableSongs.Add(Item);
            }
            room.SendMessage(new LoadJukeboxUserMusicItemsComposer(avaliableSongs));
             QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_JukeboxPlaceDisc", 1);
            }

        public InteractorMusicDisc(Item item)
        {
            Item = item;
        }

        public void OnTrigger(GameClient Session, Item item, int Request, bool HasRights)
        {
        }
        public void OnWiredTrigger(Item Item)
        {
        }
    }
}
