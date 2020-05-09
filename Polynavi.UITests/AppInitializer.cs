using Xamarin.UITest;

namespace Polynavi.UITests
{
    public static class AppInitializer
    {
        private const string ApkPath = "com.starovoitov.polynavi.apk";

        public static IApp StartApp()
        {
            return ConfigureApp.Android.ApkFile(ApkPath).StartApp();
        }
    }
}
