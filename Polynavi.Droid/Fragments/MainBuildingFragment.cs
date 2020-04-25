using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Polynavi.Common.Exceptions;
using Polynavi.Droid.Views;
using Point = Android.Graphics.Point;

namespace Polynavi.Droid.Fragments
{
    public class MainBuildingFragment : Fragment, TextView.IOnEditorActionListener, AppBarLayout.IOnOffsetChangedListener,
        ITextWatcher
    {
        private static readonly GraphNode mainBuildingGraph = CreateGraph();
        private static Dictionary<string, string> roomsDictionary;
        private View view;
        private AutoCompleteTextView editTextInputFrom, editTextInputTo;
        private AppBarLayout appBar;
        private FloatingActionButton drawRouteButton;
        private FloatingActionButton upButton, downButton;

        private readonly List<MainBuildingMapFragment> fragments = new List<MainBuildingMapFragment>()
        {
            new MainBuildingMapFragment(Resource.Drawable.first_floor),
            new MainBuildingMapFragment(Resource.Drawable.second_floor),
            new MainBuildingMapFragment(Resource.Drawable.third_floor)
        };

        private int currentFloor = 1;
        private bool fullyExpanded, fullyCollapsed;
        private static bool editTextFromIsFocused, editTextToIsFocused;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_mainbuilding, container, false);

