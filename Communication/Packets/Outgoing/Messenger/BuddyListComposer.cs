using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.Users.Messenger;
using Quasar.HabboHotel.Users.Relationships;

namespace Quasar.Communication.Packets.Outgoing.Messenger
{
    class BuddyListComposer : ServerPacket
    {
        public BuddyListComposer(ICollection<MessengerBuddy> Friends, Habbo Player)
            : base(ServerPacketHeader.BuddyListMessageComposer)
        {
            var friendCount = Friends.Count;
            if (Player.Rank >= 2) friendCount++;

            base.WriteInteger(1);
            base.WriteInteger(0);
            base.WriteInteger(friendCount);

            foreach (MessengerBuddy Friend in Friends.ToList())
            {
                Relationship Relationship = Player.Relationships.FirstOrDefault(x => x.Value.UserId == Convert.ToInt32(Friend.UserId)).Value;

                base.WriteInteger(Friend.Id);
                base.WriteString(Friend.mUsername);
                base.WriteInteger(1);//Gender.
                base.WriteBoolean(Friend.IsOnline);
                base.WriteBoolean(Friend.IsOnline && Friend.InRoom);
                base.WriteString(Friend.IsOnline ? Friend.mLook : string.Empty);
                base.WriteInteger(0); // category id
                base.WriteString(Friend.IsOnline ? Friend.mMotto : string.Empty);
                base.WriteString(string.Empty);//Alternative name?
                base.WriteString(string.Empty);
                base.WriteBoolean(true);
                base.WriteBoolean(false);
                base.WriteBoolean(false);//Pocket Habbo user.
                base.WriteShort(Relationship == null ? 0 : Relationship.Type);
            }
            if (Player.Rank >= 2)
            {
                base.WriteInteger(int.MinValue);  // Int.MaxValue
                base.WriteString("Personeel");
                base.WriteInteger(1);
                base.WriteBoolean(true);
                base.WriteBoolean(false);
                base.WriteString("b07134s01094");
                base.WriteInteger(0);
                base.WriteString(string.Empty);
                base.WriteString(string.Empty);
                base.WriteString(string.Empty);
                base.WriteBoolean(true);
                base.WriteBoolean(false);
                base.WriteBoolean(false);
                base.WriteShort(0);
            }
        }
    }
}
