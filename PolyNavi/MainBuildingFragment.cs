using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using static Android.Widget.TextView;

namespace PolyNavi
{
	public class MainBuildingFragment : Fragment, IOnEditorActionListener
	{
		View v;
		TextInputLayout ti1;
		TextInputLayout ti2;
		EditText editText, editText2;
		static bool focus1, focus2;
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			//view = new MainBuildingNavView(Activity.BaseContext);


		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{


			v = inflater.Inflate(Resource.Layout.fragment_mainbuilding, container, false);


			var ft = FragmentManager.BeginTransaction();
			//ft.AddToBackStack(null);
			//ft.Add(Resource.Id.contentframe_main, new MainFragment());
			ft.Add(Resource.Id.frame_mainbuilding, new MainBuildingMapFragment());
			ft.Commit();

			editText = v.FindViewById<EditText>(Resource.Id.edittext_input_to);
			editText.SetOnEditorActionListener(this);
			editText.FocusChange += EditText_FocusChange;

			editText2 = v.FindViewById<EditText>(Resource.Id.edittext_input_from);
			editText2.FocusChange += EditText_FocusChange2;


			return v;

		}

		private void EditText_FocusChange(object sender, View.FocusChangeEventArgs e)
		{
			if (!e.HasFocus)
			{
				focus1 = false;
				//Toast.MakeText(Activity.Application, "Lost the focus", ToastLength.Long).Show();
				//InputMethodManager imm = (InputMethodManager)Activity.ApplicationContext.GetSystemService(Context.InputMethodService);
				////InputMethodManager imm = (InputMethodManager)getSystemService(Context.INPUT_METHOD_SERVICE);
				//imm.HideSoftInputFromWindow(v.WindowToken, 0);
			}
			else
			{
				focus1 = true;
			}
		}

		private void EditText_FocusChange2(object sender, View.FocusChangeEventArgs e)
		{
			if (!e.HasFocus)
			{
				focus2 = false;
				//Toast.MakeText(Activity.Application, "Lost the focus", ToastLength.Long).Show();
				//InputMethodManager imm = (InputMethodManager)Activity.ApplicationContext.GetSystemService(Context.InputMethodService);
				////InputMethodManager imm = (InputMethodManager)getSystemService(Context.INPUT_METHOD_SERVICE);
				//imm.HideSoftInputFromWindow(v.WindowToken, 0);
			}
			else
			{
				focus2 = true;
			}
		}

		public static bool checkFocus()
		{
			return (focus1 && focus2);
		}



		public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
		{

			//bool handled = false;
			if (actionId == ImeAction.Go)
			{
				Toast.MakeText(Activity.BaseContext, "HELLO", ToastLength.Short).Show();
				//handled = true;
			}
			return false;
		}
	}
}