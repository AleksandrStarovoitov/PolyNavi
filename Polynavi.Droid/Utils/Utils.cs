using Android.App;
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using System;
using System.IO;

namespace Polynavi.Droid.Utils
{
    internal static class Utils
    {
        internal static void HideKeyboard(View view, Context context)
        {
            //TODO Check null
            var inputMethodManager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(view.WindowToken, 0);
        }

        internal static string GetFileFullPath(string fileName) //TODO Move
        {
            var dirPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(dirPath, fileName);
        }

        ///<param name="relativePath">Path relative to Polynavi.Droid.EmbeddedResources</param>
        internal static Stream GetEmbeddedResourceStream(string relativePath) //TODO Move
        {
            var assembly = typeof(MainApp).Assembly;
            return assembly.GetManifestResourceStream($"Polynavi.Droid.EmbeddedResources.{relativePath}");
        }

        internal static bool IsAppUpdated()
        {
            var appInfoSettings = AndroidDependencyContainer.Instance.AppInfoSettings;

            var currentVersionCode = Application.Context.PackageManager.GetPackageInfo(Application.Context.PackageName, 0).VersionCode;
            var savedVersionCode = appInfoSettings.AppVersionCode;

            if (savedVersionCode == 0 || currentVersionCode > savedVersionCode)
            {
                appInfoSettings.AppVersionCode = currentVersionCode;

                return true;
            }

            return false;
        }
    }
}
