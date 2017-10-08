using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Quasar.Communication.Packets.Incoming;

using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Pathfinding;

using Quasar.Communication.Packets.Outgoing.Rooms.Engine;
using Quasar.HabboHotel.Rooms.Games.Teams;
using Quasar.HabboHotel.Items.Wired;
using Quasar.Football;
using System.Threading.Tasks;
using System.Threading;
using Quasar.HabboHotel.Rooms;

namespace Quasar.HabboHotel.Rooms.Games.Football
{
    public class Soccer
    {
        private Room _room;
        private Item[] gates;
        private ConcurrentDictionary<int, Item> _balls;
        private bool _gameStarted;

        public Soccer(Room room)
        {
            this._room = room;
            this.gates = new Item[4];
            this._balls = new ConcurrentDictionary<int, Item>();
            this._gameStarted = false;
        }
        public bool GameIsStarted
        {
            get { return this._gameStarted; }
        }
        public void StopGame(bool userTriggered = false)
        {
            this._gameStarted = false;

            if (!userTriggered)
                _room.GetWired().TriggerEvent(WiredBoxType.TriggerGameEnds, null);
        }

        public void StartGame()
        {
            this._gameStarted = true;
            _room.GetWired().TriggerEvent(WiredBoxType.TriggerGameStarts, null);
        }

        public void AddBall(Item item)
        {
            this._balls.TryAdd(item.Id, item);
        }

        public void RemoveBall(int itemID)
        {
            Item Item = null;
            this._balls.TryRemove(itemID, out Item);
        }

        internal void OnUserWalk(RoomUser User)
        {
            if (User == null)
                return;

            foreach (Item ball in _balls.Values)
            {
                if (ball == null)
                    return;

                //If user position is same as ball position AND The goal Y is the same as Ball y position SHOOT!
                if (User.SetX == ball.GetX && User.SetY == ball.GetY && User.GoalX == ball.GetX && User.GoalY == ball.GetY && User.handelingBallStatus == 0) // super chute.
                {
                    Point userPoint = new Point(User.X, User.Y);
                    ball.ExtraData = "55";
                    ball.BallTryGoThrough = 0;
                    ball.Shoot = true;
                    ball.ballIsMoving = true;
                    ball.ballMover = User.GetClient();
                    MoveBall(ball, User.GetClient(), userPoint);
                }

                //if user position is same as ball //shoot!  SHOOT!
                else if (User.X == ball.GetX && User.Y == ball.GetY && User.handelingBallStatus == 0)
                {
                    Point userPoint = new Point(User.SetX, User.SetY);
                    ball.ExtraData = "55";
                    ball.BallTryGoThrough = 0;
                    ball.Shoot = true;
                    ball.ballIsMoving = true;
                    ball.ballMover = User.GetClient();
                    MoveBall(ball, User.GetClient(), userPoint);
                }
                else
                {
                    //if user goal is same as ball position
                    if (User.handelingBallStatus == 0 && User.GoalX == ball.GetX && User.GoalY == ball.GetY)
                        return;

                    if (User.SetX == ball.GetX && User.SetY == ball.GetY && User.IsWalking && (User.X != User.GoalX || User.Y != User.GoalY))
                    {
                        User.handelingBallStatus = 1;
                        IComeDirection _comeDirection = ComeDirection.GetComeDirection(new Point(User.X, User.Y), ball.Coordinate);
                        if (_comeDirection != IComeDirection.Null)
                        {
                            int NewX = User.SetX;
                            int NewY = User.SetY;

                            ComeDirection.GetNewCoords(_comeDirection, ref NewX, ref NewY);
                            if (ball.GetRoom().GetGameMap().ValidTile(NewX, NewY))
                            {
                                ball.ExtraData = "11";
                                ball.ballMover = User.GetClient();
                                MoveBall(ball, User.GetClient(), NewX, NewY);
                            }
                        }
                    }
                }
            }
        }

        private bool VerifyBall(RoomUser user, int actualx, int actualy)
        {
            return Rotation.Calculate(user.X, user.Y, actualx, actualy) == user.RotBody;
        }

        public void RegisterGate(Item item)
        {
            if (gates[0] == null)
            {
                item.team = TEAM.BLUE;
                gates[0] = item;
            }
            else if (gates[1] == null)
            {
                item.team = TEAM.RED;
                gates[1] = item;
            }
            else if (gates[2] == null)
            {
                item.team = TEAM.GREEN;
                gates[2] = item;
            }
            else if (gates[3] == null)
            {
                item.team = TEAM.YELLOW;
                gates[3] = item;
            }
        }

        public void UnRegisterGate(Item item)
        {
            switch (item.team)
            {
                case TEAM.BLUE:
                    {
                        gates[0] = null;
                        break;
                    }
                case TEAM.RED:
                    {
                        gates[1] = null;
                        break;
                    }
                case TEAM.GREEN:
                    {
                        gates[2] = null;
                        break;
                    }
                case TEAM.YELLOW:
                    {
                        gates[3] = null;
                        break;
                    }
            }
        }

