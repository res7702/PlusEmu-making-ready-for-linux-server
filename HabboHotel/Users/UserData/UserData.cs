using System.Collections;
using System.Collections.Generic;
using Quasar.HabboHotel.Achievements;
using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Rooms.AI;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users.Badges;
using Quasar.HabboHotel.Users.Inventory;
using Quasar.HabboHotel.Users.Messenger;
using Quasar.HabboHotel.Users.Relationships;
using System.Collections.Concurrent;
using Quasar.HabboHotel.Club;

namespace Quasar.HabboHotel.Users.UserDataManagement
{
    public class UserData
    {
        public int userID;
        public Habbo user;

        public Dictionary<int, Relationship> Relations;
        public ConcurrentDictionary<string, UserAchievement> achievements;
        public List<Badge> badges;
        public List<int> favouritedRooms;
        public Dictionary<int, MessengerRequest> requests;
        public Dictionary<int, MessengerBuddy> friends;
        public List<int> ignores;
        public Dictionary<int, int> quests;
        public Dictionary<int, UserTalent> Talents;
        public List<RoomData> rooms;
        public Dictionary<string, Subscription> subscriptions;

        public UserData(int userID, ConcurrentDictionary<string, UserAchievement> achievements, List<int> favouritedRooms, List<int> ignores,
            List<Badge> badges, Dictionary<int, MessengerBuddy> friends, Dictionary<int, MessengerRequest> requests, List<RoomData> rooms, Dictionary<int, int> quests, Habbo user,
            Dictionary<int, Relationship> Relations, Dictionary<int, UserTalent> talents, Dictionary<string, Subscription> subscriptions)
        {
            this.userID = userID;
            this.achievements = achievements;
            this.favouritedRooms = favouritedRooms;
            this.ignores = ignores;
            this.badges = badges;
            this.friends = friends;
            this.requests = requests;
            this.rooms = rooms;
            this.quests = quests;
            this.user = user;
            this.Talents = talents;
            this.Relations = Relations;
            this.subscriptions = subscriptions;
        }
    }
}