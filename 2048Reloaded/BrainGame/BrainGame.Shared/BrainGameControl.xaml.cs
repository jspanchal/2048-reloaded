using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Brain.Animate;
using Brain.Storage;
using BrainGame.Controls;
using BrainGame.DataModel;
using BrainGame.Game;
using PropertyChanged;

namespace BrainGame
{
    [ImplementPropertyChanged]
    public partial class BrainGameControl
    {
        private const int DefaultTileWidth = 30;
        private const int DefaultTileHeight = 30;
        private readonly IStorage storage = new SimpleStorage();

        public BrainGameControl()
        {
            InitializeComponent();

            SizeChanged += OnSizeChanged;
        }

        private double TileWidth
        {
            get
            {
                if (GameCanvas.ActualWidth <= 0)
                    return DefaultTileWidth;
                return GameCanvas.ActualWidth/Game.BinaryGrid.Width;
            }
        }

        private double TileHeight
        {
            get
            {
                if (GameCanvas.ActualHeight <= 0)
                    return DefaultTileHeight;
                return GameCanvas.ActualHeight/Game.BinaryGrid.Height;
            }
        }

        public BinaryGame Game
        {
            get { return DataContext as BinaryGame; }
            set
            {
                DataContext = value;

                Game.TileAdded += GameOnTileAdded;
                Game.TileMoved += GameOnTileMoved;
                Game.TileRemoved += GameOnTileRemoved;
                Game.GameWon += GameOnGameWon;
                Game.GameOver += GameOnGameOver;
            }
        }

        private async void GameOnTileAdded(object sender, TileData tileData)
        {
            var tileControl = new TileControl
            {
                TileData = tileData,
                Width = TileWidth,
                Height = TileHeight
            };

            Canvas.SetZIndex(tileControl, tileData.Value);
            GameCanvas.Children.Add(tileControl);
            await tileControl.MoveToAsync(
                0.0, new Point((GameCanvas.Width/2) - (TileWidth/2), -200));
            await TileMove(tileControl, tileData.Pos.X, tileData.Pos.Y, false);
        }

        private async void GameOnTileMoved(object sender, TileData tileData)
        {
            bool hasMerged = (tileData.MergedFrom != null);
            TileControl tileControl = FindTile(tileData);
            if (tileControl != null)
            {
                Canvas.SetZIndex(tileControl, tileData.Value);
                await TileMove(tileControl, tileData.Pos.X, tileData.Pos.Y, hasMerged);
            }

            if (Game.Score > Game.GameData.BestScore)
            {
                Game.GameData.BestScore = Game.Score;
                RaiseNewHighScore(Game.Score);
            }

            if (tileData.Value > Game.GameData.BestPiece)
            {
                Game.GameData.BestPiece = tileData.Value;
                RaiseNewAchievement(tileData.Value);
                SaveData();
            }
        }

        private async void GameOnTileRemoved(object sender, TileData tileData)
        {
            TileControl tileControl = FindTile(tileData);
            if (tileControl != null)
            {
                Canvas.SetZIndex(tileControl, 2050);
                await tileControl.AnimateAsync(new BounceOutAnimation {Amplitude = 0.4, Duration = 0.2});
                GameCanvas.Children.Remove(tileControl);
            }
        }

        private void GameOnGameWon(object sender, EventArgs e)
        {
        }

        private void GameOnGameOver(object sender, EventArgs e)
        {
        }

        private async Task TileMove(TileControl tile, int x, int y, bool hasMerged)
        {
            //var width = (tile.ActualWidth == DefaultTileWidth) ? TileWidth : tile.ActualWidth;
            //var height = (tile.ActualHeight <= 0) ? TileHeight : tile.ActualHeight;

            if (hasMerged)
            {
                await Task.WhenAll(new Task[]
                {
                    tile.MoveToAsync(0.3, new Point(x*TileWidth, y*TileHeight), new BackEase {Amplitude = 0.4}),
                    tile.AnimateAsync(new PulseAnimation {Duration = 0.4})
                });
            }
            else
                await tile.MoveToAsync(0.3, new Point(x*TileWidth, y*TileHeight), new BackEase {Amplitude = 0.4});
        }

