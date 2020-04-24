using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using AndroidX.Preference;
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

        public static MainApp Instance { get; private set; } //TODO Remove

        public Dictionary<string, Point> BuildingsDictionary { get; } = new Dictionary<string, Point>()
        {
            { "Главный учебный корпус", new Point(60.00718, 30.37281) },
            { "Химический корпус", new Point(60.00648, 30.37630) },
            { "Механический корпус", new Point(60.00768, 30.37628) },
            { "Гидрокорпус-1", new Point(60.00565, 30.38176) },
            { "Гидрокорпус-2", new Point(60.00670, 30.38266) },
            { "НИК", new Point(60.005903, 30.379046) },
            { "1-й учебный корпус", new Point(60.00885, 30.37270) },
            { "2-й учебный корпус", new Point(60.00846, 30.37492) },
            { "3-й учебный корпус", new Point(60.00711, 30.38149) },
            { "4-й учебный корпус", new Point(60.00750, 30.37694) },
            { "5-й учебный корпус", new Point(59.99984, 30.37438) },
            { "6-й учебный корпус", new Point(60.00048, 30.36805) },
            { "9-й учебный корпус", new Point(60.00081, 30.36619) },
            { "10-й учебный корпус", new Point(60.00066, 30.36902) },
            { "11-й учебный корпус", new Point(60.00900, 30.37744) },
            { "15-й учебный корпус (ИМОП)", new Point(60.00689, 30.39065) },
            { "16-й учебный корпус", new Point(60.00790, 30.39041) },
            { "Спортивный комплекс", new Point(60.00295, 30.36801) },
            { "Лабораторный корпус", new Point(60.00734, 30.37954) },
            { "Гидробашня", new Point(60.00583, 30.37428) },
            { "НОЦ РАН", new Point(60.00317, 30.37468) },
            { "1-й профессорский корпус", new Point(60.00481, 30.37071) },
            { "2-й профессорский корпус", new Point(60.00475, 30.37796) },
            { "Дом ученых в Лесном", new Point(60.00448, 30.37908) },
            { "Секретариат приемной комиссии", new Point(60.009405, 30.371689) },
            { "ИПМЭиТ", new Point(59.994757, 30.356456) }
        };

        public Dictionary<string, string> RoomsDictionary { get; private set; } = new Dictionary<string, string>();

        private int GetVersionCode()
        {
            var packageInfo = PackageManager.GetPackageInfo(PackageName, 0);
            return packageInfo.VersionCode;
        }

        private bool IsAppUpdated()
        {
            const int defaultVersionPrefValue = -1;

            var currentVersion = GetVersionCode();
            var savedVersion = SharedPreferences
                .GetInt(PreferenceConstants.VersionPreferenceKey, defaultVersionPrefValue);

            if (savedVersion == defaultVersionPrefValue || currentVersion > savedVersion)
            {
                SharedPreferences.Edit().PutInt(PreferenceConstants.VersionPreferenceKey, currentVersion).Commit();
                return true;
            }

            return false;
        }

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

        public ISharedPreferences SharedPreferences { get; } //TODO Remove

        private SaverLoader GraphSaverLoader { get; }

        public MainApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Instance = this;
            AndroidDependencyContainer.EnsureInitialized(ApplicationContext);
            SharedPreferences = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            GraphSaverLoader = new SaverLoader(new AssetsProvider(ApplicationContext));
            SetDefaultPreferences();

            if (IsAppUpdated()) //TODO
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
            var containsIsTeacher = SharedPreferences.Contains(PreferenceConstants.IsUserTeacherPreferenceKey);

            if (!containsIsTeacher)
            {
                SharedPreferences.Edit().PutBoolean(PreferenceConstants.IsUserTeacherPreferenceKey, false).Commit();
            }
        }

        public override void OnCreate()
        {
            base.OnCreate();
            language = SharedPreferences.GetString("language", null);
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
