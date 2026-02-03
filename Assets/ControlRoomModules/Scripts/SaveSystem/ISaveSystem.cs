namespace ControlRoom
{
    public interface ISaveSystem
    {
        void Save<T>(string key, T data, string fileName = null);
        T Load<T>(string key, string fileName = null, T defaultValue = default);
        bool Exists(string key, string fileName = null);
        void Delete(string key, string fileName = null);
    }
}
