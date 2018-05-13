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
using System.Linq;

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
		private List<MainBuildingMapFragment> fragments;
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

			fragments = new List<MainBuildingMapFragment>() { new MainBuildingMapFragment(Resource.Drawable.first_floor), new MainBuildingMapFragment(Resource.Drawable.second_floor), new MainBuildingMapFragment(Resource.Drawable.third_floor) };

			fragmentTransaction = FragmentManager.BeginTransaction();
			fragmentTransaction.Add(Resource.Id.frame_mainbuilding, fragments[2], "MAP_MAINBUILDING_3");
			fragmentTransaction.Add(Resource.Id.frame_mainbuilding, fragments[1], "MAP_MAINBUILDING_2");
			fragmentTransaction.Detach(fragments[2]);
			fragmentTransaction.Detach(fragments[1]);
			fragmentTransaction.Add(Resource.Id.frame_mainbuilding, fragments[0], "MAP_MAINBUILDING_1");
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
				fragmentTransaction = FragmentManager.BeginTransaction();
				fragmentTransaction.Detach(fragments[currentFloor - 2]);
				fragmentTransaction.Attach(fragments[currentFloor - 1]);
				fragmentTransaction.Commit();
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
				fragmentTransaction = FragmentManager.BeginTransaction();
				fragmentTransaction.Detach(fragments[currentFloor]);
				fragmentTransaction.Attach(fragments[currentFloor - 1]);
				fragmentTransaction.Commit();
			}
			if (currentFloor == 1)
			{
				buttonDown.Enabled = false;
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

					var fragmentWithMap = FragmentManager.FindFragmentByTag<MainBuildingMapFragment>($"MAP_MAINBUILDING_{currentFloor}");
					List<GraphNode> route;
					try
					{
						route = Algorithms.CalculateRoute(mapGraph, editTextInputFrom.Text, editTextInputTo.Text);
						fragmentWithMap.MapView.SetRoute(route.Select(gnode => new Android.Graphics.Point(gnode.Point.X, gnode.Point.Y)).ToList());
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