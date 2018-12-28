using PropertyChanged;

namespace BrainGame.Game
{
    [ImplementPropertyChanged]
    public class GameData
    {
        public int BestScore { get; set; }
        public int BestPiece { get; set; }

        public int MoveCount { get; set; }
        public int GameCount { get; set; }

        public string Description { get; set; }
        public string Rank { get; set; }

        public void Reset()
        {
            BestScore = 0;
            BestPiece = 0;
        }
    }
}