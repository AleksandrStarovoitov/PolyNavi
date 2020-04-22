namespace Polynavi.Common.Services
{
    public interface ISettingsProvider
    {
        object this[string key] { get; set; }
    }
}
