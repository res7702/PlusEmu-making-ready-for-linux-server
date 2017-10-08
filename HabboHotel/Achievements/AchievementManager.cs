using System;
using System.Linq;
using System.Collections.Generic;

using Quasar.HabboHotel.GameClients;

using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Communication.Packets.Outgoing.Inventory.Achievements;

using Quasar.Database.Interfaces;
using log4net;

using Quasar.HabboHotel.Users;
using Quasar.Communication.Packets.Outgoing.Inventory.Badges;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;


namespace Quasar.HabboHotel.Achievements
{
    public class AchievementManager
    {
        private static readonly ILog log = LogManager.GetLogger("Quasar.HabboHotel.Achievements.AchievementManager");

        public Dictionary<string, Achievement> _achievements;

        public AchievementManager()
        {
            this._achievements = new Dictionary<string, Achievement>();
            this.LoadAchievements();

            string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
            Console.WriteLine(CurrentTime + "» Hotel Achievements");
        }

        public void LoadAchievements()
        {
            AchievementLevelFactory.GetAchievementLevels(out _achievements);
        }

        public bool ProgressAchievement(GameClient Session, string AchievementGroup, int ProgressAmount, bool FromZero = false)
        {
            if (!_achievements.ContainsKey(AchievementGroup))
            {
                Session.SendNotification("Oeps! Er is een klein detailtje vergeten waardoor deze Achievement helaas nog niet werkt. Meldt dit bij één van de hotel beheerders.");
                return false;
            }

            var AchData = _achievements[AchievementGroup];

            UserAchievement UserAch = Session?.GetHabbo()?.GetOrAddAchievementData(AchievementGroup);

            if (UserAch == null || ProgressAmount <= 0)
            {
                return false;
            }

            AchievementLevel Current = null;

            if(AchData.Levels.ContainsKey(UserAch.Level))
            {
                Current = AchData.Levels[UserAch.Level];
            }

            AchievementLevel Next = (UserAch.Level >= AchData.Levels.Count) ? AchData.Levels.Values.LastOrDefault() : AchData.Levels[UserAch.Level + 1];

            if (UserAch.Level != 0 && Current.Level == Next.Level && 
                UserAch.Progress >= Current.Requirement)
            {
                return false;
            }

            UserAch.Progress += ProgressAmount;

            if (UserAch.Progress >= Next.Requirement)
            {
                UserAch.Level = Next.Level;

                if (UserAch.Level == 1)
                {
                    Session.GetHabbo().GetBadgeComponent().GiveBadge(AchievementGroup + Next.Level,
                        true, Session);
                }
                else { 
                    Session.GetHabbo().GetBadgeComponent().ReplaceBadge(AchievementGroup + Current.Level,
                        AchievementGroup + Next.Level, true);
                }

                Session.GetHabbo().Duckets += Next.RewardPixels;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Next.RewardPixels));

                Session.GetHabbo().GetStats().AchievementPoints += Next.RewardPoints;
                Session.SendMessage(new AchievementScoreComposer(Session.GetHabbo().GetStats().AchievementPoints));


                Session.SendMessage(new BadgesComposer(Session));
                Session.SendMessage(new FurniListNotificationComposer(1, 4));

                Session.SendMessage(new AchievementUnlockedComposer(AchData, UserAch.Level, Next.RewardPoints,
                    Next.RewardPixels, AchievementGroup + UserAch.Level));

                Session.GetHabbo().GetMessenger().BroadcastAchievement(Session.GetHabbo().Id,
                    Users.Messenger.MessengerEventTypes.ACHIEVEMENT_UNLOCKED, AchievementGroup + (UserAch.Level - 1));

