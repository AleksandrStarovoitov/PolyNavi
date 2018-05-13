using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using System;
using static Android.Widget.TextView;

using Graph;
using System.Collections.Generic;

namespace PolyNavi
{
	public class MainBuildingFragment : Fragment, IOnEditorActionListener, AppBarLayout.IOnOffsetChangedListener
	{
		GraphNode mapGraph;

		private View view;
		private EditText editTextInputFrom, editTextInputTo;
		private FragmentTransaction fragmentTransaction;
		private AppBarLayout appBar;
		private FloatingActionButton fab;
		private RelativeLayout relativeLayout;
		private AppBarLayout.LayoutParams relativeLayoutParams;
		private FrameLayout frameLayout;
		private CoordinatorLayout.LayoutParams fabLayoutParams;
		private FloatingActionButton buttonUp, buttonDown;
		private int currentFloor = 1;
		static bool editTextFromIsFocused, editTextToIsFocused;
		
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			mapGraph = MainApp.Instance.MainBuildingGraph.Value;

			view = inflater.Inflate(Resource.Layout.fragment_mainbuilding, container, false);

			fragmentTransaction = FragmentManager.BeginTransaction();
			fragmentTransaction.Add(Resource.Id.frame_mainbuilding, new MainBuildingMapFragment(), "MAP_MAINBUILDING");
			fragmentTransaction.Commit();

			editTextInputFrom = view.FindViewById<EditText>(Resource.Id.edittext_input_from);
			editTextInputFrom.FocusChange += EditTextToFocusChanged;

			editTextInputTo = view.FindViewById<EditText>(Resource.Id.edittext_input_to);
			editTextInputTo.SetOnEditorActionListener(this);
			editTextInputTo.FocusChange += EditTextFromFocusChanged;

			appBar = view.FindViewById<AppBarLayout>(Resource.Id.appbar_mainbuilding);
			appBar.AddOnOffsetChangedListener(this);

			fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab_mainbuilding);
			fab.Click += Fab_Click;


			relativeLayout = view.FindViewById<RelativeLayout>(Resource.Id.search_frame_mainbuilding);
			relativeLayoutParams = (AppBarLayout.LayoutParams)relativeLayout.LayoutParameters;

			frameLayout = view.FindViewById<FrameLayout>(Resource.Id.frame_mainbuilding);
			fabLayoutParams = (CoordinatorLayout.LayoutParams)fab.LayoutParameters;

			var rl = view.FindViewById<RelativeLayout>(Resource.Id.relativelayout_floor_buttons_mainbuilding);
			rl.BringToFront();

			buttonUp = view.FindViewById<FloatingActionButton>(Resource.Id.fab_up_mainbuilding);
			buttonUp.Click += ButtonUp_Click;
			buttonUp.Alpha = 0.7f;

			buttonDown = view.FindViewById<FloatingActionButton>(Resource.Id.fab_down_mainbuilding);
			buttonDown.Click += ButtonDown_Click;
			buttonDown.Alpha = 0.7f;
			buttonDown.Enabled = false;

			return view;
		}

		private void ButtonUp_Click(object sender, EventArgs e)
		{
			buttonDown.Enabled = true;
			if (currentFloor < 3)
			{
				currentFloor++;
				MainBuildingMapFragment fragment = (MainBuildingMapFragment)FragmentManager.FindFragmentByTag("MAP_MAINBUILDING");
				fragment.MapView.ChangeDrawable(GetDrawableIdByFloor(currentFloor));
			}
			if (currentFloor == 3)
			{
				buttonUp.Enabled = false;
			}
		}

		private void ButtonDown_Click(object sender, EventArgs e)
		{
			buttonUp.Enabled = true;
			if (currentFloor > 1)
			{
				currentFloor--;
				MainBuildingMapFragment fragment = (MainBuildingMapFragment)FragmentManager.FindFragmentByTag("MAP_MAINBUILDING");
				fragment.MapView.ChangeDrawable(GetDrawableIdByFloor(currentFloor));
			}
			if (currentFloor == 1)
			{
				buttonDown.Enabled = false;
			}
		}

		private int GetDrawableIdByFloor(int floor)
		{
			switch (floor)
			{
				case 1:
					return Resource.Drawable.first_floor;
				case 2:
					return Resource.Drawable.second_floor;
				case 3:
					return Resource.Drawable.third_floor;
				default:
					return Resource.Drawable.second_floor;
			}
		}

		bool fullyExpanded, fullyCollapsed;
		private void Fab_Click(object sender, EventArgs e)
		{
			if (fullyExpanded)
			{
				// FIXME в editTextInput пишутся имена комнат, количество символов может быть больше 3
				if (editTextInputFrom.Text.Length == 3 && editTextInputTo.Text.Length == 3)
				{
					InputMethodManager imm = (InputMethodManager)Activity.BaseContext.GetSystemService(Context.InputMethodService);
					imm.HideSoftInputFromWindow(View.WindowToken, 0);
					fab.SetImageResource(Resource.Drawable.ic_gps_fixed_black_24dp);
					appBar.SetExpanded(false);

					//fabLayoutParams.AnchorId = frameLayout.Id;
					//fab.LayoutParameters = fabLayoutParams;

					var fragmentWithMap = FragmentManager.FindFragmentByTag<MainBuildingMapFragment>("MAP_MAINBUILDING");
					List<GraphNode> route;
					try
					{
						route = Algorithms.CalculateRoute(mapGraph, editTextInputFrom.Text, editTextInputTo.Text);
					}
					catch (Algorithms.GraphRoutingException ex)
					{
						Toast.MakeText(Activity, ex.Message, ToastLength.Long).Show();
					}
					//fragmentWithMap.MapView.
				}
				else
				{
					Toast.MakeText(Activity.BaseContext, GetString(Resource.String.enter_correct_number), ToastLength.Short).Show();
				}
			}
			else
			if (fullyCollapsed)
			{
				fab.SetImageResource(Resource.Drawable.ic_done_black_24dp);
				appBar.SetExpanded(true);

				//fabLayoutParams.AnchorId = relativeLayout.Id;
				//fab.LayoutParameters = fabLayoutParams;
				//fab.SetImageResource(Resource.Drawable.ic_done_black_24dp);
			}
		}

		private void EditTextFromFocusChanged(object sender, View.FocusChangeEventArgs e)
		{
			if (!e.HasFocus)
			{
				editTextFromIsFocused = false;
			}
			else
			{
				editTextFromIsFocused = true;
			}
		}

		private void EditTextToFocusChanged(object sender, View.FocusChangeEventArgs e)
		{
			if (!e.HasFocus)
			{
				editTextToIsFocused = false;
			}
			else
			{
				editTextToIsFocused = true;
			}
		}

		public static bool CheckFocus()
		{
			return (editTextFromIsFocused && editTextToIsFocused);
		}

		public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
		{
			if (actionId == ImeAction.Go)
			{
				Toast.MakeText(Activity.BaseContext, "HELLO", ToastLength.Short).Show();
				appBar.SetExpanded(false);
			}
			return false;
		}

		public void OnOffsetChanged(AppBarLayout appBarLayout, int verticalOffset)
		{
			fullyExpanded = (verticalOffset == 0);
			fullyCollapsed = (appBar.Height + verticalOffset == 0);
			
			if (fullyCollapsed)
			{
				fab.SetImageResource(Resource.Drawable.ic_gps_fixed_black_24dp);
			}
			else if (fullyExpanded)
			{
				fab.SetImageResource(Resource.Drawable.ic_done_black_24dp);
			}
		}
	}
}