using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.LandingView;
using Quasar.HabboHotel.LandingView.Promotions;
using Quasar.Communication.Packets.Outgoing.LandingView;

namespace Quasar.Communication.Packets.Incoming.LandingView
{
    class GetPromoArticlesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            ICollection<Promotion> LandingPromotions = QuasarEnvironment.GetGame().GetLandingManager().GetPromotionItems();

            Session.SendMessage(new PromoArticlesComposer(LandingPromotions));
        }
    }
}
