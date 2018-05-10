using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using static Android.Support.V7.Widget.RecyclerView;

namespace PolyNavi
{
	[Activity(Label = "NEWMapRouteActivity")]
	public class MapRouteActivity : AppCompatActivity, ITextWatcher
	{
		private EditText editTextFrom;
		private EditText editTextTo;
		private EditText selected, deselected;
		private BuildingsAdapter adapterBuildings;
		private List<object> buildings;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			SetTheme(Resource.Style.MyAppTheme);
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.new_activity_map_routing);

			var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_route);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			//SupportActionBar.SetDisplayShowHomeEnabled(true);

			editTextFrom = FindViewById<EditText>(Resource.Id.edittext_from_buildings);
			editTextTo = FindViewById<EditText>(Resource.Id.edittext_to_buildings);
			selected = editTextFrom;
			deselected = editTextTo;
			
			editTextFrom.Click += EditTextFrom_Click;
			editTextTo.Click += EditTextTo_Click;
			editTextFrom.AddTextChangedListener(this);
			editTextTo.AddTextChangedListener(this);
			//SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
			Title = GetString(Resource.String.title_route_activity);

			buildings = new List<object>(MainApp.Instance.BuildingsDictionary.Keys);
			buildings[0] = new MainBuildingTag() { MainBuildingString = buildings[0].ToString() };

			var listView = FindViewById<ListView>(Resource.Id.listview_buildingslist);

			adapterBuildings = new BuildingsAdapter(this, buildings);
			listView.Adapter = adapterBuildings;
			listView.ItemClick += ListView_ItemClick;
		}

		private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			var obj = buildings[e.Position];
			var temp = deselected;
			selected.Text = obj.ToString();
			Select(deselected);
			Deselect(selected);

			temp = deselected;
			deselected = selected;
			selected = temp;
		}

		private void EditTextFrom_Click(object sender, EventArgs e)
		{
			if (selected == editTextTo)
			{
				Select(editTextFrom);
				Deselect(editTextTo);

				selected = editTextFrom;
				deselected = editTextTo;
			}
			else
			{
				Select(editTextTo);
				Deselect(editTextFrom);

				selected = editTextTo;
				deselected = editTextFrom;
			}			
		}

		private void EditTextTo_Click(object sender, EventArgs e)
		{
			if (selected == editTextFrom)
			{
				Select(editTextTo);
				Deselect(editTextFrom);
				selected = editTextTo;
				deselected = editTextFrom;				
			}
			else
			{
				Select(editTextFrom);
				Deselect(editTextTo);
				selected = editTextFrom;
				deselected = editTextTo;
			}
		}

		public void Deselect(EditText editText)
		{
			DisplayMetrics metrics = Resources.DisplayMetrics;
			float dp = 5f;
			float fpixels = metrics.Density * dp;
			int pixels = (int)(fpixels + 0.5f);

			ViewGroup.MarginLayoutParams lparams = (ViewGroup.MarginLayoutParams)editText.LayoutParameters;
			//deselection
			lparams.Height = editText.Height - 2*pixels;
			//lparams.Width = editTextTo.Width - 20;
			lparams.LeftMargin += pixels;
			lparams.RightMargin += pixels;
			lparams.BottomMargin += pixels;
			lparams.TopMargin += pixels;
			editText.LayoutParameters = lparams;
		}

		public void Select(EditText editText)
		{
			DisplayMetrics metrics = Resources.DisplayMetrics;
			float dp = 5f;
			float fpixels = metrics.Density * dp;
			int pixels = (int)(fpixels + 0.5f);

			//selection
			ViewGroup.MarginLayoutParams lparams = (ViewGroup.MarginLayoutParams)editText.LayoutParameters;
			lparams.Height = editText.Height + 2*pixels;
			//lparams.Width = editTextFrom.Width + 20;
			lparams.LeftMargin -= pixels;
			lparams.RightMargin -= pixels;
			lparams.BottomMargin -= pixels;
			lparams.TopMargin -= pixels;
			editText.LayoutParameters = lparams;
		}

		public void AfterTextChanged(IEditable s)
		{
			if (!editTextFrom.Text.Equals("") && !editTextTo.Text.Equals(""))
			{
				string[] route = { editTextFrom.Text, editTextTo.Text };
				var intent = new Intent();
				intent.PutExtra("route", route);
				SetResult(Result.Ok, intent);
				Finish();
				//Toast.MakeText(this, "FROM: " + editTextFrom.Text + " TO: " + editTextTo.Text, ToastLength.Short).Show();
				//Finish();
			}
		}

		public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
		{
			//throw new NotImplementedException();
		}

		public void OnTextChanged(ICharSequence s, int start, int before, int count)
		{
			//throw new NotImplementedException();
		}

		public override bool OnSupportNavigateUp()
		{
			Finish();
			return true;
		}
	}
}