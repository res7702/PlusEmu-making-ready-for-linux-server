using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.Communication.Packets.Outgoing.Rooms.Engine;
using Quasar.Database.Interfaces;


namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class GezichtloosCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_gezichtloos"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Loop door Habbis zonder gezicht."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null || User.GetClient() == null)
                return;

            string[] headParts;
            string[] figureParts = Session.GetHabbo().Look.Split('.');
            foreach (string Part in figureParts)
            {
                if (Part.StartsWith("hd"))
                {
                    headParts = Part.Split('-');
                    if (!headParts[1].Equals("99999"))
                        headParts[1] = "99999";
                    else
                        return;

                    Session.GetHabbo().Look = Session.GetHabbo().Look.Replace(Part, "hd-" + headParts[1] + "-" + headParts[2]);
                    break;
                }
            }
            Session.GetHabbo().Look = QuasarEnvironment.GetGame().GetAntiMutant().RunLook(Session.GetHabbo().Look);
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `look` = @look WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("look", Session.GetHabbo().Look);
                dbClient.RunQuery();
            }
            Session.SendWhisper("Je hebt nu geen gezicht meer! :)", 34);
            Session.SendMessage(new UserChangeComposer(User, true));
            Session.GetHabbo().CurrentRoom.SendMessage(new UserChangeComposer(User, false));
            return;
        }
    }
}
