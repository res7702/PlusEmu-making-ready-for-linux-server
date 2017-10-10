using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Achievements;

namespace Quasar.Communication.Packets.Outgoing.Inventory.Achievements
{
    class AchievementUnlockedComposer : ServerPacket
    {
        public AchievementUnlockedComposer(Achievement Achievement, int Level, int PointReward, int PixelReward, string Replacebadge)
            : base(ServerPacketHeader.AchievementUnlockedMessageComposer)
        {
            base.WriteInteger(Achievement.Id); // Achievement ID
            base.WriteInteger(Level); // Achieved level
            base.WriteInteger(Achievement.Id); // Unknown. Random useless number.
            base.WriteString(Achievement.GroupName + Level); // Achieved name
            base.WriteInteger(PointReward); // Point reward
            base.WriteInteger(PixelReward); // Pixel reward
            base.WriteInteger(Achievement.Id); // Unknown.
            base.WriteInteger(Achievement.Id); // Unknown.
            base.WriteInteger(Achievement.Id); // Unknown. (Extra reward?)
            base.WriteString(Level > 1 ? Achievement.GroupName + (Level - 1) : string.Empty);
            base.WriteString(Replacebadge);
            base.WriteString(Achievement.Category);
            base.WriteBoolean(true);
        }

        public AchievementUnlockedComposer(string badge)
            : base(ServerPacketHeader.AchievementUnlockedMessageComposer)
        {
            base.WriteInteger(-1); // Achievement ID
            base.WriteInteger(-1); // Achieved level
            base.WriteInteger(-1); // Unknown. Random useless number.
            base.WriteString(badge); // Achieved name
            base.WriteInteger(0); // Point reward
            base.WriteInteger(0); // Pixel reward
            base.WriteInteger(-1); // Unknown.
            base.WriteInteger(-1); // Unknown.
            base.WriteInteger(-1); // Unknown. (Extra reward?)
            base.WriteString("");
            base.WriteString("");
            base.WriteBoolean(false);
        }
    }
}