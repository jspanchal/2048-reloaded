namespace Brain.Storage
{
    /// <summary>
    /// Used for simple storage of small data.  Not good for large files
    /// </summary>
    public interface ISimpleStorage
    {
        T GetSettings<T>(string name);
        void SaveSettings<T>(T settings, string name);
        void DeleteSetting(string name);
    }
}
