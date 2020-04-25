namespace Polynavi.Common.Services
{
    public interface IKeyValueStorage
    {
        bool GetBoolean(string key, bool defaultValue);
        string GetString(string key, string defaultValue);
        int GetInt(string key, int defaultValue);
        long GetLong(string key, long defaultValue);

        void PutBoolean(string key, bool value);
        void PutString(string key, string value);
        void PutInt(string key, int value);
        void PutLong(string key, long value);

        bool Contains(string key);
        void Remove(string key);

        void AddOnChangeListener(object listener);
        void RemoveOnChangeListener(object listener);
    }
}
