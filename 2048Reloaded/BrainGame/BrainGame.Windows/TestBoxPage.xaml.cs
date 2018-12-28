// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using BrainGame.Controls;

namespace BrainGame
{
    /// <summary>
    ///     A page that displays details for a single item within a group while allowing gestures to
    ///     flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class TestBoxPage
    {
        public TestBoxPage()
        {
            InitializeComponent();
        }

        private async void Boxtana_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            await Boxtana.Do(BoxtanaAction.Exit);
            await Task.Delay(500);
            await Boxtana.Do(BoxtanaAction.Entrance);
        }

        private async void One_OnClick(object sender, RoutedEventArgs e)
        {
            await Boxtana.Do(BoxtanaAction.Swing);
        }

        private async void Two_OnClick(object sender, RoutedEventArgs e)
        {
            await Boxtana.Do(BoxtanaAction.RotateRight);
        }

        private async void Three_OnClick(object sender, RoutedEventArgs e)
        {
            await Boxtana.Do(BoxtanaAction.Color);
        }
    }
}