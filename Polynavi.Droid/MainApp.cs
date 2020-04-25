using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Graph;
using Java.Util;
using Polynavi.Common.Constants;
using Polynavi.Droid.Services;
using Point = Mapsui.Geometries.Point;

namespace Polynavi.Droid
{
    [Application(
        Label = "@string/app_name",
        AllowBackup = true,
        Theme = "@style/MyAppTheme.Launcher")]
    public class MainApp : Application
    {
        internal const string DatabaseFilename = "schedule.sqlite"; //TODO Move
        private const string MainGraphFilename = "main.graph";
        private const string MainGraphXmlFilename = "main_graph.xml";
        private string language;

        private SaverLoader GraphSaverLoader { get; }

        public static MainApp Instance { get; private set; } //TODO Remove

        

        public Lazy<GraphNode> MainBuildingGraph { get; } = new Lazy<GraphNode>(() =>
        {
            GraphNode graphNode;

            if (File.Exists(GetFileFullPath(MainGraphFilename)) && !Instance.IsAppUpdated())
            {
                graphNode = LoadGraphFromFile();
            }
            else
            {
                graphNode = LoadGraphFromXml();

                SaveGraphToFile(graphNode);
            }

            FillRoomsDictionary(graphNode);

            return graphNode;
        });

        public Dictionary<string, string> RoomsDictionary { get; private set; } = new Dictionary<string, string>();

        public MainApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            GraphSaverLoader = new SaverLoader(new AssetsProvider(ApplicationContext));
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

        private bool IsAppUpdated()
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


        private static GraphNode LoadGraphFromFile()
        {
            using var stream = File.OpenRead(GetFileFullPath(MainGraphFilename));

            return SaverLoader.Load(stream);
        }

        private static GraphNode LoadGraphFromXml()
        {
            var graph = Instance.GraphSaverLoader.LoadFromXmlDescriptor(MainGraphXmlFilename);

            return graph;
        }

        private static void SaveGraphToFile(GraphNode graphNode)
        {
            using var stream = File.Create(GetFileFullPath(MainGraphFilename));

            SaverLoader.Save(stream, graphNode);
        }

        private static void FillRoomsDictionary(GraphNode graph)
        {
            var ids = new List<GraphNode>();

            var bfsQueue = new Queue<GraphNode>();
            bfsQueue.Enqueue(graph);

            while (bfsQueue.Count > 0)
            {
                var node = bfsQueue.Dequeue();

                foreach (var neighbour in node.Neighbours.Where(neighbour => !ids.Contains(neighbour)))
                {
                    ids.Add(neighbour);
                    bfsQueue.Enqueue(neighbour);
                    if (neighbour.RoomName.Equals("*Unknown*"))
                    {
                        continue;
                    }

                    var name = neighbour.RoomName.Replace("_а", " (а)").Replace("_М_1_1", " М 1 эт. 1") //TODO
                        .Replace("_М_1_2", " М 1 эт. 2").Replace("_М_2_1", " М 2 эт. 1")
                        .Replace("_М_2_2", " М 2 эт. 2").Replace("_Ж_1_1", " Ж 1 эт. 1")
                        .Replace("_Ж_1_2", " Ж 1 эт. 2").Replace("_Ж_1_3", " Ж 1 эт. 3")
                        .Replace("_Ж_2_1", " Ж 2 эт. 1").Replace("_Ж_2_2", " Ж 2 эт. 2")
                        .Replace("_Ж_2_3", " Ж 2 эт. 3").Replace("Ректорат_", "Ректорат ")
                        .Replace("101а", "101 (а)").Replace("170_б", "170 (б)");
                    Instance.RoomsDictionary[name] = neighbour.RoomName;
                }
            }

            var ordered = Instance.RoomsDictionary.OrderBy(x => x.Value, new DictionaryComp());
            Instance.RoomsDictionary = ordered.ToDictionary(x => x.Key, x => x.Value);
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

        private class DictionaryComp : IComparer<string>
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