                if (UserAch.Level < AchData.Levels.Count)
                {
                    Next = AchData.Levels[UserAch.Level + 1];
                }
            }

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("REPLACE INTO `user_achievements` VALUES ('" + Session.GetHabbo().Id + "', @group, '" + UserAch.Level + "', '" + UserAch.Progress + "')");
                dbClient.AddParameter("group", AchievementGroup);
                dbClient.RunQuery();
            }

            Session.SendMessage(new AchievementProgressedComposer(AchData, UserAch.Level + 1, Next, AchData.Levels.Count, UserAch));

            return true;

            /*if (!_achievements.ContainsKey(AchievementGroup) || Session == null)
                return false;

            Achievement AchievementData = null;
            AchievementData = _achievements[AchievementGroup];

            UserAchievement UserData = Session.GetHabbo().GetAchievementData(AchievementGroup);
            if (UserData == null)
            {
                UserData = new UserAchievement(AchievementGroup, 0, 0);
                Session.GetHabbo().Achievements.TryAdd(AchievementGroup, UserData);
            }

            int TotalLevels = AchievementData.Levels.Count;

            if (UserData != null && UserData.Level >= TotalLevels)
                return false; // done, no more.

            int OldLevel = (UserData != null ? UserData.Level : 0);

            int TargetLevel = (UserData != null ? UserData.Level + 1 : 1);

            if (TargetLevel > TotalLevels)
                TargetLevel = TotalLevels;

            AchievementLevel TargetLevelData = AchievementData.Levels[TargetLevel - 1 == 0 ? TargetLevel : TargetLevel - 1];
            int NewProgress = 0;
            if (FromZero)
                NewProgress = ProgressAmount;
            else
                NewProgress = (UserData != null ? UserData.Progress + ProgressAmount : ProgressAmount);

            int NewLevel = (UserData != null ? UserData.Level : 0);
            int NewTarget = NewLevel + 1;

            if (NewTarget > TotalLevels)
                NewTarget = TotalLevels;

            

            if (NewProgress >= TargetLevelData.Requirement)
            {
                if (AchievementData.Levels.Count < NewTarget)
                    return false;

                //NewLevel++;
                NewTarget++;

                int ProgressRemainder = NewProgress - TargetLevelData.Requirement;

                NewProgress = 0;

                AchievementLevel TargetACHLevel = AchievementData.Levels[NewLevel];
                while (ProgressRemainder > 0 && NewLevel <= AchievementData.Levels.Count)
                {
                    TargetACHLevel = AchievementData.Levels[NewLevel];
                    ProgressRemainder -= TargetACHLevel.Requirement;
                    NewLevel++;
                }
                

                TargetLevel = TargetACHLevel.Level;
                NewTarget = TargetACHLevel.Level;

                if (TargetLevel == 1)
                    Session.GetHabbo().GetBadgeComponent().GiveBadge(AchievementGroup + TargetLevel, true);
                else
                {
                    Session.GetHabbo().GetBadgeComponent().ReplaceBadge(Convert.ToString(AchievementGroup + (OldLevel)), AchievementGroup + TargetLevel, true);
                }

                if (NewTarget > TotalLevels)
                {
                    NewTarget = TotalLevels;
                }


                Session.SendMessage(new AchievementUnlockedComposer(AchievementData, TargetLevel, TargetLevelData.RewardPoints, TargetLevelData.RewardPixels, OldLevel > 0 ? AchievementGroup + OldLevel : ""));
                Session.GetHabbo().GetMessenger().BroadcastAchievement(Session.GetHabbo().Id, Users.Messenger.MessengerEventTypes.ACHIEVEMENT_UNLOCKED, AchievementGroup + TargetLevel);

                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("REPLACE INTO `user_achievements` VALUES ('" + Session.GetHabbo().Id + "', @group, '" + NewLevel + "', '" + NewProgress + "')");
                    dbClient.AddParameter("group", AchievementGroup);
                    dbClient.RunQuery();
                }

                UserData.Level = NewLevel;
                UserData.Progress = NewProgress > 0 ? NewProgress : 0;

                Session.GetHabbo().Duckets += TargetLevelData.RewardPixels;
                Session.GetHabbo().GetStats().AchievementPoints += TargetLevelData.RewardPoints;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, TargetLevelData.RewardPixels));
                Session.SendMessage(new AchievementScoreComposer(Session.GetHabbo().GetStats().AchievementPoints));

                AchievementLevel NewLevelData = AchievementData.Levels[NewTarget];
                Session.SendMessage(new AchievementProgressedComposer(AchievementData, NewTarget+1, NewLevelData, TotalLevels, Session.GetHabbo().GetAchievementData(AchievementGroup)));

                return true;
            }
            else
            {
                UserData.Level = NewLevel;
                UserData.Progress = NewProgress;
                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("REPLACE INTO `user_achievements` VALUES ('" + Session.GetHabbo().Id + "', @group, '" + NewLevel + "', '" + NewProgress + "')");
                    dbClient.AddParameter("group", AchievementGroup);
                    dbClient.RunQuery();
                }

                Session.SendMessage(new AchievementProgressedComposer(AchievementData, TargetLevel, TargetLevelData, TotalLevels, Session.GetHabbo().GetAchievementData(AchievementGroup)));
            }
            return false;*/
        }
        public ICollection<Achievement> GetGameAchievements(int GameId)
        {
            List<Achievement> GameAchievements = new List<Achievement>();
            foreach (Achievement Achievement in _achievements.Values.ToList())
            {
                if (Achievement.Category == "games" && Achievement.GameId == GameId)
                    GameAchievements.Add(Achievement);
            }
            return GameAchievements;
        }

        public Achievement GetAchievement(string achievementGroup)
           => _achievements.ContainsKey(achievementGroup) ? _achievements[achievementGroup] : null;
    }
}