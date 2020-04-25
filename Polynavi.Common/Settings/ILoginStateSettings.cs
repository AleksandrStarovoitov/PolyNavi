namespace Polynavi.Common.Settings
{
    public interface ILoginStateSettings
    {
        bool IsWelcomeCompleted { get; set; }

        bool IsAuthCompleted { get; set; }
    }
}
