using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Rooms.Games;
using Quasar.HabboHotel.Rooms.Games.Teams;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class EnableCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_enable"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Laat je Habbis een effect uitvoeren!"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je bent vergeten een geldig effect in te vullen.");
                return;
            }

            if (!Room.EnablesEnabled && !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendWhisper("Oeps! Je kan in deze kamer geen effecten dragen.");
                return;
            }

            RoomUser ThisUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            if (ThisUser == null)
                return;

            if (ThisUser.RidingHorse)
            {
                Session.SendWhisper("Oeps! Je kan dit effect niet doen terwijl je aan het paardrijden bent.");
                return;
            }
            else if (ThisUser.Team != TEAM.NONE)
                return;
            else if (ThisUser.isLying)
                return;

            int EffectId = 0;
            if (!int.TryParse(Params[1], out EffectId))
                return;

            if (EffectId > int.MaxValue || EffectId < int.MinValue)
                return;

            if ((EffectId == 102 ||  EffectId == 593 || EffectId == 598) && !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendWhisper("Oeps! Deze effecten zijn enkel beschikbaar voor Habbis Personeel.");
                return;
            }

            if (EffectId == 178 && Session.GetHabbo()._hulptroepen < 4 || EffectId == 178 && Session.GetHabbo()._hulptroepen > 3)
            {
                Session.SendWhisper("Oeps! Dit effect is enkel beschikbaar voor Habbis Ambassadeurs.");
                return;
            }
            if (EffectId == 599 && Session.GetHabbo()._hulptroepen < 2 || EffectId == 599 && Session.GetHabbo()._hulptroepen > 2)
            {
                Session.SendWhisper("Oeps! Dit effect is enkel beschikbaar voor Habbis Bouwers.");
                return;
            }

            if (EffectId == 594 && Session.GetHabbo()._hulptroepen < 3 || EffectId == 599 && Session.GetHabbo()._hulptroepen > 3)
            {
                Session.SendWhisper("Oeps! Dit effect is enkel beschikbaar voor Habbis Palers.");
                return;
            }

            if (EffectId == 596 && Session.GetHabbo()._hulptroepen < 1 || EffectId == 596 && Session.GetHabbo()._hulptroepen > 1)
            {
                Session.SendWhisper("Oeps! Dit effect is enkel beschikbaar voor Habbis Entertainers.");
                return;
            }

            if (EffectId == 592 && !Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oeps! Dit effect is enkel beschikbaar voor Habbis die rechten hebben in de kamer.");
                return;
            }

            Session.GetHabbo().Effects().ApplyEffect(EffectId);
        }
    }
}
