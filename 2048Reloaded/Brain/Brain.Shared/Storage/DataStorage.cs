using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Brain.Utils;
using Newtonsoft.Json;

namespace Brain.Storage
{
    public class DataStorage : IDataStorage
    {
        public T Get<T>(string key)
        {
            var result = AsyncHelper.RunSync(() => GetAsync<T>(key));
            return result;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var folder = ApplicationData.Current.RoamingFolder;

            try
            {
                var f = await folder.GetItemAsync(key) as StorageFile;
                if (f == null)
                    return default(T);

                using (var s = await f.OpenAsync(FileAccessMode.Read))
                {
                    using (var sr = new StreamReader(s.AsStreamForRead()))
                    {
                        var json = sr.ReadToEnd();
                        return JsonConvert.DeserializeObject<T>(json);
                    }
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public void Save<T>(T value, string key)
        {
            AsyncHelper.RunSync(() => SaveAsync(value, key));
        }

        public async Task SaveAsync<T>(T value, string key)
        {
            var folder = ApplicationData.Current.RoamingFolder;

            try
            {
                var f = await folder.GetItemAsync(key) as StorageFile;
                if (f == null)
                    return;

                using (var s = await f.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (var wr = new StreamWriter(s.AsStreamForWrite()))
                    {
                        await wr.WriteLineAsync(JsonConvert.SerializeObject(value));
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void Delete(string key)
        {
            AsyncHelper.RunSync(() => DeleteAsync(key));
        }

        public async Task DeleteAsync(string key)
        {
            var folder = ApplicationData.Current.RoamingFolder;

            try
            {
                var f = await folder.GetItemAsync(key) as StorageFile;
                if (f == null)
                    return;

                await f.DeleteAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}
