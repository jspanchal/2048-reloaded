using System;
using System.Collections.Generic;
using Brain.Utils;
using BrainGame.DataModel;
using PropertyChanged;

namespace BrainGame.Game
{
    [ImplementPropertyChanged]
    public class BinaryGrid
    {
        private string _definitionUniqueId;
        private GameDefinition gameDefinition;

        public BinaryGrid(GameDefinition gameDefinition)
        {
            this.gameDefinition = gameDefinition;

            if (gameDefinition != null)
            {
                Width = gameDefinition.Width;
                Height = gameDefinition.Height;
                Tiles = new TileData[gameDefinition.Width, gameDefinition.Height];
                DefinitionUniqueId = gameDefinition.UniqueId;
            }
        }

        public TileData[,] Tiles { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public string DefinitionUniqueId
        {
            get { return _definitionUniqueId; }
            set
            {
                _definitionUniqueId = value;
                if (gameDefinition == null)
                    gameDefinition = AsyncHelper.RunSync(() => GameDefinitionSource.GetGameDefinition(value));
            }
        }

        public void ForEachTile(Action<TileData> action)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Tiles[x, y] != null)
                        action(Tiles[x, y]);
                }
            }
        }

        public void Setup()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tiles[x, y] = null;
                }
            }
        }


        public XY RandomAvailablePosition()
        {
            List<XY> available = AvailablePositions();
            if (available.Count <= 0) return null;

            int index = RandomManager.Next(available.Count);
            return available[index];
        }

        public List<XY> AvailablePositions()
        {
            var list = new List<XY>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Tiles[x, y] == null)
                        list.Add(new XY(x, y));
                }
            }
            return list;
        }

        public int AvailablePositionCount()
        {
            return AvailablePositions().Count;
        }

        public TileData InsertTile(XY pos, int value)
        {
            var tile = new TileData(pos, value, gameDefinition);
            Tiles[pos.X, pos.Y] = tile;
            return tile;
        }

        public void InsertTile(TileData tile)
        {
            Tiles[tile.Pos.X, tile.Pos.Y] = tile;
        }

        public void RemoveTile(XY pos)
        {
            Tiles[pos.X, pos.Y] = null;
        }

        public void MoveTile(TileData tile, XY pos)
        {
            Tiles[tile.Pos.X, tile.Pos.Y] = null;
            Tiles[pos.X, pos.Y] = tile;
            tile.Pos = pos;
        }

        public bool WithinBounds(XY pos)
        {
            return ((pos.X >= 0 && pos.X < Width) &&
                    (pos.Y >= 0 && pos.Y < Height));
        }

        public bool PosAvailable(XY pos)
        {
            return Tiles[pos.X, pos.Y] == null;
        }

        public TileData TileAt(XY pos)
        {
            if (WithinBounds(pos))
                return Tiles[pos.X, pos.Y];
            return null;
        }
    }
}