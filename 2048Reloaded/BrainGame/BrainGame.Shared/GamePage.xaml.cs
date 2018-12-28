#if WINDOWS_APP

// Windows phone not ready yet
#define GOOGLE_ANALYTICS
using GoogleAnalytics;

#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Store;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Brain.Animate;
using BrainGame.DataModel;
using BrainGame.Game;
using PropertyChanged;

namespace BrainGame
{
    [ImplementPropertyChanged]
    public partial class GamePage
    {
        private string ShareMessage;

        public GamePage()
        {
            InitializeComponent();

#if WINDOWS_APP
            SizeChanged += (sender, args) =>
            {
                if (pageRoot.ActualWidth < 500)
                    BackButton.Style = Application.Current.Resources["BackButtonSmallStyle"] as Style;
                else
                    BackButton.Style = Application.Current.Resources["BackButtonNormalStyle"] as Style;
            };
#endif
        }

        public int TotalScore { get; set; }
        public int BestScore { get; set; }

        public bool AllGamesOver { get; set; }
        public bool ShowShare { get; set; }

        public bool ShowTrophy { get; set; }
        public string AchievementTitle { get; set; }
        public string AchievementText { get; set; }

        public string Rank { get; set; }

        private void GameOnTileMoved(object sender, TileData tileData)
        {
            TotalScore = bc.Game.Score;
            BestScore = bc.Game.GameData.BestScore;
        }

        private void GameOnNewAchievement(object sender, int value)
        {
            if (value > 8)
            {
                Rank = BinaryGame.GetRank(bc.Game.GameData.BestPiece);
                ShowAchievement(Rank, "NEW ACHIEVEMENT", true);
            }
        }

        private void GameOnNewHighScore(object sender, int score)
        {
            BestScore = score;
        }


        private async void ShowAchievement(string content, string title = null, bool showTrophy = false)
        {
            ShowTrophy = showTrophy;
            AchievementTitle = title;
            AchievementText = content;

            await AchievementBalloon.AnimateAsync(new BounceInUpAnimation());

            await Task.Delay(3000);

            await AchievementBalloon.AnimateAsync(new BounceOutDownAnimation());
        }

        private void GameOnGameWon(object sender, EventArgs eventArgs)
        {
            //GameMessage = "You ROCK!";
        }

        private void GameOnGameOver(object sender, EventArgs eventArgs)
        {
            AllGamesOver = (bc.Game.IsGameOver);
            if (!AllGamesOver) return;

            ShowAchievement("No more moves", "GAME OVER");

#if GOOGLE_ANALYTICS

            EasyTracker.GetTracker().SendEvent("GameOver" + bc.Game.GameDefinition.UniqueId, "Score", null, TotalScore);
            EasyTracker.GetTracker().SendEvent("GameOver" + bc.Game.GameDefinition.UniqueId, "Moves", null, bc.Game.Moves);
            if (TotalScore == bc.Game.GameData.BestScore)
                EasyTracker.GetTracker().SendEvent("BestScore" + bc.Game.GameDefinition.UniqueId, "BestScore", null, bc.Game.GameData.BestScore);
#endif

            ShowShare = true;
            PlayGrid.IsHitTestVisible = true;
            PlayGrid.AnimateAsync(new FadeInAnimation());
            PlayButton.IsHitTestVisible = true;
            PlayButton.AnimateAsync(new BounceInUpAnimation());

            bc.SaveData();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (bc.Game == null)
            {
                List<GameDefinition> games = await GameDefinitionSource.LoadDataAsync();
                GameDefinition gameDefinition = games.FirstOrDefault(g => g.UniqueId == (string) e.Parameter);
                if (gameDefinition == null) return;

                bc.Build(gameDefinition);

                bc.Game.TileMoved += GameOnTileMoved;
                bc.NewHighScore += GameOnNewHighScore;
                bc.NewAchievement += GameOnNewAchievement;
                bc.Game.GameWon += GameOnGameWon;
                bc.Game.GameOver += GameOnGameOver;
            }

            if (TotalScore == 0)
            {
                bc.LoadData();
                TotalScore = bc.Game.Score;
                BestScore = bc.Game.GameData.BestScore;
                Rank = BinaryGame.GetRank(bc.Game.GameData.BestPiece);
            }
            Rank = BinaryGame.GetRank(bc.Game.GameData.BestPiece);

#if GOOGLE_ANALYTICS
            EasyTracker.GetTracker().SendView("Main" + bc.Game.GameDefinition.UniqueId);
#endif

            if (TotalScore == 0)
            {
                PlayGrid.AnimateAsync(new FadeInAnimation {Delay = 0.3});
                PlayButton.AnimateAsync(new BounceInUpAnimation {Delay = 0.4});

                PlayGrid.IsHitTestVisible = true;
                PlayButton.IsHitTestVisible = true;
            }
            else
            {
                PlayGrid.Opacity = 0;
                PlayGrid.IsHitTestVisible = false;
                PlayButton.Opacity = 0;
                PlayButton.IsHitTestVisible = false;
            }

            DataTransferManager.GetForCurrentView().DataRequested += DataTransferManagerOnDataRequested;

            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            Window.Current.VisibilityChanged += OnVisibilityChanged;
        }