            var fragmentTransaction = Activity.SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.frame_mainbuilding, fragments[2], "MAP_MAINBUILDING_3");
            fragmentTransaction.Add(Resource.Id.frame_mainbuilding, fragments[1], "MAP_MAINBUILDING_2");
            fragmentTransaction.Detach(fragments[2]); //TODO ?
            fragmentTransaction.Detach(fragments[1]);
            fragmentTransaction.Add(Resource.Id.frame_mainbuilding, fragments[0], "MAP_MAINBUILDING_1");
            fragmentTransaction.Commit();

            var array = roomsDictionary.Select(x => x.Key).ToArray();
            editTextInputFrom = view.FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteTextView_from);
            editTextInputFrom.FocusChange += EditTextFromFocusChanged;
            editTextInputFrom.Adapter = new ArrayAdapter(Activity.BaseContext,
                Android.Resource.Layout.SimpleDropDownItem1Line, array);
            editTextInputFrom.AddTextChangedListener(this);

            editTextInputTo = view.FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteTextView_to);
            editTextInputTo.SetOnEditorActionListener(this);
            editTextInputTo.FocusChange += EditTextToFocusChanged;
            editTextInputTo.Adapter = new ArrayAdapter(Activity.BaseContext,
                Android.Resource.Layout.SimpleDropDownItem1Line, array);
            editTextInputTo.AddTextChangedListener(this);

            appBar = view.FindViewById<AppBarLayout>(Resource.Id.appbar_mainbuilding);
            appBar.AddOnOffsetChangedListener(this);

            drawRouteButton = view.FindViewById<FloatingActionButton>(Resource.Id.fab_mainbuilding);
            drawRouteButton.Click += DrawRouteButton_Click;

            var changeFloorButtonsRelativeLayout =
                view.FindViewById<RelativeLayout>(Resource.Id.relativelayout_floor_buttons_mainbuilding);
            changeFloorButtonsRelativeLayout.BringToFront();

            upButton = view.FindViewById<FloatingActionButton>(Resource.Id.fab_up_mainbuilding);
            upButton.Click += UpButton_Click;
            upButton.Alpha = 0.7f;

            downButton = view.FindViewById<FloatingActionButton>(Resource.Id.fab_down_mainbuilding);
            downButton.Click += DownButton_Click;
            downButton.Alpha = 0.7f;
            downButton.Enabled = false;

            return view;
        }

        private void UpButton_Click(object sender, EventArgs e)
        {
            ChangeFloor(currentFloor + 1);
        }

        private void DownButton_Click(object sender, EventArgs e)
        {
            ChangeFloor(currentFloor - 1);
        }

        /// <summary>Start index = 1</summary>
        /// <param name="newFloor"></param>
        private void ChangeFloor(int newFloor)
        {
            if (newFloor < 1 && newFloor > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(newFloor), newFloor, "Invalid floor number");
            }

            downButton.Enabled = true;
            upButton.Enabled = true;
            var currentFragment =
                (MainBuildingMapFragment)FragmentManager.FindFragmentByTag($"MAP_MAINBUILDING_{currentFloor}");
            var currentView = currentFragment.MapView;

            var newFragment =
                (MainBuildingMapFragment)FragmentManager.FindFragmentByTag($"MAP_MAINBUILDING_{newFloor}");
            var newView = newFragment.MapView;

            newView.PosX = currentView.PosX;
            newView.PosY = currentView.PosY;

            FragmentManager.BeginTransaction()
                .Detach(currentFragment)
                .Attach(newFragment)
                .Commit();

            currentFloor = newFloor;
            switch (currentFloor)
            {
                case 1:
                    downButton.Enabled = false;
                    break;

                case 3:
                    upButton.Enabled = false;
                    break;
            }
        }

        private void DrawRouteButton_Click(object sender, EventArgs e)
        {
            if (fullyExpanded)
            {
                CheckInputAndDrawRoute();
            }
            else if (fullyCollapsed)
            {
                ToggleAppBarAndChangeButtonIcon();
            }
        }

        private static GraphNode CreateGraph()
        {
            var graphService = AndroidDependencyContainer.Instance.GraphService;

            GraphNode graphNode;

            var path = Utils.Utils.GetFileFullPath(MainApp.MainGraphFilename);
            
            if (File.Exists(path) && !Utils.Utils.IsAppUpdated())
            {
                using var stream = File.OpenRead(path);

                graphNode = graphService.Load(stream);
            }
            else
            {
                graphNode = graphService.LoadFromXmlDescriptor(MainApp.MainGraphXmlFilename);

                using var stream = File.Create(Utils.Utils.GetFileFullPath(MainApp.MainGraphFilename));
                graphService.Save(stream, graphNode);
            }

            FillRoomsDictionary(graphNode);

            return graphNode;
        }

        private static void FillRoomsDictionary(GraphNode graph)
        {
            var ids = new List<GraphNode>();
            roomsDictionary = new Dictionary<string, string>();
            var bfsQueue = new Queue<GraphNode>();
            bfsQueue.Enqueue(graph);

            while (bfsQueue.Count > 0)
            {
                var node = bfsQueue.Dequeue();

                foreach (var neighbour in node.Neighbours.Where(neighbour => !ids.Contains(neighbour)))
                {
                    ids.Add(neighbour);
                    bfsQueue.Enqueue(neighbour);
                    if (neighbour.RoomName.Equals("*Unknown*"))
                    {
                        continue;
                    }

                    var name = neighbour.RoomName
                        .Replace("1_1", "1 эт. 1").Replace("1_2", "1 эт. 2")
                        .Replace("2_1", "2 эт. 1").Replace("2_2", "2 эт. 2")
                        .Replace("1_3", "1 эт. 3").Replace("2_3", "2 эт. 3")
                        .Replace("101а", "101 (а)").Replace("_", " ");
                    roomsDictionary[name] = neighbour.RoomName;
                }
            }

            var ordered = roomsDictionary.OrderBy(x => x.Value, new MainApp.DictionaryComp());
            roomsDictionary = ordered.ToDictionary(x => x.Key, x => x.Value);
        }

        private void CheckInputAndDrawRoute()
        {
            string startName = null, finishName = null;

            Utils.Utils.HideKeyboard(View, Activity);

            ToggleAppBarAndChangeButtonIcon();

            try
            {
                if (editTextInputFrom.Text == editTextInputTo.Text && editTextInputFrom.Text.Any())
                {
                    throw new SameRoomsSelectedException();
                }

                startName = roomsDictionary[editTextInputFrom.Text];
                finishName = roomsDictionary[editTextInputTo.Text];

                CalculateAndDrawRoute(startName, finishName);
            }
            catch (GraphRoutingException)
            {
                Toast.MakeText(Activity, "Error", ToastLength.Long).Show(); //TODO
            }
            catch (KeyNotFoundException)
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
            catch (SameRoomsSelectedException)
            {
                Toast.MakeText(Activity, "Same rooms were selected!", ToastLength.Long).Show(); //TODO
            }
        }

        private void ToggleAppBarAndChangeButtonIcon()
        {
            appBar.SetExpanded(!fullyExpanded);
        }

        private void CalculateAndDrawRoute(string startName, string finishName)
        {
            var route = Algorithms.CalculateRoute(mainBuildingGraph, startName, finishName);

            var coordinateGroups = route.GroupBy(node => new { node.FloorNumber, node.FloorPartNumber })
                .Select(g => new
                {
                    Floor = g.Key,
                    Coordinates = g.Select(graphNode => new Point(graphNode.Point.X, graphNode.Point.Y))
                });

            ClearAllRoutes();

            foreach (var coordinateGroup in coordinateGroups)
            {
                var fragment = FragmentManager
                    .FindFragmentByTag($"MAP_MAINBUILDING_{coordinateGroup.Floor.FloorNumber}") as MainBuildingMapFragment;
                fragment?.MapView.SetRoute(coordinateGroup.Coordinates.ToList());
            }

            var startFloor = route[0].FloorNumber;
            var endFloor = route.Last().FloorNumber;
            fragments[startFloor - 1].MapView.SetMarker(new Point(route.First().Point.X, route.First().Point.Y),
                MainBuildingView.Marker.Start);
            fragments[endFloor - 1].MapView.SetMarker(new Point(route.Last().Point.X, route.Last().Point.Y),
                MainBuildingView.Marker.End);

            ChangeFloor(startFloor);

            //TODO Pan to start point
        }

        private void ClearAllRoutes()
        {
            foreach (var fragment in fragments)
            {
                fragment.MapView.SetRoute(null);
                fragment.MapView.SetMarker(new Point(), MainBuildingView.Marker.None);
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
            return editTextFromIsFocused && editTextToIsFocused;
        }

        public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
        {
            if (IsGoButtonPressed())
            {
                CheckInputAndDrawRoute();
            }

            return false;

            bool IsGoButtonPressed() => actionId == ImeAction.Go;
        }

        public void OnOffsetChanged(AppBarLayout appBarLayout, int verticalOffset)
        {
            fullyExpanded = verticalOffset == 0;
            fullyCollapsed = (appBar.Height + verticalOffset) == 0;

            if (fullyCollapsed)
            {
                drawRouteButton.SetImageResource(Resource.Drawable.ic_directions_black);
            }
            else if (fullyExpanded)
            {
                drawRouteButton.SetImageResource(Resource.Drawable.ic_done_black);
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
            ClearInputErrorMessages();
        }

        private void ClearInputErrorMessages()
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
