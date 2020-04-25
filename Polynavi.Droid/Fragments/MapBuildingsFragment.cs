using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using Google.Android.Material.AppBar;
using Google.Android.Material.FloatingActionButton;
using Itinero;
using Itinero.Profiles;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI;
using Mapsui.UI.Android;
using Mapsui.Utilities;
using Mapsui.Widgets;
using Mapsui.Widgets.ScaleBar;
using Polynavi.Droid.Activities;
using Polynavi.Droid.Extensions;
using Context = Android.Content.Context;
using Fragment = AndroidX.Fragment.App.Fragment;
using Vehicle = Itinero.Osm.Vehicles.Vehicle;

namespace Polynavi.Droid.Fragments
{
    // TODO Кеширование RouterDB чтобы не загружать ее при каждой загрузке фрагмента
    public class MapBuildingsFragment : Fragment, AppBarLayout.IOnOffsetChangedListener, ILocationListener
    {
        private const string RouterDbName = "polytech_map.routerdb";
        private const string MarkerAName = "ic_marker_a.png";
        private const string MarkerBName = "ic_marker_b.png";
        private const string MarkerLocationName = "ic_gps_fixed_black.png";
        private const string MyLocation = "Мое местоположение"; //TODO
        private const string MapCrs = "EPSG:3857";
        private const string ItineroLayerName = "Itinero Route"; //TODO
        private const string AnimatedPointsLayer = "Animated Points";
        internal const string MapActivityIntentResultName = "route";

        private readonly Point leftBottomMapPoint = new Point(30.356456, 59.994757);
        private readonly Point rightTopMapPoint = new Point(30.391848, 60.008916);

        private const int FromRequestCode = 1;
        private const int ToRequestCode = 2;

        private const int FineLocationRequestId = 10;

        private Router router;
        private Profile itineroProfile;

        private MapControl mapControl;
        private Map map;
        private ILayer routeLayer;

        private EditText editTextInputFrom, editTextInputTo;
        private AppBarLayout appBar;
        private FloatingActionButton drawRouteButton;
        private FloatingActionButton buttonLocation;
        private ImageButton buttonFromCurrentLocation;
        private LocationManager locationManager;
        private AnimatedPointsWithAutoUpdateLayer animatedLocation;
        private bool fullyExpanded, fullyCollapsed;

        public static Dictionary<string, Point> BuildingsWithCoordinates { get; } = new Dictionary<string, Point>()
        {
            { "Главный учебный корпус", new Point(60.00718, 30.37281) },
            { "Химический корпус", new Point(60.00648, 30.37630) },
            { "Механический корпус", new Point(60.00768, 30.37628) },
            { "Гидрокорпус-1", new Point(60.00565, 30.38176) },
            { "Гидрокорпус-2", new Point(60.00670, 30.38266) },
            { "НИК", new Point(60.005903, 30.379046) },
            { "1-й учебный корпус", new Point(60.00885, 30.37270) },
            { "2-й учебный корпус", new Point(60.00846, 30.37492) },
            { "3-й учебный корпус", new Point(60.00711, 30.38149) },
            { "4-й учебный корпус", new Point(60.00750, 30.37694) },
            { "5-й учебный корпус", new Point(59.99984, 30.37438) },
            { "6-й учебный корпус", new Point(60.00048, 30.36805) },
            { "9-й учебный корпус", new Point(60.00081, 30.36619) },
            { "10-й учебный корпус", new Point(60.00066, 30.36902) },
            { "11-й учебный корпус", new Point(60.00900, 30.37744) },
            { "15-й учебный корпус (ИМОП)", new Point(60.00689, 30.39065) },
            { "16-й учебный корпус", new Point(60.00790, 30.39041) },
            { "Спортивный комплекс", new Point(60.00295, 30.36801) },
            { "Лабораторный корпус", new Point(60.00734, 30.37954) },
            { "Гидробашня", new Point(60.00583, 30.37428) },
            { "НОЦ РАН", new Point(60.00317, 30.37468) },
            { "1-й профессорский корпус", new Point(60.00481, 30.37071) },
            { "2-й профессорский корпус", new Point(60.00475, 30.37796) },
            { "Дом ученых в Лесном", new Point(60.00448, 30.37908) },
            { "Секретариат приемной комиссии", new Point(60.009405, 30.371689) },
            { "ИПМЭиТ", new Point(59.994757, 30.356456) }
        };

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_map_buildings, container, false);