        private void OnVisibilityChanged(object sender, VisibilityChangedEventArgs args)
        {
            if (!args.Visible)
                bc.SaveData();
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Window.Current.VisibilityChanged -= OnVisibilityChanged;
            DataTransferManager.GetForCurrentView().DataRequested -= DataTransferManagerOnDataRequested;
            Window.Current.CoreWindow.KeyUp -= CoreWindow_KeyUp;

            bc.SaveData();

            base.OnNavigatedFrom(e);
        }


        private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (AllGamesOver) return;

            double x = e.Cumulative.Translation.X;
            double y = e.Cumulative.Translation.Y;

            if (Math.Abs(x) > Math.Abs(y))
            {
                if (x < -20)
                    Move(MoveType.Left);
                else if (x > 20)
                    Move(MoveType.Right);
            }
            else
            {
                if (y < -20)
                    Move(MoveType.Up);
                else if (y > 20)
                    Move(MoveType.Down);
            }
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs e)
        {
            switch (e.VirtualKey)
            {
                case VirtualKey.Left:
                    Move(MoveType.Left);
                    break;
                case VirtualKey.Right:
                    Move(MoveType.Right);
                    break;
                case VirtualKey.Up:
                    Move(MoveType.Up);
                    break;
                case VirtualKey.Down:
                    Move(MoveType.Down);
                    break;
            }
        }

        private void Move(MoveType moveType)
        {
            bc.Game.Move(moveType);
        }


        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            ShareMessage = string.Format("OMG! I just scored {0} in Binary {1}. (Rank: {2}) #BrainOffline #Binary",
                TotalScore, bc.Game.GameDefinition.Title, Rank);

            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManagerOnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataPackage data = args.Request.Data;
            data.Properties.Title = "binary";
            data.Properties.Description = "share with the world how awesome you are";
#if DEBUG
            var uri = CurrentAppSimulator.LinkUri;
#else
            Uri uri = CurrentApp.LinkUri;
#endif
            data.SetApplicationLink(uri);

            if (!string.IsNullOrWhiteSpace(ShareMessage))
                data.SetText(ShareMessage + "\n" + uri);
            else
                data.SetText(
                    string.Format("I am awesome at binary. High Score: {0} (Rank: {1}) #BrainOffline #Binary\n {2}",
                        bc.Game.GameData.BestScore, Rank, uri));
        }


        private async void PlayButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            await PlayGrid.AnimateAsync(new FadeOutAnimation());
            PlayGrid.IsHitTestVisible = false;

#if GOOGLE_ANALYTICS
            EasyTracker.GetTracker().SendEvent("GameStart" + bc.Game.GameDefinition.UniqueId, "Start", null, 0);
#endif

            await bc.Clear();
            bc.Game.Setup();

            TotalScore = 0;
            AllGamesOver = false;
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            PlayButton_Click(sender, null);
        }
    }
}