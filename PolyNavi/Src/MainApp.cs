using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Preferences;
using Java.Util;

using Mapsui.Geometries;

using Nito.AsyncEx;

using PolyNaviLib.BL;

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
		private const string DatabaseFilename = "schedule.sqlite";
		private Locale locale = null;
		private string language;

		public static MainApp Instance { get; private set; }

		public Dictionary<string, Point> BuildingsDictionary { get; private set; } = new Dictionary<string, Point>()
		{
			{ "Главный учебный корпус", new Point(60.00718, 30.37281)},
			{ "Химический корпус", new Point(60.00648, 30.37630)},
			{ "Механический корпус", new Point(60.00768, 30.37628)},
			{ "Гидрокорпус-1", new Point(60.00565, 30.38176)},
			{ "Гидрокорпус-2", new Point(60.00670, 30.38266)},
			{ "НИК", new Point(59.99828, 30.37347)},
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
			{ "Секретариат приемной комиссии", new Point(60.00048, 30.36805)}
		};

		public Dictionary<string, string> RoomsDictionary { get; private set; } = new Dictionary<string, string>();

		public Lazy<Graph.GraphNode> MainBuildingGraph { get; private set; } = new Lazy<Graph.GraphNode>(() =>
		{
			if (File.Exists(GetFileFullPath("main.graph")))
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
			//using (var stream = MainApp.Instance.Assets.Open("floor_1.graph"))
			//{
			//	Graph.GraphNode floor1 = Graph.SaverLoader.Load(stream);
			//	return floor1;
			//}
		});

		private static void FillRoomsDictionary()
		{
			using (var stream = File.OpenRead(GetFileFullPath("main.graph")))
			{
				var graph = Graph.SaverLoader.Load(stream);

				var ids = new List<Tuple<int, int>>();
				Queue<Graph.GraphNode> bfsQueue = new Queue<Graph.GraphNode>();
				bfsQueue.Enqueue(graph);

				ids.Add(new Tuple<int, int>(graph.Id, graph.FloorNumber));

				while (bfsQueue.Count > 0)
				{
					var node = bfsQueue.Dequeue();

					if (node == null)
					{
						continue;
					}

					foreach (var neighbour in node.Neighbours)
					{
						if (!ids.Contains(new Tuple<int, int>(neighbour.Id, neighbour.FloorNumber)))
						{
							ids.Add(new Tuple<int, int>(neighbour.Id, neighbour.FloorNumber));
							bfsQueue.Enqueue(neighbour);
							if (!neighbour.RoomName.Equals("*Unknown*"))
							{
								Instance.RoomsDictionary[neighbour.RoomName] = neighbour.RoomName.Replace("_М_1_1", " М").Replace("_М_2_1", " М").Replace("_М_2_2", " М");
							}
						}
					}
				}
			}
			var ordered = Instance.RoomsDictionary.OrderBy(x => x.Value);
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