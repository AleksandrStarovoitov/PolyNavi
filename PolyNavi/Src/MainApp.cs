using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Preferences;

using Mapsui.Geometries;

using Nito.AsyncEx;

using PolyNaviLib.BL;
using System.Reflection;

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
			{ "15-й учебный корпус (ИМОП)", new Point(60.00714, 30.39050)},
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

		public AsyncLazy<PolyManager> PolyManager { get; private set; } = new AsyncLazy<PolyManager>(async () =>
		{
			return await PolyNaviLib.BL.PolyManager.CreateAsync(GetFileFullPath(DatabaseFilename),
				                                                new NetworkChecker(MainApp.Instance));
		});

		public ISharedPreferences SharedPreferences { get; private set; }

		public MainApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
			Instance = this;
			SharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this.ApplicationContext);
#if DEBUG
			DebugInit();
#endif
		}

		private void DebugInit()
		{
			var editor = SharedPreferences.Edit();
			editor.PutString("startactivity_preference", "schedule");
			editor.PutString("groupnumber_preference", "23537/1");
			editor.Apply();
		}

		public override void OnCreate()
		{
			base.OnCreate();
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
	}
}