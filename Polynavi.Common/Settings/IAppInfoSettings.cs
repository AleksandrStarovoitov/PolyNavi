namespace Polynavi.Common.Settings
{
    public interface IAppInfoSettings
    {
        string StartScreen { get; set; }

        int AppVersionCode { get; set; }

        string AppLanguage { get; set; }
    }
}
