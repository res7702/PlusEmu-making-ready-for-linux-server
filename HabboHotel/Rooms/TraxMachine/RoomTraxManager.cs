using Quasar.Communication.Packets.Outgoing.Sound;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Quasar.HabboHotel.Rooms.TraxMachine
{
    public class RoomTraxManager
    {
        public int Capacity = 10;

        private DataTable dataTable;

        public Room Room
        {
            get;
            private set;
        }

        public List<Item> Playlist
        {
            get;
            private set;
        }

        public bool IsPlaying
        {
            get;
            private set;
        }

        public int StartedPlayTimestamp
        {
            get;
            private set;
        }

        public Item SelectedDiscItem
        {
            get;
            private set;
        }

        public TraxMusicData AnteriorMusic
        {
            get;
            private set;
        }

        public Item AnteriorItem
        {
            get;
            private set;
        }

        public int TimestampSinceStarted
        {
            get
            {
                return checked((int)QuasarEnvironment.GetUnixTimestamp() - this.StartedPlayTimestamp);
            }
        }

        public int TotalPlayListLength
        {
            get
            {
                int num = 0;
                checked
                {
                    foreach (Item current in this.Playlist)
                    {
                        TraxMusicData music = TraxSoundManager.GetMusic(current.ExtradataInt);
                        bool flag = music == null;
                        if (!flag)
                        {
                            num += music.Length;
                        }
                    }
                    return num;
                }
            }
        }

        public Item ActualSongData
        {
            get
            {
                IEnumerable<KeyValuePair<int, Item>> enumerable = this.GetPlayLine().Reverse<KeyValuePair<int, Item>>();
                int timestampSinceStarted = this.TimestampSinceStarted;
                bool flag = timestampSinceStarted > this.TotalPlayListLength;
                Item result;
                if (flag)
                {
                    result = null;
                }
                else
                {
                    foreach (KeyValuePair<int, Item> current in enumerable)
                    {
                        bool flag2 = current.Key <= timestampSinceStarted;
                        if (flag2)
                        {
                            result = current.Value;
                            return result;
                        }
                    }
                    result = null;
                }
                return result;
            }
        }

        public int ActualSongTimePassed
        {
            get
            {
                Dictionary<int, Item> playLine = this.GetPlayLine();
                int num = 0;
                foreach (KeyValuePair<int, Item> current in playLine)
                {
                    bool flag = current.Value == this.ActualSongData;
                    if (flag)
                    {
                        num = current.Key;
                    }
                }
                return checked(this.TimestampSinceStarted - num);
            }
        }

        public RoomTraxManager(Room room)
        {
            this.Room = room;
            room.OnFurnisLoad += new Room.FurnitureLoad(this.Room_OnFurnisLoad);
            this.IsPlaying = false;
            this.StartedPlayTimestamp = checked((int)QuasarEnvironment.GetUnixTimestamp());
            this.Playlist = new List<Item>();
            this.SelectedDiscItem = null;
            using (IQueryAdapter queryReactor = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.RunQuery("SELECT * FROM room_jukebox_songs WHERE room_id = '" + this.Room.Id + "'");
                this.dataTable = queryReactor.getTable();
            }
        }

        private void Room_OnFurnisLoad()
        {
            foreach (DataRow dataRow in this.dataTable.Rows)
            {
                int pId = int.Parse(dataRow["item_id"].ToString());
                Item item = this.Room.GetRoomItemHandler().GetItem(pId);
                bool flag = item == null;
                if (!flag)
                {
                    this.Playlist.Add(item);
                }
            }
        }

        public void OnCycle()
        {
            bool isPlaying = this.IsPlaying;
            if (isPlaying)
            {
                bool flag = this.ActualSongData != this.SelectedDiscItem;
                if (flag)
                {
                    this.AnteriorItem = this.SelectedDiscItem;
                    this.AnteriorMusic = this.GetMusicByItem(this.SelectedDiscItem);
                    this.SelectedDiscItem = this.ActualSongData;
                    bool flag2 = this.SelectedDiscItem == null;
                    if (flag2)
                    {
                        this.StopPlayList();
                        this.PlayPlaylist();
                    }
                    this.Room.SendMessage(new SetJukeboxNowPlayingComposer(this.Room));
                }
            }
        }

        public void ClearPlayList()
        {
            bool isPlaying = this.IsPlaying;
            if (isPlaying)
            {
                this.StopPlayList();
            }
            this.Playlist.Clear();
        }

        public Dictionary<int, Item> GetPlayLine()
        {
            int num = 0;
            Dictionary<int, Item> dictionary = new Dictionary<int, Item>();
            checked
            {
                foreach (Item current in this.Playlist)
                {
                    TraxMusicData musicByItem = this.GetMusicByItem(current);
                    bool flag = musicByItem == null;
                    if (!flag)
                    {
                        dictionary.Add(num, current);
                        num += musicByItem.Length;
                    }
                }
                return dictionary;
            }
        }

        public TraxMusicData GetMusicByItem(Item item)
        {
            return (item != null) ? TraxSoundManager.GetMusic(item.ExtradataInt) : null;
        }

        public int GetMusicIndex(Item item)
        {
            checked
            {
                int result;
                for (int i = 0; i < this.Playlist.Count; i++)
                {
                    bool flag = this.Playlist[i] == item;
                    if (flag)
                    {
                        result = i;
                        return result;
                    }
                }
                result = 0;
                return result;
            }
        }

        public void PlayPlaylist()
        {
            bool flag = this.Playlist.Count == 0;
            if (!flag)
            {
                this.StartedPlayTimestamp = checked((int)QuasarEnvironment.GetUnixTimestamp());
                this.SelectedDiscItem = null;
                this.IsPlaying = true;
                this.SetJukeboxesState();
            }
        }

        public void StopPlayList()
        {
            this.IsPlaying = false;
            this.StartedPlayTimestamp = 0;
            this.SelectedDiscItem = null;
            this.Room.SendMessage(new SetJukeboxNowPlayingComposer(this.Room));
            this.SetJukeboxesState();
        }

        public void TriggerPlaylistState()
        {
            bool isPlaying = this.IsPlaying;
            if (isPlaying)
            {
                this.StopPlayList();
            }
            else
            {
                this.PlayPlaylist();
            }
        }

        public void SetJukeboxesState()
        {
            foreach (Item current in this.Room.GetRoomItemHandler().GetFloor)
            {
                bool flag = current.GetBaseItem().InteractionType == InteractionType.JUKEBOX;
                if (flag)
                {
                    current.ExtraData = (this.IsPlaying ? "1" : "0");
                    current.UpdateState();
                }
            }
        }

        public bool AddDisc(Item item)
        {
            bool flag = item.GetBaseItem().InteractionType != InteractionType.MUSIC_DISC;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                int id;
                bool flag2 = !int.TryParse(item.ExtraData, out id);
                if (flag2)
                {
                    result = false;
                }
                else
                {
                    TraxMusicData music = TraxSoundManager.GetMusic(id);
                    bool flag3 = music == null;
                    if (flag3)
                    {
                        result = false;
                    }
                    else
                    {
                        bool flag4 = this.Playlist.Contains(item);
                        if (flag4)
                        {
                            result = false;
                        }
                        else
                        {
                            bool isPlaying = this.IsPlaying;
                            if (isPlaying)
                            {
                                result = false;
                            }
                            else
                            {
                                using (IQueryAdapter queryReactor = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    queryReactor.SetQuery("INSERT INTO room_jukebox_songs (room_id, item_id) VALUES (@room, @item)");
                                    queryReactor.AddParameter("room", this.Room.Id);
                                    queryReactor.AddParameter("item", item.Id);
                                    queryReactor.RunQuery();
                                }
                                this.Playlist.Add(item);
                                this.Room.SendMessage(new SetJukeboxPlayListComposer(this.Room), false);
                                this.Room.SendMessage(new LoadJukeboxUserMusicItemsComposer(this.Room), false);
                                result = true;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public bool RemoveDisc(int id)
        {
            Item discItem = this.GetDiscItem(id);
            bool flag = discItem == null;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                bool isPlaying = this.IsPlaying;
                if (isPlaying)
                {
                    result = false;
                }
                else
                {
                    using (IQueryAdapter queryReactor = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        queryReactor.RunQuery("DELETE FROM room_jukebox_songs WHERE item_id = '" + discItem.Id + "'");
                    }
                    this.Playlist.Remove(discItem);
                    this.Room.SendMessage(new SetJukeboxPlayListComposer(this.Room), false);
                    this.Room.SendMessage(new LoadJukeboxUserMusicItemsComposer(this.Room), false);
                    result = true;
                }
            }
            return result;
        }

        public bool RemoveDisc(Item item)
        {
            return this.RemoveDisc(item.Id);
        }

        public List<Item> GetAvaliableSongs()
        {
            return (from c in this.Room.GetRoomItemHandler().GetFloor
                    where c.GetBaseItem().InteractionType == InteractionType.MUSIC_DISC && !this.Playlist.Contains(c)
                    select c).ToList<Item>();
        }

        public Item GetDiscItem(int id)
        {
            Item result;
            foreach (Item current in this.Playlist)
            {
                bool flag = current.Id == id;
                if (flag)
                {
                    result = current;
                    return result;
                }
            }
            result = null;
            return result;
        }
    }
}
