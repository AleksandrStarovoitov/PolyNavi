using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Support.V7.Preferences;
using Java.Util;

using Mapsui.Geometries;

using Nito.AsyncEx;

using PolyNaviLib.BL;
using PolyNaviLib.SL;

using Newtonsoft.Json;
using Android.Content.PM;

namespace PolyNavi
{
    [Application(
        Label = "@string/app_name",
        AllowBackup = true,
        Theme = "@style/MyAppTheme.Launcher",
#if DEBUG
		Debuggable = true
#else
        Debuggable = false
#endif
        )]
    public class MainApp : Application
    {
        const string DatabaseFilename = "schedule.sqlite";
        readonly string groupLink = @"http://m.spbstu.ru/p/proxy.php?csurl=http://ruz.spbstu.ru/api/v1/ruz/search/groups";

        Locale locale = null;
        string language;

        public static MainApp Instance { get; private set; }

        public Dictionary<string, Point> BuildingsDictionary { get; private set; } = new Dictionary<string, Point>()
        {
            { "Главный учебный корпус", new Point(60.00718, 30.37281)},
            { "Химический корпус", new Point(60.00648, 30.37630)},
            { "Механический корпус", new Point(60.00768, 30.37628)},
            { "Гидрокорпус-1", new Point(60.00565, 30.38176)},
            { "Гидрокорпус-2", new Point(60.00670, 30.38266)},
            { "НИК", new Point(60.005903, 30.379046)},
            { "1-й учебный корпус", new Point(60.00885, 30.37270)},
            { "2-й учебный корпус", new Point(60.00846, 30.37492)},
            { "3-й учебный корпус", new Point(60.00711, 30.38149)},
            { "4-й учебный корпус", new Point(60.00750, 30.37694)},
            { "5-й учебный корпус", new Point(59.99984, 30.37438)},
            { "6-й учебный корпус", new Point(60.00048, 30.36805)},
            { "9-й учебный корпус", new Point(60.00081, 30.36619)},
            { "10-й учебный корпус", new Point(60.00066, 30.36902)},
            { "11-й учебный корпус", new Point(60.00900, 30.37744)},
            { "15-й учебный корпус (ИМОП)", new Point(60.00689, 30.39065)},
            { "16-й учебный корпус", new Point(60.00790, 30.39041)},
            { "Спортивный комплекс", new Point(60.00295, 30.36801)},
            { "Лабораторный корпус", new Point(60.00734, 30.37954)},
            { "Гидробашня", new Point(60.00583, 30.37428)},
            { "НОЦ РАН", new Point(60.00317, 30.37468)},
            { "1-й профессорский корпус", new Point(60.00481, 30.37071)},
            { "2-й профессорский корпус", new Point(60.00475, 30.37796)},
            { "Дом ученых в Лесном", new Point(60.00448, 30.37908)},
            { "Секретариат приемной комиссии", new Point(60.009405, 30.371689)},
            { "ИПМЭиТ", new Point(59.994757, 30.356456) }
        };

        public Dictionary<string, string> RoomsDictionary { get; private set; } = new Dictionary<string, string>();

        public AsyncLazy<Dictionary<string, int>> GroupsDictionary { get; set; } = new AsyncLazy<Dictionary<string, int>>(async () =>
        {
            return await FillGroupsDictionary(false, new CancellationToken());
        });

        public static async Task<Dictionary<string, int>> FillGroupsDictionary(bool rewrite, CancellationToken cts)
        {
            var networkChecker = new NetworkChecker(Instance);
            string resultJson;

            var resourceName = "groups.json";

            var b = File.Exists(GetFileFullPath(resourceName));

            if (rewrite && networkChecker.Check())
            {
                var client = new HttpClient();
                resultJson = await HttpClientSL.GetResponseAsync(client, Instance.groupLink, cts);

                File.WriteAllText(GetFileFullPath(resourceName), resultJson);
            }
            else
            {
                using (var stream = GetEmbeddedResourceStream(resourceName))
                using (var reader = b ? new StreamReader(GetFileFullPath(resourceName)) : new StreamReader(stream))
                {
                    resultJson = await reader.ReadToEndAsync();
                }
            }
            var groups = JsonConvert.DeserializeObject<GroupRoot>(resultJson);
            var dictionary = groups.Groups.ToDictionary(x => x.Name, x => x.Id);

            return dictionary;
        }

        public int GetVersionCode()
        {
            PackageInfo packageInfo = PackageManager.GetPackageInfo(PackageName, 0);
            return packageInfo.VersionCode;
        }

