using System;
using Xamarin.UITest;
using Xamarin.UITest.Configuration;

namespace Polynavi.UITests
{
    internal static class AppManager
    {
        private const string ApkPath = "./../../../../Polynavi.Droid/bin/Release/com.starovoitov.polynavi.apk";
        
        private static IApp app;
        private static Platform? platform;
        
        public static IApp App =>
            app ?? throw new NullReferenceException(
                "'AppManager.App' not set. Call 'AppManager.StartApp()' before trying to access it.");
        
        public static Platform Platform
        {
            get
            {
                if (platform == null)
                {
                    throw new NullReferenceException("'AppManager.Platform' not set.");
                }

                return platform.Value;
            }

            set => platform = value;
        }
        
        public static void StartApp()
        {
            if (Platform == Platform.Android)
            {
                app = ConfigureApp
                    .Android
                    .ApkFile(ApkPath)
                    .StartApp();
            }
        }

        public static void StartInstalledApp()
        {
            if (Platform == Platform.Android)
            {
                app = ConfigureApp
                    .Android
                    .InstalledApp("com.starovoitov.polynavi")
                    .StartApp(AppDataMode.DoNotClear);
            }
        }
    }
}