        public void onGateRemove(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.FOOTBALL_GOAL_RED:
                case InteractionType.footballcounterred:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.RED);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_GREEN:
                case InteractionType.footballcountergreen:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.GREEN);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_BLUE:
                case InteractionType.footballcounterblue:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.BLUE);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_YELLOW:
                case InteractionType.footballcounteryellow:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.YELLOW);
                        break;
                    }
            }
        }

        private IEnumerable<Item> GetFootballItemsForAllTeams()
        {
            List<Item> items = _room.GetGameManager().GetFurniItems(TEAM.RED).Values.ToList();
            items.AddRange(_room.GetGameManager().GetFurniItems(TEAM.GREEN).Values);

            items.AddRange(_room.GetGameManager().GetFurniItems(TEAM.BLUE).Values);

            items.AddRange(_room.GetGameManager().GetFurniItems(TEAM.YELLOW).Values);

            return items;
        }

        private bool GameItemOverlaps(Item gameItem)
        {
            Point gameItemCoord = gameItem.Coordinate;
            return
                GetFootballItemsForAllTeams()
                    .Any(
                        item =>
                            item.GetAffectedTiles.Values.Any(
                                tile => tile.X == gameItemCoord.X && tile.Y == gameItemCoord.Y));
        }

        internal bool MoveBall(Item item, GameClient mover, int newX, int newY)
        {
            if (item == null || item.GetBaseItem() == null)
                return false;


            bool itemIsOnGameItem = GameItemOverlaps(item);

            if (!_room.GetGameMap().itemCanBePlacedHere(newX, newY))
                return false;

            Point oldRoomCoord = item.Coordinate;

            Double NewZ = _room.GetGameMap().Model.SqFloorHeight[newX, newY];

            _room.SendMessage(new UpdateFootBallComposer(item, newX, newY));

            if (oldRoomCoord.X == newX && oldRoomCoord.Y == newY)
                return false;

            item.SetState(newX, newY, item.GetZ, Gamemap.GetAffectedTiles(item.GetBaseItem().Length, item.GetBaseItem().Width, newX, newY, item.Rotation));

            if (itemIsOnGameItem || mover == null || mover.GetHabbo() == null)
                return false;

            this._room.OnUserShoot(mover, item);

            return true;
        }

        public void MoveBall(Item item, GameClient client, Point user, bool skip = false)
        {
            try
            {
                item.comeDirection = ComeDirection.GetComeDirection(user, item.Coordinate);

                if (item.comeDirection != IComeDirection.Null)
                    // item.ballMover = client;
                    new TaskFactory().StartNew(() => MoveBallProcess(item, skip));
            }
            catch
            {
            }
        }

        public async void MoveBallProcess(Item item, bool skip = false)
        {
            if (item.ballMover == null || item.ballMover.GetHabbo() == null)
                return;

            if (item == null)
                return;

            int newX = item.Coordinate.X;
            int newY = item.Coordinate.Y;

            // if comedirection niets is?!
            if (item.comeDirection == IComeDirection.Null)
            {
                item.ballIsMoving = false;
                return;
            }

            if (_room == null)
                return;

            if (_room.GetRoomUserManager().GetRoomUsers() == null)
                return;

            List<RoomUser> users = _room.GetRoomUserManager().GetRoomUsers().OrderBy(a => Guid.NewGuid()).ToList();
            foreach (RoomUser user in users)
            {
                if (user.SetX == newX && user.SetY == newY && user.UserId != item.ballMover.GetHabbo().Id && item.ExtraData != "44" && item.ExtraData != "33"
                    && user.VirtualId > _room.GetRoomUserManager().GetRoomUserByHabbo(item.ballMover.GetHabbo().Id).VirtualId)
                {
                    Point userPoint = new Point(user.X, user.Y);
                    item.ExtraData = "55";
                    item.ballIsMoving = true;
                    item.BallTryGoThrough = 0;
                    item.Shoot = true;
                    item.ballMover = user.GetClient();
                    MoveBall(item, user.GetClient(), userPoint);
                    return;
                }
            }

            //Set old newX and newY
            int resetX = newX;
            int resetY = newY;

            //Calc other direction
            ComeDirection.GetNewCoords(item.comeDirection, ref newX, ref newY);

            bool ignoreUsers = false;
            if (_room.GetGameMap().SquareHasUsers(newX, newY))
            {
                if (item.ExtraData != "55" && item.ExtraData != "44")
                {
                    item.ballIsMoving = false;
                    return;
                }

                ignoreUsers = true;
            }

            if (ignoreUsers == false)
            {
                //check if ball can go through
                if (!_room.GetGameMap().itemCanBePlacedHere(newX, newY))
                {
                    //A ball can't go through a wall!!! Set reversed coordinates.
                    item.comeDirection = ComeDirection.InverseDirections(_room, item.comeDirection, newX, newY);
                    newX = resetX;
                    newY = resetY;

                    item.BallTryGoThrough++;

                    //try 2 times
                    if (item.BallTryGoThrough > 2)
                    {
                        item.ballIsMoving = false;
                        item.ExtraData = ("0").ToString();
                        return;
                    }

                    //try again
                    MoveBallProcess(item, true);
                    return;
                }
            }

            if (item.ballIsMoving)
            {
                if (MoveBall(item, item.ballMover, newX, newY))
                {
                    int ExtraData = 11;
                    int.TryParse(item.ExtraData, out ExtraData);

                    if (ExtraData > 11)
                    {
                        if (!item.Shoot)
                        {
                            item.ExtraData = (int.Parse(item.ExtraData) - 11).ToString();
                        }

                        item.Shoot = false;
                        // wait 90 secs
                        await Task.Delay(_room.BallSpeed);
                        MoveBallProcess(item, false);
                        return;
                    }
                }

                item.ExtraData = "0";
                item.BallTryGoThrough = 0;
                item.ballIsMoving = false;
            }
        }

        public void Dispose()
        {
            Array.Clear(gates, 0, gates.Length);
            gates = null;
            _room = null;
            _balls.Clear();
            _balls = null;
        }
    }
}