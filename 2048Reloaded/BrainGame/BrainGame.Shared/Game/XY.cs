using System.Diagnostics;

namespace BrainGame.Game
{
    [DebuggerDisplay("X: {X}, Y: {Y}")]
    public class XY
    {
        public XY(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}