        private TileControl FindTile(TileData tile)
        {
            return GameCanvas.Children.OfType<TileControl>().First(x => x.TileData == tile);
        }

        public void Build(GameDefinition gameDefinition)
        {
            Game = new BinaryGame(gameDefinition);

            TileBackgroundGrid.RowDefinitions.Clear();
            TileBackgroundGrid.ColumnDefinitions.Clear();

            var style = Application.Current.Resources["EmptyBorder"] as Style;

            for (int x = 0; x < Game.BinaryGrid.Width; x++)
                TileBackgroundGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int y = 0; y < Game.BinaryGrid.Height; y++)
                TileBackgroundGrid.RowDefinitions.Add(new RowDefinition());

            for (int x = 0; x < Game.BinaryGrid.Width; x++)
            {
                for (int y = 0; y < Game.BinaryGrid.Height; y++)
                {
                    var border = new Border {Style = style};

                    Grid.SetColumn(border, x);
                    Grid.SetRow(border, y);
                    TileBackgroundGrid.Children.Add(border);
                }
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            foreach (TileControl tileControl in GameCanvas.Children.OfType<TileControl>())
            {
                tileControl.Width = TileWidth;
                tileControl.Height = TileHeight;
                TileMove(tileControl, tileControl.TileData.Pos.X, tileControl.TileData.Pos.Y, false);
            }
        }

        public async Task Clear()
        {
            var animations = new List<Task>();
            int i = 0;
            foreach (TileControl tileControl in GameCanvas.Children.OfType<TileControl>())
                animations.Add(
                    tileControl.AnimateAsync(
                        new BounceOutAnimation {Amplitude = 0.4, Duration = 0.4, Delay = i++*0.05}));
            await Task.WhenAll(animations);

            foreach (TileControl tileControl in GameCanvas.Children.OfType<TileControl>().ToList())
                GameCanvas.Children.Remove(tileControl);
        }

        public void LoadData()
        {
            GameData GameData = storage.Get<GameData>("GameData." + Game.GameDefinition.UniqueId) ?? new GameData();
            Game.GameData = GameData;

            var savedGame = storage.Get<SavedGame>("SavedGame." + Game.GameDefinition.UniqueId);
            if (savedGame != null)
            {
                Game.Score = savedGame.Score1;

                for (int x = 0; x < Game.GameDefinition.Width; x++)
                {
                    for (int y = 0; y < Game.GameDefinition.Height; y++)
                    {
                        Game.RestoreTile(savedGame.BinaryGrid1.Tiles[x, y], Game.GameDefinition);
                    }
                }
            }
        }

        public void SaveData()
        {
            storage.Set(Game.GameData, "GameData." + Game.GameDefinition.UniqueId);

            if (!Game.IsGameOver && Game.Score > 0)
            {
                var savedGame = new SavedGame
                {
                    Score1 = Game.Score,
                    BinaryGrid1 = Game.BinaryGrid,
                };
                storage.Set(savedGame, "SavedGame." + Game.GameDefinition.UniqueId);
            }
            else
                storage.Delete("SavedGame." + Game.GameDefinition.UniqueId);
        }


        public event EventHandler<int> NewHighScore;

        protected virtual void RaiseNewHighScore(int highScore)
        {
            EventHandler<int> handler = NewHighScore;
            if (handler != null) handler(this, highScore);
        }

        public event EventHandler<int> NewAchievement;

        protected virtual void RaiseNewAchievement(int achievement)
        {
            EventHandler<int> handler = NewAchievement;
            if (handler != null) handler(this, achievement);
        }
    }
}