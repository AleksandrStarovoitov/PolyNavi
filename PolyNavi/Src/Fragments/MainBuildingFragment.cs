using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.Fragment.App;
using Google.Android.Material.AppBar;
using Google.Android.Material.FloatingActionButton;
using Graph;
using Java.Lang;
using PolyNavi.Views;
using static Android.Widget.TextView;
using Point = Android.Graphics.Point;

namespace PolyNavi.Fragments
{
    public class MainBuildingFragment : Fragment, IOnEditorActionListener, AppBarLayout.IOnOffsetChangedListener, ITextWatcher
    {
        private GraphNode mapGraph;
        private View view;
        private AutoCompleteTextView editTextInputFrom, editTextInputTo;
        private FragmentTransaction fragmentTransaction;
        private AppBarLayout appBar;
        private FloatingActionButton fab;
        private FloatingActionButton upButton, downButton;
        private List<MainBuildingMapFragment> fragments;
        private int currentFloor = 1;
        private static bool editTextFromIsFocused, editTextToIsFocused;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mapGraph = MainApp.Instance.MainBuildingGraph.Value;

            view = inflater.Inflate(Resource.Layout.fragment_mainbuilding, container, false);

            fragments = new List<MainBuildingMapFragment>()
            {
                new MainBuildingMapFragment(Resource.Drawable.first_floor), 
                new MainBuildingMapFragment(Resource.Drawable.second_floor),
                new MainBuildingMapFragment(Resource.Drawable.third_floor)
            };

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
            editTextInputFrom.AddTextChangedListener(this);

            editTextInputTo = view.FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteTextView_to);
            editTextInputTo.SetOnEditorActionListener(this);
            editTextInputTo.FocusChange += EditTextToFocusChanged;
            editTextInputTo.Adapter = new ArrayAdapter(Activity.BaseContext, Android.Resource.Layout.SimpleDropDownItem1Line, array);
            editTextInputTo.AddTextChangedListener(this);

            appBar = view.FindViewById<AppBarLayout>(Resource.Id.appbar_mainbuilding);
            appBar.AddOnOffsetChangedListener(this);

            fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab_mainbuilding);
            fab.Click += Fab_Click;

            var rl = view.FindViewById<RelativeLayout>(Resource.Id.relativelayout_floor_buttons_mainbuilding);
            rl.BringToFront();

            upButton = view.FindViewById<FloatingActionButton>(Resource.Id.fab_up_mainbuilding);
            upButton.Click += UpButtonClick;
            upButton.Alpha = 0.7f;

            downButton = view.FindViewById<FloatingActionButton>(Resource.Id.fab_down_mainbuilding);
            downButton.Click += DownButtonClick;
            downButton.Alpha = 0.7f;
            downButton.Enabled = false;

            return view;
        }

        private void UpButtonClick(object sender, EventArgs e)
        {
            ChangeFloor(currentFloor + 1);
        }

        private void DownButtonClick(object sender, EventArgs e)
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
                throw new ArgumentOutOfRangeException(nameof(newFloor), newFloor, "Не валидный номер этажа");
            }
            downButton.Enabled = true;
            upButton.Enabled = true;
            var currentFragment = (MainBuildingMapFragment)FragmentManager.FindFragmentByTag($"MAP_MAINBUILDING_{currentFloor}");
            var currentView = currentFragment.MapView;

            var newFragment = (MainBuildingMapFragment)FragmentManager.FindFragmentByTag($"MAP_MAINBUILDING_{newFloor}");
            var newView = newFragment.MapView;

            newView.PosX = currentView.PosX;
            newView.PosY = currentView.PosY;

            FragmentManager.BeginTransaction().
                            Detach(currentFragment).
                            Attach(newFragment).
                            Commit();
            currentFloor = newFloor;
            switch (currentFloor)
            {
                case 3:
                    upButton.Enabled = false;
                    break;
                case 1:
                    downButton.Enabled = false;
                    break;
            }
        }

        private void ClearAllRoutes()
        {
            foreach (var fragment in fragments)
            {
                fragment.MapView.SetRoute(null);
                fragment.MapView.SetMarker(new Point(), MainBuildingView.Marker.None);
            }
        }

        private void Fab_Click(object sender, EventArgs e)
        {
            DrawRoute();
        }

        private bool fullyExpanded, fullyCollapsed;
        private void DrawRoute()
        {
            if (fullyExpanded)
            {
                if (editTextInputFrom.Text != editTextInputTo.Text & MainApp.Instance.RoomsDictionary.TryGetValue(editTextInputFrom.Text, out var startName) & MainApp.Instance.RoomsDictionary.TryGetValue(editTextInputTo.Text, out var finishName))
                {
                    var imm = (InputMethodManager)Activity.BaseContext.GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(View.WindowToken, 0);
                    fab.SetImageResource(Resource.Drawable.ic_directions_black);
                    appBar.SetExpanded(false);

                    try
                    {
                        var route = Algorithms.CalculateRoute(mapGraph, startName, finishName);
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
                                                   Coordinates = from graphNode in g select new Point(graphNode.Point.X, graphNode.Point.Y)
                                               };
                        ClearAllRoutes();
                        foreach (var coordinateGroup in coordinateGroups)
                        {
                            var fragment = FragmentManager.FindFragmentByTag($"MAP_MAINBUILDING_{coordinateGroup.Floor.FloorNumber}") as MainBuildingMapFragment;
                            fragment?.MapView.SetRoute(coordinateGroup.Coordinates.ToList());
                        }

                        var startFloor = route[0].FloorNumber;
                        var endFloor = route.Last().FloorNumber;
                        fragments[startFloor - 1].MapView.SetMarker(new Point(route.First().Point.X, route.First().Point.Y), MainBuildingView.Marker.Start);
                        fragments[endFloor - 1].MapView.SetMarker(new Point(route.Last().Point.X, route.Last().Point.Y), MainBuildingView.Marker.End);

                        ChangeFloor(startFloor);
                    }
                    catch (GraphRoutingException ex)
                    {
                        Toast.MakeText(Activity, ex.Message, ToastLength.Long).Show();
                    }
                }
                else
                {
                    if (startName == null)
                    {
                        editTextInputFrom.Error = GetString(Resource.String.wrong_room);
                    }
                    if (finishName == null)
                    {
                        editTextInputTo.Error = GetString(Resource.String.wrong_room);
                    }
                }
            }
            else if (fullyCollapsed)
            {
                fab.SetImageResource(Resource.Drawable.ic_done_black);
                appBar.SetExpanded(true);
            }
        }
        private void EditTextFromFocusChanged(object sender, View.FocusChangeEventArgs e)
        {
            editTextInputFrom.ShowDropDown();
            editTextFromIsFocused = !e.HasFocus;
        }

        private void EditTextToFocusChanged(object sender, View.FocusChangeEventArgs e)
        {
            editTextInputTo.ShowDropDown();
            editTextToIsFocused = !e.HasFocus;
        }

        public static bool CheckFocus()
        {
            return (editTextFromIsFocused && editTextToIsFocused);
        }

        public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
        {
            if (actionId == ImeAction.Go)
            {
                DrawRoute();
            }
            return false;
        }

        public void OnOffsetChanged(AppBarLayout appBarLayout, int verticalOffset)
        {
            fullyExpanded = (verticalOffset == 0);
            fullyCollapsed = (appBar.Height + verticalOffset == 0);

            if (fullyCollapsed)
            {
                fab.SetImageResource(Resource.Drawable.ic_directions_black);
            }
            else if (fullyExpanded)
            {
                fab.SetImageResource(Resource.Drawable.ic_done_black);
            }
        }

        public void AfterTextChanged(IEditable s)
        {
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            if (editTextInputFrom.Error != null)
            {
                editTextInputFrom.Error = null;
            }

            if (editTextInputTo.Error != null)
            {
                editTextInputTo.Error = null;
            }
        }
    }
}