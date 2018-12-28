// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=321224
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Brain.Animate;
using Brain.Extensions;
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
        private Canvas HowToGameCanvas;
        private bool _repeating;

        public AboutPage()
        {
            InitializeComponent();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            Games = new ObservableCollection<GameData>();
        }

        private Button LikeUsButton
        {
            get { return this.Child<Button>("LikeUsButton"); }
        }

        private Button DontLikeUsButton
        {
            get { return this.Child<Button>("DontLikeUsButton"); }
        }

        private Button DonateButton
        {
            get { return this.Child<Button>("DonateButton"); }
        }

        private Button PleaseRateButton
        {
            get { return this.Child<Button>("PleaseRateButton"); }
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
                    // Do Nothing
                    Debug.WriteLine(ex.ToString());
                }
            });

            //AnimateHowToRepeat();
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


        private async void PleaseRateButton_OnClick(object sender, RoutedEventArgs e)
        {
            storage.Set(true, "Reviewed");

            await Launcher.LaunchUriAsync(new Uri(
                String.Format("ms-windows-store:Review?PFN={0}", Package.Current.Id.FamilyName)));
        }

        private void DontLikeUsButton_OnClick(object sender, RoutedEventArgs e)
        {
            //EasyTracker.GetTracker().SendEvent("Like", "No", null, 0);

            LikeUsButton.AnimateAsync(new BounceOutDownAnimation());
            DontLikeUsButton.AnimateAsync(new BounceOutAnimation());
        }

        private async void DonateButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //EasyTracker.GetTracker().SendEvent("Like", "Donate", null, 0);

                string xml = await CurrentApp.RequestProductPurchaseAsync(DonationProductId, false);
                ApplicationData.Current.RoamingSettings.Values["HasDonated"] = true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Donation failed to process: ");
            }
        }

        private async void LikeUsButton_OnClick(object sender, RoutedEventArgs e)
        {
            //EasyTracker.GetTracker().SendEvent("Like", "Yes", null, 0);

            await Task.WhenAll(new Task[]
            {
                LikeUsButton.AnimateAsync(new TadaAnimation()),
                DontLikeUsButton.AnimateAsync(new BounceOutDownAnimation())
            });
            await LikeUsButton.AnimateAsync(new BounceOutDownAnimation());

            await Task.WhenAll(new Task[]
            {
                DonateButton.AnimateAsync(new BounceInUpAnimation()),
                PleaseRateButton.AnimateAsync(new BounceInUpAnimation {Delay = 0.1})
            });
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
            Canvas howToGameCanvas = HowToGameCanvas;

            var tileDataTopLeft = new TileData(new XY(2, 0), 2, demoGameDefinition);
            var tileTopLeft = new TileControl {TileData = tileDataTopLeft, Width = 80, Height = 80};
            howToGameCanvas.Children.Add(tileTopLeft);
            await tileTopLeft.MoveToAsync(0.0, new Point(0, 0));

            var tileDataMiddle = new TileData(new XY(1, 1), 2, demoGameDefinition);
            var tileMiddle = new TileControl {TileData = tileDataMiddle, Width = 80, Height = 80};
            howToGameCanvas.Children.Add(tileMiddle);
            await tileMiddle.MoveToAsync(0.0, new Point(0, 80));

            var tileDataMiddleRight = new TileData(new XY(2, 1), 4, demoGameDefinition);
            var tileMiddleRight = new TileControl {TileData = tileDataMiddleRight, Width = 80, Height = 80};
            howToGameCanvas.Children.Add(tileMiddleRight);
            await tileMiddleRight.MoveToAsync(0.0, new Point(160, 80));

            var tileDataBottom = new TileData(new XY(2, 2), 2, demoGameDefinition);
            var tileBottom = new TileControl {TileData = tileDataBottom, Width = 80, Height = 80};
            howToGameCanvas.Children.Add(tileBottom);
            await tileBottom.MoveToAsync(0.0, new Point(0, 160));

            var tileDataBottomRight = new TileData(new XY(2, 2), 2, demoGameDefinition);
            var tileBottomRight = new TileControl {TileData = tileDataBottomRight, Width = 80, Height = 80};
            tileDataBottomRight.MergedFrom = tileDataBottom;
            howToGameCanvas.Children.Add(tileBottomRight);
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
            howToGameCanvas.Children.Remove(tileBottomRight);

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


        private void HowToGameCanvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            HowToGameCanvas = sender as Canvas;
            AnimateHowToRepeat();
        }
    }
}