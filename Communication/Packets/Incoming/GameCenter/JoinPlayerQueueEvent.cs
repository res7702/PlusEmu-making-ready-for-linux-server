#region

using System;
using System.Data;
using System.Text;
using Quasar.Communication.Packets.Outgoing.GameCenter;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Games;

#endregion

namespace Quasar.Communication.Packets.Incoming.GameCenter
{
    internal class JoinPlayerQueueEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if ((Session == null) || (Session.GetHabbo() == null))
                return;

            var GameId = Packet.PopInt();

            GameData GameData = null;
            if (QuasarEnvironment.GetGame().GetGameDataManager().TryGetGame(GameId, out GameData))
            {
                Session.SendMessage(new JoinQueueComposer(GameData.GameId));
                var HabboID = Session.GetHabbo().Id;
                using (var dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    DataTable data;
                    dbClient.SetQuery("SELECT user_id FROM user_auth_ticket WHERE user_id = '" + HabboID + "'");
                    data = dbClient.getTable();
                    var count = 0;
                    foreach (DataRow row in data.Rows)
                    {
                        if (Convert.ToInt32(row["user_id"]) == HabboID)
                            count++;
                    }
                    if (count == 0)
                    {
                        var SSOTicket = "Fastfood-" + GenerateSSO(32) + "-" + Session.GetHabbo().Id;
                        dbClient.RunQuery("INSERT INTO user_auth_ticket(user_id, auth_ticket) VALUES ('" + HabboID +
                                          "', '" +
                                          SSOTicket + "')");
                        Session.SendMessage(new LoadGameComposer(GameData, SSOTicket));
                    }
                    else
                    {
                        dbClient.SetQuery("SELECT user_id,auth_ticket FROM user_auth_ticket WHERE user_id = " + HabboID);
                        data = dbClient.getTable();
                        foreach (DataRow dRow in data.Rows)
                        {
                            var SSOTicket = dRow["auth_ticket"];
                            Session.SendMessage(new LoadGameComposer(GameData, (string)SSOTicket));
                        }
                    }

                    // Console.WriteLine(SSOTicket + "\n" + HabboID);
                }
            }
        }

        private string GenerateSSO(int length)
        {
            var random = new Random();
            var characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var result = new StringBuilder(length);
            for (var i = 0; i < length; i++)
                result.Append(characters[random.Next(characters.Length)]);
            return result.ToString();
        }
    }
}