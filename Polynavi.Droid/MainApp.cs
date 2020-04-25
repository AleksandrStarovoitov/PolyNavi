using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Java.Util;

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

        public MainApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            AndroidDependencyContainer.EnsureInitialized();

            SetDefaultPreferences();

            if (Utils.Utils.IsAppUpdated()) //TODO
            {
                //Task.Run(async () =>
                //{
                //    var manager = await PolyManager;
                //    await manager.ReinitializeDatabaseAsync();
                //});
            }
        }        

        private void SetDefaultPreferences()
        {
            var scheduleSettings = AndroidDependencyContainer.Instance.ScheduleSettings;

            if (!scheduleSettings.ContainsIsTeacherKey)
            {
                scheduleSettings.IsUserTeacher = false;
            }
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
            var language = AndroidDependencyContainer.Instance.AppInfoSettings.AppLanguage;

            if (String.IsNullOrEmpty(language))
            {
                return;
            }

            var config = c.Resources.Configuration;

            var locale = new Locale(language);
            Locale.Default = locale;
            var conf = new Configuration(config);
            conf.SetLocale(locale);
            c.Resources.UpdateConfiguration(conf, c.Resources.DisplayMetrics);
        }
#pragma warning restore 618
    }
}
