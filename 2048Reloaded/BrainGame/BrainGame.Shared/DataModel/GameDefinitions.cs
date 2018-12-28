using System.Collections.Generic;

namespace BrainGame.DataModel
{
    public class GameDefinitions
    {
        public List<GameDefinition> Games { get; set; }
    }


    public class GameDefinition
    {
        public string UniqueId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Style { get; set; }
    }
}