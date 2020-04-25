using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Java.Util;
using Polynavi.Common.Constants;

namespace Polynavi.Droid
{
    [Application(
        Label = "@string/app_name",
        AllowBackup = true,
        Theme = "@style/MyAppTheme.Launcher")]
    public class MainApp : Application
    {
        internal const string DatabaseFilename = "schedule.sqlite"; //TODO Move
        internal const string MainGraphFilename = "main.graph"; //TODO Move
        internal const string MainGraphXmlFilename = "main_graph.xml"; //TODO Move
        private string language;

        public static MainApp Instance { get; private set; } //TODO Remove        

        public MainApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            Instance = this;
            AndroidDependencyContainer.EnsureInitialized(ApplicationContext);
            SetDefaultPreferences();

            if (IsAppUpdated()) //TODO
            {
                //Task.Run(async () =>
                //{
                //    var manager = await PolyManager;
                //    await manager.ReinitializeDatabaseAsync();
                //});
            }

            language = AndroidDependencyContainer.Instance.SettingsStorage.GetString("language", null);
        }

        private int GetVersionCode()
        {
            var packageInfo = PackageManager.GetPackageInfo(PackageName, 0);
            return packageInfo.VersionCode;
        }

        internal bool IsAppUpdated()
        {
            const int defaultVersionPrefValue = -1;

            var currentVersion = GetVersionCode();
            var savedVersion = AndroidDependencyContainer.Instance.SettingsStorage
                .GetInt(PreferenceConstants.VersionPreferenceKey, defaultVersionPrefValue);

            if (savedVersion == defaultVersionPrefValue || currentVersion > savedVersion)
            {
                AndroidDependencyContainer.Instance.SettingsStorage
                    .PutInt(PreferenceConstants.VersionPreferenceKey, currentVersion);

                return true;
            }

            return false;
        }

        private void SetDefaultPreferences()
        {
            var containsIsTeacher = AndroidDependencyContainer.Instance.SettingsStorage
                .Contains(PreferenceConstants.IsUserTeacherPreferenceKey);

            if (!containsIsTeacher)
            {
                AndroidDependencyContainer.Instance.SettingsStorage
                    .PutBoolean(PreferenceConstants.IsUserTeacherPreferenceKey, false);
            }
        }

        internal static string GetFileFullPath(string fileName) //TODO Move
        {
            var dirPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(dirPath, fileName);
        }

        ///<param name="relativePath">Path relative to Polynavi.Droid.EmbeddedResources</param>
        internal static Stream GetEmbeddedResourceStream(string relativePath) //TODO Move
        {
            var assembly = typeof(MainApp).GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream($"Polynavi.Droid.EmbeddedResources.{relativePath}");
        }

        internal class DictionaryComp : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x[0] > '0' && x[0] < '9' && y[0] > 'A' && y[0] < 'я') //100, abc
                {
                    return 1;
                }

                if (y[0] > '0' && y[0] < '9' && x[0] > 'A' && x[0] < 'я') //abc, 100
                {
                    return -1;
                }

                return x.CompareTo(y); //TODO ?
            }
        }

#pragma warning disable 618 // Disable "UpdateConfiguration" deprecate warning
        public static void ChangeLanguage(Context c)
        {
            if (Instance.language == null)
            {
                return;
            }

            var config = c.Resources.Configuration;

            var locale = new Locale(Instance.language);
            Locale.Default = locale;
            var conf = new Configuration(config);
            conf.SetLocale(locale);
            c.Resources.UpdateConfiguration(conf, c.Resources.DisplayMetrics);
        }
#pragma warning restore 618
    }
}
