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
		Debugable = false
#endif
		)]
	public class MainApp : Application
	{
		private const string DatabaseFilename = "schedule.sqlite";

		public static AsyncLazy<PolyManager> PolyManager { get; private set; } = new AsyncLazy<PolyManager>(async () =>
		{
			return await PolyNaviLib.BL.PolyManager.CreateAsync(GetFileFullPath(DatabaseFilename),
				                                                new NetworkChecker());
		});

		public static ISharedPreferences SharedPreferences { get; private set; }

		public MainApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
			SharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this.ApplicationContext);
		}

		public override void OnCreate()
		{
			base.OnCreate();
		}

		private static string GetFileFullPath(string fname)
		{
			string dirPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			return Path.Combine(dirPath, fname);
		}
	}
}