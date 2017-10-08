using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Rooms.Avatar;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;


using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
using Quasar.Database.Interfaces;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class BewCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_bew"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Zet jezelf weer op aanwezig"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            User.UnIdle();
            User.ApplyEffect(0);
            Session.SendWhisper("Je bent weer terug, welkom terug!");
        }
    }
}
