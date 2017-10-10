using Quasar.Communication.Interfaces;
using Quasar.Communication.Packets.Outgoing;
using Quasar.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Incoming.LandingView
{
    class GetHallOfFame
    {
        private static GetHallOfFame instance = new GetHallOfFame()
            ;
        internal static GetHallOfFame getInstance()
        {
            return instance;
        }
        private Dictionary<uint, UserRank> ranks;
        private List<UserCompetition> usersWithRank;

        internal GetHallOfFame()
        {
            ranks = new Dictionary<uint, UserRank>();
            usersWithRank = new List<UserCompetition>();
        }

        internal void Load()
        {
            using (IQueryAdapter dbQuery = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ranks = new Dictionary<uint, UserRank>();
                usersWithRank = new List<UserCompetition>();

                dbQuery.SetQuery("SELECT * FROM ranks");
                var gRanksTable = (DataTable)dbQuery.getTable();

                foreach (DataRow Row in gRanksTable.Rows)
                    if (!ranks.ContainsKey((uint)Row["id"]))
                        ranks.Add((uint)Row["id"], new UserRank((string)Row["name"], (string)Row["badgeid"]));

                dbQuery.SetQuery("SELECT * FROM users WHERE rank BETWEEN 2 AND 7 ORDER BY rank DESC LIMIT 16");
                DataTable gUsersTable = dbQuery.getTable();

                foreach (DataRow Row in gUsersTable.Rows)
                {
                    var staff = new UserCompetition(Row);
                    if (!usersWithRank.Contains(staff))
                        usersWithRank.Add(staff);
                }
            }

        }

        internal void Serialize(ServerPacket message)
        {
            message.WriteInteger(usersWithRank.Count);
            foreach (UserCompetition user in usersWithRank)
            {
                message.WriteInteger((int)user.userId); //userID
                message.WriteString(user.userName);//userName
                message.WriteString(user.Look);//Look
                message.WriteInteger((int)user.Rank); //rank
                message.WriteInteger((int)user.Rank);//?
            }
        }
    }

    class UserCompetition
    {
        internal int userId, Rank;
        internal string userName, Look;

        internal UserCompetition(DataRow row)
        {
            this.userId = (int)row["id"];
            this.userName = (string)row["username"];
            this.Look = (string)row["look"];
            this.Rank = Convert.ToInt32(row["rank"].ToString());
        }
    }

    class UserRank
    {
        internal string Name, BadgeId;

        internal UserRank(string Name, string BadgeId)
        {
            this.Name = Name;
            this.BadgeId = BadgeId;
        }
    }
}
