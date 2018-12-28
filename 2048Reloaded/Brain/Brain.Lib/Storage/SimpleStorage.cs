using Windows.Foundation.Collections;
using Windows.Storage;
using Newtonsoft.Json;

namespace Brain.Storage
{
    public class SimpleStorage : IStorage
    {
        public T Get<T>(string name)
        {
            IPropertySet values = ApplicationData.Current.RoamingSettings.Values;
            if (values.ContainsKey(name))
            {
                var json = values[name] as string;
                return JsonConvert.DeserializeObject<T>(json);
            }
            return default(T);
        }

        public void Set<T>(T settings, string name)
        {
            var json = JsonConvert.SerializeObject(settings);
            ApplicationData.Current.RoamingSettings.Values[name] = json;
        }

        public void Delete(string name)
        {
            ApplicationData.Current.RoamingSettings.Values.Remove(name);
        }
    }
}