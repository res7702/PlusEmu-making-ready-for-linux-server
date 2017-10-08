using Quasar.Communication.Packets.Outgoing.Notifications;
using System.Data;
using System;
using Quasar.Database.Interfaces;
using System.Text;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Events
{
    internal class FaqCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "command_faq";
            }
        }
        public string Parameters
        {
            get
            {
                return "";
            }
        }
        public string Description
        {
            get
            {
                return "De meest gestelde vragen.";
            }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            DataTable table;
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT question, answer FROM faq");
                table = dbClient.getTable();
            }
            StringBuilder builder = new StringBuilder();
            builder.Append("De meest gestelde vragen van gebruikers: \r\r");
            foreach (DataRow row in table.Rows)
            {
                builder.Append("Q " + ((string)row["question"]) + "\r");
                builder.Append("A: " + ((string)row["answer"]) + "\r\r");
            }
            Session.SendMessage(new MOTDNotificationComposer(builder.ToString()));
        }
    }
}



