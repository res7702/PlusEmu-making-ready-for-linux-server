#region

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Rooms.Avatar;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.HabboHotel.GameClients;

using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
using Quasar.Database.Interfaces;

#endregion

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    internal class HoverboardCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_hoverboard"; }
        }


        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Rij op een coole Hoverboard!"; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            User.ApplyEffect(191);
        }
    }
}