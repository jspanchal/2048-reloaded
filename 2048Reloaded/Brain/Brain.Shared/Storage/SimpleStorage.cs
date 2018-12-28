using Windows.Storage;
using Newtonsoft.Json;

namespace Brain.Storage
{
    public class SimpleStorage : ISimpleStorage
    {
        public T GetSettings<T>(string name)
        {
            var values = ApplicationData.Current.RoamingSettings.Values;
            if (values.ContainsKey(name))
            {
                var json = values[name] as string;
                return JsonConvert.DeserializeObject<T>(json);
            }
            return default(T);
        }

        public void SaveSettings<T>(T settings, string name)
        {
            var json = JsonConvert.SerializeObject(settings);
            ApplicationData.Current.RoamingSettings.Values[name] = json;
        }

        public void DeleteSetting(string name)
        {
            ApplicationData.Current.RoamingSettings.Values.Remove(name);
        }
    }
}
