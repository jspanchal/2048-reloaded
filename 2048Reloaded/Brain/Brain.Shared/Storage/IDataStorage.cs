namespace Brain.Storage
{
    /// <summary>
    /// Stores data in a separate file
    /// </summary>
    public interface IDataStorage
    {
        T Get<T>(string key);
        void Save<T>(T value, string key);
        void Delete(string key);
    }
}
