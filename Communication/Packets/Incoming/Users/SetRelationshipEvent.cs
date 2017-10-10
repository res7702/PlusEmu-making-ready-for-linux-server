using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Users.Relationships;

using Quasar.HabboHotel.Users;
using Quasar.Communication.Packets.Outgoing.Messenger;
using Quasar.HabboHotel.Users.Messenger;
using Quasar.Database.Interfaces;
using Quasar.Communication.Packets.Outgoing.Moderation;
using Quasar.HabboHotel.Quests;

namespace Quasar.Communication.Packets.Incoming.Users
{
    class SetRelationshipEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
                return;

            int User = Packet.PopInt();
            int Type = Packet.PopInt();

            if (!Session.GetHabbo().GetMessenger().FriendshipExists(User))
            {
                Session.SendMessage(new BroadcastMessageAlertComposer("Oeps! Je kunt alleen vrienden toevoegen aan je relatie status."));
                return;
            }

            if (Type < 0 || Type > 3)
            {
                Session.SendMessage(new BroadcastMessageAlertComposer("Oeps! Er een onbekende fout opgetreden bij het kiezen van een relatie status."));
                return;
            }

            if (Session.GetHabbo().Relationships.Count > 2000)
            {
                Session.SendMessage(new BroadcastMessageAlertComposer("Oeps! Je hebt het relatie status limiet overstreden (2000)."));
                return;
            }

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (Type == 0)
                {
                    dbClient.SetQuery("SELECT `id` FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", User);
                    int Id = dbClient.getInteger();

                    dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", User);
                    dbClient.RunQuery();

                    if (Session.GetHabbo().Relationships.ContainsKey(User))
                        Session.GetHabbo().Relationships.Remove(User);
                }
                else
                {
                    dbClient.SetQuery("SELECT id FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", User);
                    int Id = dbClient.getInteger();

                    if (Id > 0)
                    {
                        dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                        dbClient.AddParameter("target", User);
                        dbClient.RunQuery();

                        if (Session.GetHabbo().Relationships.ContainsKey(Id))
                            Session.GetHabbo().Relationships.Remove(Id);
                    }

                    dbClient.SetQuery("INSERT INTO `user_relationships` (`user_id`,`target`,`type`) VALUES ('" + Session.GetHabbo().Id + "', @target, @type)");
                    dbClient.AddParameter("target", User);
                    dbClient.AddParameter("type", Type);
                    int newId = Convert.ToInt32(dbClient.InsertQuery());
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RelatieStatus", 1);
                 
                    if (!Session.GetHabbo().Relationships.ContainsKey(User))
                        Session.GetHabbo().Relationships.Add(User, new Relationship(newId, User, Type));
                       
                }

                GameClient Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(User);
                if (Client != null)
                    Session.GetHabbo().GetMessenger().UpdateFriend(User, Client, true);
                else
                {
                    Habbo Habbo = QuasarEnvironment.GetHabboById(User);
                    if (Habbo != null)
                    {
                        MessengerBuddy Buddy = null;
                        if (Session.GetHabbo().GetMessenger().TryGetFriend(User, out Buddy))
                            Session.SendMessage(new FriendListUpdateComposer(Session, Buddy));
                    }
                }
            }
        }
    }
}