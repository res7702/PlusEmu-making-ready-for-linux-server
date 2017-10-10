using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Sound;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing;
using Quasar.HabboHotel.Rooms.TraxMachine;

namespace Quasar.Communication.Packets.Incoming.Sound
{
    class GetJukeboxDiscsDataEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var songslen = Packet.PopInt();
            var Songs = new List<TraxMusicData>();
            while (songslen-- > 0)
            {
                var id = Packet.PopInt();
                var music = TraxSoundManager.GetMusic(id);
                if (music != null)
                    Songs.Add(music);
            }
            if (Session.GetHabbo().CurrentRoom != null)
                Session.SendMessage(new SetJukeboxSongMusicDataComposer(Songs));
           
        }
    }
}
