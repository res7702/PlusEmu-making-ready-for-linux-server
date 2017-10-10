using Quasar.HabboHotel.Users;

namespace Quasar.Communication.Packets.Outgoing.Handshake
{
    public class UserRightsComposer : ServerPacket
    {
            public UserRightsComposer(Habbo habbo)
                : base(ServerPacketHeader.UserRightsMessageComposer)
            {
                if (habbo.GetClubManager().HasSubscription("habbo_vip"))
                {
                    base.WriteInteger(2);
                }
                else if (habbo.GetClubManager().HasSubscription("habbo_club"))
                {
                    base.WriteInteger(1);
                }
                else
                {
                    base.WriteInteger(0);
                }

                base.WriteInteger(habbo.Rank);
                base.WriteBoolean(false);//Is an ambassador
            }
        }
    }