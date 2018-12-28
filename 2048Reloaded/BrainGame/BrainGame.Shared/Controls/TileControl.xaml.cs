using Windows.UI.Xaml.Controls;
using BrainGame.Game;

namespace BrainGame.Controls
{
    public sealed partial class TileControl : UserControl
    {
        public TileControl()
        {
            InitializeComponent();
        }

        public TileData TileData
        {
            get { return DataContext as TileData; }
            set { DataContext = value; }
        }
    }
}