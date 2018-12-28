using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Brain.Animate;
using Brain.Animate.Extensions;
using Brain.Extensions;
using BrainGame.AnimationDefinitions;
using BrainGame.Controls;
using BrainGame.DataModel;

namespace BrainGame
{
    public sealed partial class HubPage
    {
        private bool triggered;

        public HubPage()
        {
            InitializeComponent();

            Init();

            Boxtana.Hide();
            Loaded += async (sender, args) =>
            {
                await Task.Delay(800);
                if (!triggered) await Boxtana.Do(BoxtanaAction.Entrance);
                if (!triggered) await Task.Delay(500);
                if (triggered) return;
                while (!triggered)
                {
                    await Boxtana.Do(BoxtanaAction.RandomWait);
                }
            };
        }

        public ObservableCollection<GameDefinition> Games { get; set; }

        public void Init()
        {
            Games = new ObservableCollection<GameDefinition>();
            Loaded += async (sender, args) =>
            {
                if (Games.Count > 0) return;

                foreach (GameDefinition game in await GameDefinitionSource.LoadDataAsync())
                    Games.Add(game);
            };
        }

        private async void GamesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var gameDefinition = ((GameDefinition) e.ClickedItem);
            triggered = true;
            var gamesGridView = this.Child<GridView>("GamesGridView");

            await Task.WhenAll(new[]
            {
                AnimationTrigger.AnimateClose(),
                Boxtana.Do(BoxtanaAction.Exit),
                gamesGridView.AnimateItems(
                    new BounceOutDownAnimation(), 0.05,
                    gameDefinition,
                    new ExpandAnimation {Duration = 0.8, FinalScale = 1.1})
            });

            Frame.Navigate(typeof (GamePage), gameDefinition.UniqueId);
        }

        private async void Boxtana_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Boxtana.Do(BoxtanaAction.Exit);
            Frame.Navigate(typeof (AboutPage));
        }
    }
}