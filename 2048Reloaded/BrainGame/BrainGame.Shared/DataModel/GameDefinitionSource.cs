using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace BrainGame.DataModel
{
    public class GameDefinitionSource
    {
        private static List<GameDefinition> Games = new List<GameDefinition>();

        public static async Task<GameDefinition> GetGameDefinition(string id)
        {
            return (await LoadDataAsync()).FirstOrDefault(g => g.UniqueId == id);
        }

        public static async Task<List<GameDefinition>> LoadDataAsync()
        {
            if (Games.Count > 0) return Games;

            StorageFile file =
                await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///DataModel/GameDefinitions.json"));
            string json = await FileIO.ReadTextAsync(file);

            var gameDefinitions = JsonConvert.DeserializeObject<GameDefinitions>(json);
            Games = gameDefinitions.Games;

            return Games;
        }
    }
}