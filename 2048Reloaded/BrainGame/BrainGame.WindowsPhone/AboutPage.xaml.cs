// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Brain.Animate;
using Brain.Storage;
using Brain.Utils;
using BrainGame.Controls;
using BrainGame.DataModel;
using BrainGame.Game;
using PropertyChanged;

namespace BrainGame
{
    [ImplementPropertyChanged]
    public sealed partial class AboutPage
    {
        private const string DonationProductId = "Donation2";
        private readonly GameDefinition demoGameDefinition = new GameDefinition {Width = 3, Height = 3};
        private readonly IStorage storage = new SimpleStorage();
        private bool _repeating;

        public AboutPage()
        {
            InitializeComponent();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            Games = new ObservableCollection<GameData>();
        }

        public string DonationAmount { get; set; }
        public string DonationText { get; set; }
        public bool HasDonated { get; set; }

        public ObservableCollection<GameData> Games { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadData();

            DonationText = "Donate";

            Task.Run(async () =>
            {
                try
                {
#if DEBUG
                    var productLicense = CurrentAppSimulator.LicenseInformation.ProductLicenses[DonationProductId];
#else
                    ProductLicense productLicense = CurrentApp.LicenseInformation.ProductLicenses[DonationProductId];
#endif
                    if (productLicense != null)
                    {
                        ApplicationData.Current.RoamingSettings.Values["HasDonated"] = productLicense.IsActive;
                        HasDonated = productLicense.IsActive;
                    }

#if DEBUG
                    var listingInformation = await CurrentAppSimulator.LoadListingInformationAsync();
#else
                    ListingInformation listingInformation = await CurrentApp.LoadListingInformationAsync();
#endif
                    if (listingInformation != null)
                    {
                        ProductListing productListing = listingInformation.ProductListings[DonationProductId];
                        if (productListing != null)
                        {
                            await Execute.OnUIThread(() =>
                            {
                                DonationAmount = productListing.FormattedPrice;
                                DonationText = "Donate " + DonationAmount;
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    string y = ex.ToString();
                    // Do Nothing
                }
            });

            AnimateHowToRepeat();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            _repeating = false;
        }

        private async void LoadData()
        {
            Games.Clear();
            List<GameDefinition> games = await GameDefinitionSource.LoadDataAsync();
            foreach (GameDefinition gameDefinition in games)
            {
                GameData gameData = storage.Get<GameData>("GameData." + gameDefinition.UniqueId) ?? new GameData();
                gameData.Description = gameDefinition.Title;
                gameData.Rank = BinaryGame.GetRank(gameData.BestPiece);
                Games.Add(gameData);
            }
        }


        private async void ResetAllScores_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult answer = await MessageBox.ShowAsync(
                "Are you sure you want to clear all scores and achievements?", "Reset Scores", MessageBoxButton.OKCancel);
            if (answer != MessageBoxResult.OK) return;

            List<GameDefinition> games = await GameDefinitionSource.LoadDataAsync();
            foreach (GameDefinition gameDefinition in games)
                storage.Delete("GameData." + gameDefinition.UniqueId);

            LoadData();
        }


        private async void LikeUsButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(CurrentApp.LinkUri);
        }

        private async void DontLikeUsButton_OnClick(object sender, RoutedEventArgs e)
        {
            //EasyTracker.GetTracker().SendEvent("Like", "No", null, 0);
            EmailRecipient sendTo = new EmailRecipient()
            {
                Address = "jspanchal@hotmail.com"
            };
            EmailMessage mail = new EmailMessage();
            mail.Subject = "2048 Reloaded Feedback";
            mail.To.Add(sendTo);
            await EmailManager.ShowComposeNewEmailAsync(mail);
        }

      
        private async void AnimateHowToRepeat()
        {
            _repeating = true;
            while (_repeating)
            {
                await AnimateHowTo();
                HowToGameCanvas.Children.Clear();
            }
        }

        private async Task AnimateHowTo()
        {
            var tileDataTopLeft = new TileData(new XY(2, 0), 2, demoGameDefinition);
            var tileTopLeft = new TileControl {TileData = tileDataTopLeft, Width = 80, Height = 80};
            HowToGameCanvas.Children.Add(tileTopLeft);
            await tileTopLeft.MoveToAsync(0.0, new Point(0, 0));

            var tileDataMiddle = new TileData(new XY(1, 1), 2, demoGameDefinition);
            var tileMiddle = new TileControl {TileData = tileDataMiddle, Width = 80, Height = 80};
            HowToGameCanvas.Children.Add(tileMiddle);
            await tileMiddle.MoveToAsync(0.0, new Point(0, 80));

            var tileDataMiddleRight = new TileData(new XY(2, 1), 4, demoGameDefinition);
            var tileMiddleRight = new TileControl {TileData = tileDataMiddleRight, Width = 80, Height = 80};
            HowToGameCanvas.Children.Add(tileMiddleRight);
            await tileMiddleRight.MoveToAsync(0.0, new Point(160, 80));

            var tileDataBottom = new TileData(new XY(2, 2), 2, demoGameDefinition);
            var tileBottom = new TileControl {TileData = tileDataBottom, Width = 80, Height = 80};
            HowToGameCanvas.Children.Add(tileBottom);
            await tileBottom.MoveToAsync(0.0, new Point(0, 160));

            var tileDataBottomRight = new TileData(new XY(2, 2), 2, demoGameDefinition);
            var tileBottomRight = new TileControl {TileData = tileDataBottomRight, Width = 80, Height = 80};
            tileDataBottomRight.MergedFrom = tileDataBottom;
            HowToGameCanvas.Children.Add(tileBottomRight);
            await tileBottomRight.MoveToAsync(0.0, new Point(160, 160));


            await Task.Delay(2000);
            tileDataBottom.Value *= 2;
            await Task.WhenAll(new[]
            {
                TileMove(tileTopLeft, tileDataTopLeft.Pos.X, tileDataTopLeft.Pos.Y, false),
                TileMove(tileMiddle, tileDataMiddle.Pos.X, tileDataMiddle.Pos.Y, false),
                TileMove(tileBottom, tileDataBottom.Pos.X, tileDataBottom.Pos.Y, true),
                tileBottomRight.AnimateAsync(new BounceOutAnimation {Amplitude = 0.4, Duration = 0.2})
            });
            await Task.Delay(1000);
            HowToGameCanvas.Children.Remove(tileBottomRight);

            tileDataTopLeft.Pos = new XY(2, 1);
            tileDataMiddle.Pos = new XY(1, 2);
            tileDataMiddleRight.Value *= 2;
            tileDataMiddleRight.Pos = new XY(2, 2);
            await Task.WhenAll(new[]
            {
                TileMove(tileTopLeft, tileDataTopLeft.Pos.X, tileDataTopLeft.Pos.Y, false),
                TileMove(tileMiddle, tileDataMiddle.Pos.X, tileDataMiddle.Pos.Y, false),
                TileMove(tileMiddleRight, tileDataMiddleRight.Pos.X, tileDataMiddleRight.Pos.Y, true),
                tileBottom.AnimateAsync(new BounceOutAnimation {Amplitude = 0.4, Duration = 0.2})
            });

            await Task.Delay(1000);
        }

        private async Task TileMove(TileControl tile, int x, int y, bool hasMerged)
        {
            double width = (tile.ActualWidth <= 0) ? 80 : tile.ActualWidth;
            double height = (tile.ActualHeight <= 0) ? 80 : tile.ActualHeight;

            if (hasMerged)
            {
                await Task.WhenAll(new Task[]
                {
                    tile.MoveToAsync(0.3, new Point(x*width, y*height), new BackEase {Amplitude = 0.4}),
                    tile.AnimateAsync(new PulseAnimation {Duration = 0.4})
                });
            }
            else
                await tile.MoveToAsync(0.3, new Point(x*width, y*height), new BackEase {Amplitude = 0.4});
        }
    }
}