            appBar = view.FindViewById<AppBarLayout>(Resource.Id.appbar_map_buildings);
            appBar.AddOnOffsetChangedListener(this);

            drawRouteButton = view.FindViewById<FloatingActionButton>(Resource.Id.fab_map_buildings);
            drawRouteButton.Click += DrawRouteButton_Click;

            editTextInputFrom = view.FindViewById<EditText>(Resource.Id.edittext_input_from_map_builidngs);
            editTextInputFrom.Click += EditTextInputFrom_Click;

            editTextInputTo = view.FindViewById<EditText>(Resource.Id.edittext_input_to_map_builidngs);
            editTextInputTo.Click += EditTextInputTo_Click;

            buttonLocation = view.FindViewById<FloatingActionButton>(Resource.Id.fab_location_map_buildings);
            buttonLocation.Click += ButtonLocation_Click;

            buttonFromCurrentLocation =
                view.FindViewById<ImageButton>(Resource.Id.imagebutton_currentlocation_map_buildings);
            buttonFromCurrentLocation.Click += ButtonFromCurrentLocation_Click;

            var progress = view.FindViewById<ProgressBar>(Resource.Id.progressbar_map_buildings);
            progress.Visibility = ViewStates.Visible;

            mapControl = view.FindViewById<MapControl>(Resource.Id.mapControl);

            //TODO
            var tasks = new List<Task>
            {
                Task.Run(InitializeRouting),
                Task.Run(() =>
                {
                    InitializeMapControl();

                    Activity.RunOnUiThread(() =>
                    {
                        buttonLocation.Alpha = 0.7f;

                        if (IsLocationGranted())
                        {
                            SetupLocationManager();
                        }
                        else
                        {
                            RequestPermissions(new[] { Manifest.Permission.AccessFineLocation }, FineLocationRequestId);
                        }
                    });
                })
            };

            Task.WhenAll(tasks).ContinueWith(t => progress.Visibility = ViewStates.Invisible);