        public bool IsAppUpdated()
        {
            int ver = GetVersionCode();
            if (ver > SharedPreferences.GetInt("version", 0))
            {
                SharedPreferences.Edit().PutInt("version", ver).Commit();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Lazy<Graph.GraphNode> MainBuildingGraph { get; private set; } = new Lazy<Graph.GraphNode>(() =>
        {
            if (File.Exists(GetFileFullPath("main.graph")) && !Instance.IsAppUpdated())
            {
                using (var stream = File.OpenRead(GetFileFullPath("main.graph")))
                {
                    FillRoomsDictionary();
                    return Graph.SaverLoader.Load(stream);
                }
            }
            else
            {
                var graph = Instance.GraphSaverLoader.LoadFromXmlDescriptor("main_graph.xml");
                using (var stream = File.Create(GetFileFullPath("main.graph")))
                {
                    Graph.SaverLoader.Save(stream, graph);
                }
                FillRoomsDictionary();
                return graph;
            }
        });

        private static void FillRoomsDictionary()
        {
            using (var stream = File.OpenRead(GetFileFullPath("main.graph")))
            {
                var graph = Graph.SaverLoader.Load(stream);

                var ids = new List<Graph.GraphNode>();

                Queue<Graph.GraphNode> bfsQueue = new Queue<Graph.GraphNode>();
                bfsQueue.Enqueue(graph);

                while (bfsQueue.Count > 0)
                {
                    var node = bfsQueue.Dequeue();

                    foreach (var neighbour in node.Neighbours)
                    {
                        if (!ids.Contains(neighbour))
                        {
                            ids.Add(neighbour);
                            bfsQueue.Enqueue(neighbour);
                            if (!neighbour.RoomName.Equals("*Unknown*"))
                            {
                                var name = neighbour.RoomName.Replace("_а", " (а)").Replace("_М_1_1", " М 1 эт. 1")
                                                                                                    .Replace("_М_1_2", " М 1 эт. 2").Replace("_М_2_1", " М 2 эт. 1")
                                                                                                    .Replace("_М_2_2", " М 2 эт. 2").Replace("_Ж_1_1", " Ж 1 эт. 1")
                                                                                                    .Replace("_Ж_1_2", " Ж 1 эт. 2").Replace("_Ж_1_3", " Ж 1 эт. 3")
                                                                                                    .Replace("_Ж_2_1", " Ж 2 эт. 1").Replace("_Ж_2_2", " Ж 2 эт. 2")
                                                                                                    .Replace("_Ж_2_3", " Ж 2 эт. 3").Replace("Ректорат_", "Ректорат ")
                                                                                                    .Replace("101а", "101 (а)").Replace("170_б", "170 (б)");
                                Instance.RoomsDictionary[name] = neighbour.RoomName;
                            }
                        }
                    }
                }
            }
            var ordered = Instance.RoomsDictionary.OrderBy(x => x.Value, new DictionaryComp());
            Instance.RoomsDictionary = ordered.ToDictionary(x => x.Key, x => x.Value);
        }

        public AsyncLazy<PolyManager> PolyManager { get; private set; } = new AsyncLazy<PolyManager>(async () =>
        {
            return await PolyNaviLib.BL.PolyManager.CreateAsync(GetFileFullPath(DatabaseFilename),
                                                                new NetworkChecker(MainApp.Instance),
                                                                new SettingsProvider(MainApp.Instance.SharedPreferences));
        });

        public ISharedPreferences SharedPreferences { get; private set; }

        public Graph.SaverLoader GraphSaverLoader { get; private set; }

        public MainApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Instance = this;
            SharedPreferences = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            GraphSaverLoader = new Graph.SaverLoader(new AssetsProvider(ApplicationContext));
            if (IsAppUpdated())
            {
                var groupName = SharedPreferences.GetString("groupnumber", "-1");

                var dictionary = Instance.GroupsDictionary.Task.Result;

                if (dictionary.TryGetValue(groupName, out int id))
                {
                    SharedPreferences.Edit().PutInt("groupid", id).Apply();
                }
            }
        }

        public override void OnCreate()
        {
            base.OnCreate();
            language = SharedPreferences.GetString("language", null);
        }

        internal static string GetFileFullPath(string fname)
        {
            string dirPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return Path.Combine(dirPath, fname);
        }

        /// <summary>
        /// Возвращает поток встроенного ресурса, с указанным относительным путём
        /// </summary>
        /// <param name="relativePath">Путь относительно папки PolyNavi.EmbeddedResources</param>
        internal static Stream GetEmbeddedResourceStream(string relativePath)
        {
            var assembly = typeof(MainApp).GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream($"PolyNavi.EmbeddedResources.{relativePath}");
        }

        public class DictionaryComp : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if ((x[0] > '0' && x[0] < '9') && (y[0] > 'A' && y[0] < 'я')) //100, abc
                {
                    return 1;
                }
                else
                if ((y[0] > '0' && y[0] < '9') && (x[0] > 'A' && x[0] < 'я')) //abc, 100
                {
                    return -1;
                }
                else
                {
                    return x.CompareTo(y);
                }
            }
        }

#pragma warning disable 618 // Disable "UpdateConfiguration" deprecate warning
        public static void ChangeLanguage(Context c)
        {
            if (Instance.language != null)
            {
                Configuration config = c.Resources.Configuration;

                var locale = new Locale(Instance.language);
                Locale.Default = locale;
                Configuration conf = new Configuration(config);
                conf.SetLocale(locale);
                c.Resources.UpdateConfiguration(conf, c.Resources.DisplayMetrics);
            }
        }
#pragma warning restore 618
    }
}