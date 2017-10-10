using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Nux;
using Quasar.Communication.Packets.Outgoing;

namespace Quasar.Communication.Packets.Incoming.Nucs
{
    class RoomNucsAlerta : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            var habbo = Session.GetHabbo();
            if (habbo == null) return;
            if (!habbo.PassedNuxNavigator && !habbo.PassedNuxCatalog && !habbo.PassedNuxChat && !habbo.PassedNuxDuckets && !habbo.PassedNuxItems)
            {
                Session.SendMessage(new NuxAlertComposer("helpBubble/add/BOTTOM_BAR_NAVIGATOR/Dit hier noemen we de Navigator, klik erop en ga opzoek naar de leukste kamers!"));
                habbo.PassedNuxNavigator = true;
            }

            if (habbo.PassedNuxNavigator && !habbo.PassedNuxCatalog && !habbo.PassedNuxChat && !habbo.PassedNuxDuckets && !habbo.PassedNuxItems)
            {
                Session.SendMessage(new NuxAlertComposer("helpBubble/add/BOTTOM_BAR_CATALOG/Een kamer zonder decoratie is natuurlijk alles behalve af. Gebruik daarom deze winkel om de meest chique hotel kamers te maken!"));
                habbo.PassedNuxCatalog = true;
            }
            else if (habbo.PassedNuxNavigator && habbo.PassedNuxCatalog && !habbo.PassedNuxChat && !habbo.PassedNuxDuckets && !habbo.PassedNuxItems)
            {
                Session.SendMessage(new NuxAlertComposer("helpBubble/add/CHAT_INPUT/Chatten was nog nooit zo simpel, communiceer met andere Habbis in verschillende kleuren en stylen!"));
                habbo.PassedNuxChat = true;
            }
            else if (habbo.PassedNuxNavigator && habbo.PassedNuxCatalog && habbo.PassedNuxChat && !habbo.PassedNuxDuckets && !habbo.PassedNuxItems)
            {
                Session.SendMessage(new NuxAlertComposer("helpBubble/add/DUCKETS_BUTTON/Dit hier is je geld, de economie bestaat uit Credits, Duckets en Diamanten."));
                habbo.PassedNuxDuckets = true;
            }
            else if (habbo.PassedNuxNavigator && habbo.PassedNuxCatalog && habbo.PassedNuxChat && habbo.PassedNuxDuckets && !habbo.PassedNuxItems)
            {
                Session.SendMessage(new NuxAlertComposer("helpBubble/add/BOTTOM_BAR_INVENTORY/Alles wat er gekocht of geruild wordt komt in je inventaris terecht, van uit hier is het ook weer mogelijk om items in je kamer te plaatsen."));
                habbo.PassedNuxItems = true;
            }

            if (habbo.PassedNuxNavigator && habbo.PassedNuxCatalog && habbo.PassedNuxChat && habbo.PassedNuxDuckets && habbo.PassedNuxItems)
            {
             //   Session.SendMessage(new NuxAlertComposer("nux/lobbyoffer/show"));
                habbo._NUX = false;
                using (var dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                    dbClient.runFastQuery("UPDATE users SET nux_user = 'false' WHERE id = " + Session.GetHabbo().Id + ";");
                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_NuxSucceeded", 1);
                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_Login", 1);
                //QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RegistrationDuration", 1);
                var nuxStatus = new ServerPacket(ServerPacketHeader.NuxUserStatus);
                nuxStatus.WriteInteger(0);
                Session.SendMessage(nuxStatus); 
            }
        }
    }
}
