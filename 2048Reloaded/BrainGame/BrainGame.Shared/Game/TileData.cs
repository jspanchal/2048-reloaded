using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using BrainGame.DataModel;
using Newtonsoft.Json;
using PropertyChanged;

namespace BrainGame.Game
{
    [ImplementPropertyChanged]
    public class TileData
    {
        private GameDefinition _gameDefinition;

        public TileData(XY pos, int value, GameDefinition gameDefinition)
        {
            Pos = pos;
            Value = value;
            GameDefinition = gameDefinition;
        }

        public GameDefinition GameDefinition { get; set; }

        [AlsoNotifyFor("BackgroundBrush", "ForegroundBrush", "DisplayValue")]
        public int Value { get; set; }

        public string DisplayValue
        {
            get
            {
                if (GameDefinition.Style == "ABC")
                {
                    switch (Value)
                    {
                        case 2:
                            return "A";
                        case 4:
                            return "B";
                        case 8:
                            return "C";
                        case 16:
                            return "D";
                        case 32:
                            return "E";
                        case 64:
                            return "F";
                        case 128:
                            return "G";
                        case 256:
                            return "H";
                        case 512:
                            return "I";
                        case 1024:
                            return "J";
                        case 2048:
                            return "K";
                        case 4096:
                            return "L";
                        case 8192:
                            return "M";
                        case 16384:
                            return "N";
                        case 32768:
                            return "O";
                        case 65536:
                            return "P";
                        case 131072:
                            return "Q";
                        default:
                            break;
                    }
                }
                return Value.ToString();
            }
        }

        public XY Pos { get; set; }
        public XY PreviousPos { get; set; }

        [JsonIgnore]
        public TileData MergedFrom { get; set; }

        [JsonIgnore]
        public Brush BackgroundBrush
        {
            get { return Application.Current.Resources["TileBackgroundBrush" + Value] as Brush; }
        }

        [JsonIgnore]
        public Brush ForegroundBrush
        {
            get { return Application.Current.Resources["TileForegroundBrush" + Value] as Brush; }
        }
    }
}