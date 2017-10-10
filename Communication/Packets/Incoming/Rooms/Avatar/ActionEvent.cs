using Quasar.Communication.Packets.Outgoing.Rooms.Avatar;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Quests;
using Quasar.HabboHotel.Rooms;
using Quasar.Communication.Packets.Incoming;

namespace Quasar.Communication.Packets.Incoming.Rooms.Avatar
{
    public class ActionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            int Action = Packet.PopInt();

            Room Room = null;
            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (User.DanceId > 0)
                User.DanceId = 0;

            if (Session.GetHabbo().Effects().CurrentEffect > 0)
                Room.SendMessage(new AvatarEffectComposer(User.VirtualId, 0));

            User.UnIdle();
            Room.SendMessage(new ActionComposer(User.VirtualId, Action));

            if (Action == 5) // idle
            {
                User.IsAsleep = true;
                Room.SendMessage(new SleepComposer(User, true));
            }

            QuasarEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_WAVE);
            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_ExploreWave", 1);
        
         }
    }
}