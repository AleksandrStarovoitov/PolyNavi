namespace PolyNaviLib.BL
{
    public interface ISettingsProvider
    {
		object this[string key] { get; set; }
    }
}