            return view;
        }

        private void InitializeRouting()
        {
            using (var ms = new MemoryStream())
            {
                using (var stream = Activity.BaseContext.Assets.Open(RouterDbName))
                {
                    stream.CopyTo(ms);
                }

                ms.Seek(0, SeekOrigin.Begin);
                var routerDb = RouterDb.Deserialize(ms);

                router = new Router(routerDb);
            }

            itineroProfile = Vehicle.Pedestrian.Shortest();
        }

        private void InitializeMapControl()
        {
            map = mapControl.Map;
            map.CRS = MapCrs;
            Activity.RunOnUiThread(() =>
            {
                map.Layers.Add(OpenStreetMap.CreateTileLayer());

                mapControl.Navigator.NavigateTo(new BoundingBox(leftBottomMapPoint.FromLonLat(),
                    rightTopMapPoint.FromLonLat()));

                map.Transformation = new MinimalTransformation();

                map.Limiter = new ViewportLimiter()
                {
                    PanLimits = new BoundingBox(leftBottomMapPoint.FromLonLat(), rightTopMapPoint.FromLonLat()), //TODO
                    ZoomLimits = new MinMax(1, 12) //TODO
                };

                routeLayer = new Layer();
                map.Layers.Add(routeLayer);
                map.Widgets.Add(new ScaleBarWidget(map)
                {
                    TextAlignment = Alignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top
                });
            });
        }

        private bool IsLocationGranted()
        {
            return ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation)
                   == Permission.Granted;
        }

        private void SetupLocationManager()
        {
            locationManager = (LocationManager)Activity.ApplicationContext.GetSystemService(Context.LocationService);
            locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 2, this);

            animatedLocation = new AnimatedPointsWithAutoUpdateLayer { Name = AnimatedPointsLayer };
            map.Layers.Add(animatedLocation);
        }

        private void ButtonFromCurrentLocation_Click(object sender, EventArgs e)
        {
            if (IsLocationGranted())
            {
                var lastLocation = locationManager.GetLastKnownLocation(LocationManager.GpsProvider);

                if (!IsValidLocation(lastLocation))
                {
                    return;
                }

                var currentLocation = new Point(lastLocation.Latitude, lastLocation.Longitude);

                editTextInputFrom.Text = MyLocation;
                BuildingsWithCoordinates[MyLocation] = currentLocation;
            }
            else
            {
                Toast.MakeText(Activity.BaseContext, GetString(Resource.String.no_location_permission_buildings),
                    ToastLength.Long).Show();
            }
        }

        private void ButtonLocation_Click(object sender, EventArgs e)
        {
            if (IsLocationGranted())
            {
                var lastLocation = locationManager.GetLastKnownLocation(LocationManager.GpsProvider);

                if (!IsValidLocation(lastLocation))
                {
                    return;
                }

                var currentLocation = new Point(lastLocation.Longitude, lastLocation.Latitude).FromLonLat();

                mapControl.Navigator.NavigateTo(currentLocation, map.Resolutions[0]);
            }
            else
            {
                Toast.MakeText(Activity.BaseContext, GetString(Resource.String.no_location_permission_buildings),
                    ToastLength.Long).Show();
            }
        }

        private bool IsValidLocation(Location location)
        {
            var delta = SystemClock.ElapsedRealtime() - location?.ElapsedRealtimeNanos / 1000000;

            return location != null && delta < 5000 && IsInBounds(location);
        }

        private bool IsInBounds(Location location)
        {
            return leftBottomMapPoint.X < location.Longitude && leftBottomMapPoint.Y < location.Latitude
                                                             && rightTopMapPoint.X > location.Longitude &&
                                                             rightTopMapPoint.Y > location.Latitude;
        }

        public override void OnDetach()
        {
            base.OnStop();
            locationManager?.RemoveUpdates(this);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            switch (requestCode)
            {
                case FineLocationRequestId:
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        SetupLocationManager();
                        buttonLocation.Enabled = true;
                        buttonFromCurrentLocation.Enabled = true;
                    }

                    break;
            }
        }

        private static ILayer GetRouteLayer(Route route)
        {
            return new Layer(ItineroLayerName)
            {
                Name = ItineroLayerName,
                DataSource = new MemoryProvider(GenerateLinesAndMarkers(route)),
                Style = null
            };
        }

        //TODO
        private static IEnumerable<IFeature> GenerateLinesAndMarkers(Route route)
        {
            var line = new LineString
            {
                Vertices = (
                    from c in route.Shape
                    select SphericalMercator.FromLonLat(c.Longitude, c.Latitude)).ToList()
            };

            var lineFeature = new Feature()
            {
                Geometry = line,
                Styles = new List<IStyle>()
                {
                    new SymbolStyle()
                    {
                        Line = new Pen()
                        {
                            Color = Color.Blue,
                            PenStyle = PenStyle.Solid,
                            Width = 7,
                            PenStrokeCap = PenStrokeCap.Round
                        },
                        Opacity = 0.7f
                    }
                }
            };

            var markerStartFeature = new Feature()
            {
                Geometry = route.Stops[0].Coordinate.ToWorld(),
                Styles = new List<IStyle>()
                {
                    new SymbolStyle()
                    {
                        BitmapId = GetBitmapIdForEmbeddedResource(MarkerAName), SymbolScale = 0.5
                    }
                }
            };

            var markerFinishFeature = new Feature()
            {
                Geometry = route.Stops[1].Coordinate.ToWorld(),
                Styles = new List<IStyle>()
                {
                    new SymbolStyle()
                    {
                        BitmapId = GetBitmapIdForEmbeddedResource(MarkerBName), SymbolScale = 0.5
                    }
                }
            };

            return new List<Feature> { lineFeature, markerStartFeature, markerFinishFeature };
        }

        private static int GetBitmapIdForEmbeddedResource(string resourceName)
        {
            var image = Utils.Utils.GetEmbeddedResourceStream($"Images.{resourceName}");
            return BitmapRegistry.Instance.Register(image);
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (resultCode != (int)Result.Ok)
            {
                return;
            }

            switch (requestCode)
            {
                case FromRequestCode:
                    editTextInputFrom.Text = data.GetStringExtra(MapActivityIntentResultName);
                    break;
                case ToRequestCode:
                    editTextInputTo.Text = data.GetStringExtra(MapActivityIntentResultName);
                    break;
                default:
                    Toast.MakeText(Activity.BaseContext, "Error", ToastLength.Short).Show();
                    break;
            }
        }

        private void DrawRouteButton_Click(object sender, EventArgs args)
        {
            if (fullyExpanded)
            {
                CheckInputAndDrawRoute();
            }

            ToggleAppBarAndChangeButtonIcon();
        }

        private void CheckInputAndDrawRoute()
        {
            if (editTextInputFrom.Text.Any() && editTextInputTo.Text.Any())
            {
                var routeNames = new[] { editTextInputFrom.Text, editTextInputTo.Text };
                var startName = routeNames[0];
                var finishName = routeNames[1];

                var startPoint = BuildingsWithCoordinates[startName];
                var finishPoint = BuildingsWithCoordinates[finishName];
                var start = router.Resolve(itineroProfile, (float)startPoint.X,
                    (float)startPoint.Y); //TODO ResolveFailedException Probably too far...
                var finish = router.Resolve(itineroProfile, (float)finishPoint.X, (float)finishPoint.Y);

                var route = router.Calculate(itineroProfile, start, finish);

                map.Layers.Remove(routeLayer);
                routeLayer = GetRouteLayer(route);
                map.Layers.Add(routeLayer);
                mapControl.Navigator.NavigateTo(new Point(startPoint.Y, startPoint.X).FromLonLat(), map.Resolutions[0]);
            }
            else
            {
                Toast.MakeText(Activity.BaseContext, GetString(Resource.String.title_route_activity),
                    ToastLength.Short).Show();
            }
        }

        private void ToggleAppBarAndChangeButtonIcon()
        {
            appBar.SetExpanded(!fullyExpanded);
        }

        private void EditTextInputFrom_Click(object sender, EventArgs e)
        {
            StartRouteActivity(FromRequestCode);
        }

        private void EditTextInputTo_Click(object sender, EventArgs e)
        {
            StartRouteActivity(ToRequestCode);
        }

        private void StartRouteActivity(int requestCode)
        {
            var searchActivity = new Intent(Activity, typeof(MapRouteActivity));
            StartActivityForResult(searchActivity, requestCode);
        }

        public void OnOffsetChanged(AppBarLayout appBarLayout, int verticalOffset)
        {
            fullyExpanded = verticalOffset == 0;
            fullyCollapsed = appBar.Height + verticalOffset == 0;

            if (fullyCollapsed)
            {
                drawRouteButton.SetImageResource(Resource.Drawable.ic_directions_black);
            }
            else if (fullyExpanded)
            {
                drawRouteButton.SetImageResource(Resource.Drawable.ic_done_black);
            }
        }

        public void OnLocationChanged(Location location)
        {
            animatedLocation.UpdateLocation(location);
        }

        public void OnProviderDisabled(string provider)
        {
            map.Layers.Remove(animatedLocation);
        }

        public void OnProviderEnabled(string provider)
        {
            map.Layers.Add(animatedLocation);
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
        }

        private class AnimatedPointsWithAutoUpdateLayer : AnimatedPointLayer
        {
            private static IGeometry geometry;

            public AnimatedPointsWithAutoUpdateLayer()
                : base(new DynamicMemoryProvider())
            {
                Style = new SymbolStyle()
                {
                    BitmapId = GetBitmapIdForEmbeddedResource(MarkerLocationName), SymbolScale = 0.5
                };
            }

            public void UpdateLocation(Location location)
            {
                geometry = SphericalMercator.FromLonLat(location.Longitude, location.Latitude);
                UpdateData();
            }

            private class DynamicMemoryProvider : MemoryProvider
            {
                public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
                {
                    var features = new List<IFeature>();
                    var feature = new Feature { Geometry = geometry, ["ID"] = 0 };

                    features.Add(feature);

                    return features;
                }
            }
        }
    }
}
