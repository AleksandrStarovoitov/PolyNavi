using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using System;
using Android.Support.V4.App;

using static Android.Widget.TextView;

using Graph;
using System.Collections.Generic;
using System.Linq;

namespace PolyNavi
{
	public class MainBuildingFragment : Android.Support.V4.App.Fragment, IOnEditorActionListener, AppBarLayout.IOnOffsetChangedListener
	{
		GraphNode mapGraph;

		private View view;
		private AutoCompleteTextView editTextInputFrom, editTextInputTo;
		private Android.Support.V4.App.FragmentTransaction fragmentTransaction;
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

			fragmentTransaction = Activity.SupportFragmentManager.BeginTransaction();
			fragmentTransaction.Add(Resource.Id.frame_mainbuilding, fragments[2], "MAP_MAINBUILDING_3");
			fragmentTransaction.Add(Resource.Id.frame_mainbuilding, fragments[1], "MAP_MAINBUILDING_2");
			fragmentTransaction.Detach(fragments[2]);
			fragmentTransaction.Detach(fragments[1]);
			fragmentTransaction.Add(Resource.Id.frame_mainbuilding, fragments[0], "MAP_MAINBUILDING_1");
			fragmentTransaction.Commit();

			var array = MainApp.Instance.RoomsDictionary.Select(x => x.Key).ToArray();
			editTextInputFrom = view.FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteTextView_from);
			editTextInputFrom.FocusChange += EditTextFromFocusChanged;
			editTextInputFrom.Adapter = new ArrayAdapter(Activity.BaseContext, Android.Resource.Layout.SimpleDropDownItem1Line, array);

			editTextInputTo = view.FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteTextView_to);
			editTextInputTo.SetOnEditorActionListener(this);
			editTextInputTo.FocusChange += EditTextToFocusChanged;
			editTextInputTo.Adapter = new ArrayAdapter(Activity.BaseContext, Android.Resource.Layout.SimpleDropDownItem1Line, array);

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
			ChangeFloor(currentFloor + 1);
		}

		private void ButtonDown_Click(object sender, EventArgs e)
		{
			ChangeFloor(currentFloor - 1);
		}

		/// <summary>
		/// Индексация с единицы
		/// </summary>
		/// <param name="newFloor"></param>
		private void ChangeFloor(int newFloor)
		{
			if (newFloor < 1 && newFloor > 3)
			{
				throw new ArgumentOutOfRangeException("newFloor", newFloor, "Не валидный номер этажа");
			}
			buttonDown.Enabled = true;
			buttonUp.Enabled = true;
			var currentFragment = FragmentManager.FindFragmentByTag($"MAP_MAINBUILDING_{currentFloor}");
			var newFragment = FragmentManager.FindFragmentByTag($"MAP_MAINBUILDING_{newFloor}");
			FragmentManager.BeginTransaction().
							Detach(currentFragment).
							Attach(newFragment).
							Commit();
			currentFloor = newFloor;
			if (currentFloor == 3)
			{
				buttonUp.Enabled = false;
			}
			else if (currentFloor == 1)
			{
				buttonDown.Enabled = false;
			}
		}

		private void ClearAllRoutes()
		{
			foreach (var fragment in fragments)
			{
				fragment.MapView.SetRoute(null);
				fragment.MapView.SetMarker(new Android.Graphics.Point(), MainBuildingView.Marker.None);
			}
		}

		bool fullyExpanded, fullyCollapsed;
		private void Fab_Click(object sender, EventArgs e)
		{
			if (fullyExpanded)
			{
                if (MainApp.Instance.RoomsDictionary.TryGetValue(editTextInputFrom.Text, out string startName) && MainApp.Instance.RoomsDictionary.TryGetValue(editTextInputTo.Text, out string finishName))
                {
                    InputMethodManager imm = (InputMethodManager)Activity.BaseContext.GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(View.WindowToken, 0);
                    fab.SetImageResource(Resource.Drawable.ic_directions_black_24dp);
                    appBar.SetExpanded(false);

                    //fabLayoutParams.AnchorId = frameLayout.Id;
                    //fab.LayoutParameters = fabLayoutParams;

                    List<GraphNode> route;
                    try
                    {
                        route = Algorithms.CalculateRoute(mapGraph, startName, finishName);
                        var coordinateGroups = from node in route
                                               group node by new
                                               {
                                                   node.FloorNumber,
                                                   node.FloorPartNumber
                                               }
                                               into g
                                               select new
                                               {
                                                   Floor = g.Key,
                                                   Coordinates = from gnode in g select new Android.Graphics.Point(gnode.Point.X, gnode.Point.Y),
                                               };
                        ClearAllRoutes();
                        foreach (var coordGroup in coordinateGroups)
                        {
                            var fragment = FragmentManager.FindFragmentByTag($"MAP_MAINBUILDING_{coordGroup.Floor.FloorNumber}") as MainBuildingMapFragment;
                            fragment.MapView.SetRoute(coordGroup.Coordinates.ToList());
                        }

                        int startFloor = route[0].FloorNumber;
                        int endFloor = route.Last().FloorNumber;
                        fragments[startFloor - 1].MapView.SetMarker(new Android.Graphics.Point(route.First().Point.X, route.First().Point.Y), MainBuildingView.Marker.Start);
                        fragments[endFloor - 1].MapView.SetMarker(new Android.Graphics.Point(route.Last().Point.X, route.Last().Point.Y), MainBuildingView.Marker.End);
                        
                        ChangeFloor(startFloor);
                    }
                    catch (GraphRoutingException ex)
                    {
                        Toast.MakeText(Activity, ex.Message, ToastLength.Long).Show();
                    }
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
			editTextInputFrom.ShowDropDown();
			if (e.HasFocus)
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
			editTextInputTo.ShowDropDown();
			if (e.HasFocus)
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
				fab.SetImageResource(Resource.Drawable.ic_directions_black_24dp);
			}
			else if (fullyExpanded)
			{
				fab.SetImageResource(Resource.Drawable.ic_done_black_24dp);
			}
		}
	}
}