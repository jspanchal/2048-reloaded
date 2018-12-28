using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Brain.Utils;
using Newtonsoft.Json;

namespace Brain.Storage
{
    public class DataStorage : IStorage
    {
        public T Get<T>(string key)
        {
            T result = AsyncHelper.RunSync(() => GetAsync<T>(key));
            return result;
        }

        public void Set<T>(T value, string key)
        {
            AsyncHelper.RunSync(() => SetAsync(value, key));
        }

        public void Delete(string key)
        {
            AsyncHelper.RunSync(() => DeleteAsync(key));
        }

        public async Task<T> GetAsync<T>(string key)
        {
            StorageFolder folder = ApplicationData.Current.RoamingFolder;

            try
            {
                var f = await folder.GetItemAsync(key) as StorageFile;
                if (f == null)
                    return default(T);

                using (IRandomAccessStream s = await f.OpenAsync(FileAccessMode.Read))
                {
                    using (var sr = new StreamReader(s.AsStreamForRead()))
                    {
                        string json = sr.ReadToEnd();
                        return JsonConvert.DeserializeObject<T>(json);
                    }
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public async Task SetAsync<T>(T value, string key)
        {
            StorageFolder folder = ApplicationData.Current.RoamingFolder;

            Delete(key);

            try
            {
                StorageFile f = await folder.CreateFileAsync(key);
                if (f == null)
                    return;

                using (IRandomAccessStream s = await f.OpenAsync(FileAccessMode.ReadWrite))
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

        public async Task DeleteAsync(string key)
        {
            StorageFolder folder = ApplicationData.Current.RoamingFolder;

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