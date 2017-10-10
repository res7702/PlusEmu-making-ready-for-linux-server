using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.HabboHotel.Achievements;
using Quasar.HabboHotel.GameClients;

namespace Quasar.Communication.Packets.Outgoing.Talents
{
    class TalentTrackComposer : ServerPacket
    {
        public TalentTrackComposer(GameClient session, string trackType, List<Talent> talents)
            : base(ServerPacketHeader.TalentTrackMessageComposer)
        {
            base.WriteString(trackType);
            base.WriteInteger(talents.Count);

            int failLevel = -1;

            foreach (Talent current in talents)
            {
                base.WriteInteger(current.Level);
                int nm = failLevel == -1 ? 1 : 0; // TODO What does this mean?
                base.WriteInteger(nm);

                List<Talent> children = QuasarEnvironment.GetGame().GetTalentManager().GetTalents(trackType, current.Id);

                base.WriteInteger(children.Count);

                foreach (Talent child in children)
                {
                    UserAchievement achievment = session.GetHabbo().GetAchievementData(child.AchievementGroup);

                    int num;
                    // TODO Refactor What does num mean?!
                    if (failLevel != -1 && failLevel < child.Level)
                    {
                        num = 0;
                    }
                    else if (achievment == null)
                    {
                        num = 1;
                    }
                    else if (achievment.Level >=
                      child.AchievementLevel)
                    {
                        num = 2;
                    }
                    else
                    {
                        num = 1;
                    }

                    base.WriteInteger(child.GetAchievement().Id);
                    base.WriteInteger(0); // TODO Magic constant

                    base.WriteString(child.AchievementGroup + child.AchievementLevel);
                    base.WriteInteger(num);

                    base.WriteInteger(achievment != null ? achievment.Progress : 0);
                    base.WriteInteger(child.GetAchievement() == null ? 0
                        : child.GetAchievement().Levels[child.AchievementLevel].Requirement);

                    if (num != 2 && failLevel == -1)
                        failLevel = child.Level;
                }

                base.WriteInteger(0); // TODO Magic constant

                // TODO Type should be enum?
                if (current.Type == "citizenship" && current.Level == 4)
                {
                    base.WriteInteger(2);
                    base.WriteString("HABBO_CLUB_VIP_7_DAYS");
                    base.WriteInteger(7);
                    base.WriteString(current.Prize); // TODO Hardcoded stuff
                    base.WriteInteger(0);
                }
                else
                {
                    base.WriteInteger(1);
                    base.WriteString(current.Prize);
                    base.WriteInteger(0);
                }
            }
        }
    }
}
