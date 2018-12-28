using System;
using System.Collections.Generic;
using Brain.Utils;
using BrainGame.DataModel;
using PropertyChanged;

namespace BrainGame.Game
{
    public enum MoveType
    {
        Up,
        Right,
        Down,
        Left
    }

    [ImplementPropertyChanged]
    public class BinaryGame
    {
        private const int StartTiles = 2;

        public BinaryGame(GameDefinition gameDefinition)
        {
            GameDefinition = gameDefinition;
            BinaryGrid = new BinaryGrid(gameDefinition);
        }


        public int Score { get; set; }
        public int Moves { get; set; }
        public bool IsGameOver { get; set; }
        public bool HasGameWon { get; set; }

        public GameDefinition GameDefinition { get; set; }
        public BinaryGrid BinaryGrid { get; private set; }
        public GameData GameData { get; set; }

        public void Setup()
        {
            Score = 0;
            Moves = 0;
            IsGameOver = false;

            GameData.GameCount++;

            BinaryGrid.Setup();

            AddStartTiles();
        }

        public static string GetRank(int value)
        {
            if (value > 4096)
                return "2048 God";
            switch (value)
            {
                default:
                    return "";
                case 16:
                    return "Beginner";
                case 32:
                    return "Rookie";
                case 64:
                    return "Novice";
                case 128:
                    return "Amateur";
                case 256:
                    return "Skilled";
                case 512:
                    return "Profesional";
                case 1024:
                    return "Expert";
                case 2048:
                    return "Ninja";
                case 4096:
                    return "Extreme Ninja";
            }
        }

        private void AddStartTiles()
        {
            for (int i = 0; i < StartTiles; i++)
                AddRandomTile();
        }

        private void AddRandomTile()
        {
            if (BinaryGrid.AvailablePositionCount() > 0)
            {
                int value = (RandomManager.NextDouble() < 0.9) ? 2 : 4;
                TileData tile = BinaryGrid.InsertTile(BinaryGrid.RandomAvailablePosition(), value);

                RaiseTileAdded(tile);
            }
        }

        public void RestoreTile(TileData tile, GameDefinition gameDefinition)
        {
            if (tile == null) return;

            tile.GameDefinition = gameDefinition;
            BinaryGrid.InsertTile(tile);
            RaiseTileAdded(tile);
        }


        private void PrepareTiles()
        {
            BinaryGrid.ForEachTile(tile =>
            {
                tile.MergedFrom = null;
                tile.PreviousPos = tile.Pos;
            });
        }

        public void Move(MoveType move)
        {
            if (IsGameOver) return;

            List<int> Xs;
            List<int> Ys;
            bool moved = false;
            XY vector = GetVector(move);
            BuildTransversals(vector, out Xs, out Ys);

            PrepareTiles();
            foreach (int x in Xs)
            {
                foreach (int y in Ys)
                {
                    var pos = new XY(x, y);
                    TileData tile = BinaryGrid.TileAt(pos);

                    if (tile != null)
                    {
                        XY fathest, nextPos;
                        FindFarthestPosition(pos, vector, out fathest, out nextPos);
                        TileData nextTile = BinaryGrid.TileAt(nextPos);

                        if ((nextTile != null) &&
                            (nextTile.Value == tile.Value) &&
                            (nextTile.MergedFrom == null))
                        {
                            RaiseTileRemoved(nextTile);
                            BinaryGrid.RemoveTile(nextPos);

                            tile.MergedFrom = nextTile;
                            tile.Value *= 2;

                            Score += tile.Value;

                            BinaryGrid.MoveTile(tile, nextPos);
                            RaiseTileMoved(tile);

                            if (tile.Value >= 2048)
                            {
                                HasGameWon = true;
                                RaiseGameWon();
                            }
                            moved = true;
                        }
                        else
                        {
                            if ((pos.X != fathest.X) ||
                                (pos.Y != fathest.Y))
                            {
                                BinaryGrid.MoveTile(tile, fathest);
                                RaiseTileMoved(tile);
                                moved = true;
                            }
                        }
                    }
                }
            }

            if (moved)
            {
                Moves++;
                GameData.MoveCount++;

                AddRandomTile();

                if (!MovesAvailable())
                {
                    IsGameOver = true;
                    RaiseGameOver();
                }
            }
        }

        private XY GetVector(MoveType move)
        {
            switch (move)
            {
                case MoveType.Left:
                    return new XY(-1, 0);
                case MoveType.Up:
                    return new XY(0, -1);
                case MoveType.Right:
                    return new XY(1, 0);
                case MoveType.Down:
                    return new XY(0, 1);
                default:
                    return null;
            }
        }

        private void BuildTransversals(XY vector, out List<int> Xs, out List<int> Ys)
        {
            Xs = new List<int>();
            Ys = new List<int>();

            for (int x = 0; x < BinaryGrid.Width; x++)
                Xs.Add(x);
            for (int y = 0; y < BinaryGrid.Height; y++)
                Ys.Add(y);

            if (vector.X == 1)
                Xs.Reverse();
            if (vector.Y == 1)
                Ys.Reverse();
        }

        private void FindFarthestPosition(XY pos, XY vector, out XY fathest, out XY next)
        {
            XY previous = null;

            do
            {
                previous = pos;
                pos = new XY(previous.X + vector.X, previous.Y + vector.Y);
            } while (BinaryGrid.WithinBounds(pos) && BinaryGrid.PosAvailable(pos));

            fathest = previous;
            next = pos;
        }

        private bool MovesAvailable()
        {
            if (BinaryGrid.AvailablePositionCount() > 0)
                return true;
            if (TileMatchesAvailable())
                return true;
            return false;
        }

        private bool TileMatchesAvailable()
        {
            for (int x = 0; x < BinaryGrid.Width; x++)
            {
                for (int y = 0; y < BinaryGrid.Height; y++)
                {
                    TileData tile = BinaryGrid.Tiles[x, y];
                    if (tile != null)
                    {
                        for (int direction = 0; direction < 4; direction++)
                        {
                            XY vector = GetVector((MoveType) direction);
                            var pos = new XY(x + vector.X, y + vector.Y);

                            TileData other = BinaryGrid.TileAt(pos);
                            if ((other != null) && (other.Value == tile.Value))
                                return true;
                        }
                    }
                }
            }
            return false;
        }


        /**************************************************************/

        public event EventHandler<TileData> TileAdded;
        public event EventHandler<TileData> TileMoved;
        public event EventHandler<TileData> TileRemoved;
        public event EventHandler GameWon;
        public event EventHandler GameOver;

        protected virtual void RaiseTileAdded(TileData tile)
        {
            EventHandler<TileData> handler = TileAdded;
            if (handler != null) handler(this, tile);
        }

        protected virtual void RaiseTileMoved(TileData tile)
        {
            EventHandler<TileData> handler = TileMoved;
            if (handler != null) handler(this, tile);
        }

        protected virtual void RaiseTileRemoved(TileData tile)
        {
            EventHandler<TileData> handler = TileRemoved;
            if (handler != null) handler(this, tile);
        }

        protected virtual void RaiseGameWon()
        {
            EventHandler handler = GameWon;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void RaiseGameOver()
        {
            EventHandler handler = GameOver;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /**************************************************************/
    }
}