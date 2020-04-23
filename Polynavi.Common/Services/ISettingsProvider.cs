namespace Polynavi.Common.Services
{
    public interface ISettingsProvider //TODO GetBoolean, GetInt, etc.
    {
        object this[string key] { get; set; }
    }
}
