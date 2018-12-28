namespace Brain.Storage
{
    /// <summary>
    ///     Used for simple storage of small data.  Not good for large files
    /// </summary>
    public interface IStorage
    {
        T Get<T>(string name);
        void Set<T>(T settings, string name);
        void Delete(string name);
    }
}