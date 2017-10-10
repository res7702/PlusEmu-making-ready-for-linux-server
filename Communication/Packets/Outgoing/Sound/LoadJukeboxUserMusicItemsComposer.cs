using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;

namespace Quasar.Communication.Packets.Outgoing.Sound
{
    class LoadJukeboxUserMusicItemsComposer : ServerPacket
    {
        public LoadJukeboxUserMusicItemsComposer(Room room)
            : base(ServerPacketHeader.LoadJukeboxUserMusicItemsMessageComposer)
        {
            var songs = room.GetTraxManager().GetAvaliableSongs();

            base.WriteInteger(songs.Count);//while

            foreach (var item in songs)
            {
                base.WriteInteger(item.Id);//item id
                base.WriteInteger(item.ExtradataInt);//Song id
            }
        }

        public LoadJukeboxUserMusicItemsComposer(ICollection<Item> Items)
            : base(ServerPacketHeader.LoadJukeboxUserMusicItemsMessageComposer)
        {

            base.WriteInteger(Items.Count);//while

            foreach (var item in Items)
            {
                base.WriteInteger(item.Id);//item id
                base.WriteInteger(item.ExtradataInt);//Song id
            }
        }
    }
}