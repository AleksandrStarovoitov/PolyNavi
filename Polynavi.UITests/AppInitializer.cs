using Xamarin.UITest;

namespace Polynavi.UITests
{
    public static class AppInitializer
    {
        private const string ApkPath = "./../../../../Polynavi.Droid/bin/Release/com.starovoitov.polynavi.apk";

        public static IApp StartApp()
        {
            return ConfigureApp.Android.ApkFile(ApkPath).StartApp();
        }
    }
}
