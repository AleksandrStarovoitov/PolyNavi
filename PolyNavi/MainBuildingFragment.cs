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

namespace PolyNavi
{
	public class MainBuildingFragment : Fragment, IOnEditorActionListener, AppBarLayout.IOnOffsetChangedListener
	{
		private View view;
		private EditText editTextInputFrom, editTextInputTo;
		private TextInputLayout textInputLayoutFrom, textInputLayoutTo;
		private FragmentTransaction fragmentTransaction;
		private AppBarLayout appBar;
		private FloatingActionButton fab;
		private RelativeLayout relativeLayout;
		private AppBarLayout.LayoutParams relativeLayoutParams;
		private FrameLayout frameLayout;
		private CoordinatorLayout.LayoutParams fabLayoutParams;

		static bool editTextFromIsFocused, editTextToIsFocused;
		

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.fragment_mainbuilding, container, false);

			fragmentTransaction = FragmentManager.BeginTransaction();
			fragmentTransaction.Add(Resource.Id.frame_mainbuilding, new MainBuildingMapFragment());
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

			return view;
		}

		bool expanded = true, clicked = false;
		bool fullyExpanded, fullyCollapsed;
		private void Fab_Click(object sender, EventArgs e)
		{
			clicked = true;
			if (fullyExpanded)
			{
				if (editTextInputFrom.Text.Length == 3 && editTextInputTo.Text.Length == 3)
				{
					InputMethodManager imm = (InputMethodManager)Activity.BaseContext.GetSystemService(Context.InputMethodService);
					imm.HideSoftInputFromWindow(View.WindowToken, 0);
					fab.SetImageResource(Resource.Drawable.ic_gps_fixed_black_24dp);
					appBar.SetExpanded(false);

					//fabLayoutParams.AnchorId = frameLayout.Id;
					//fab.LayoutParameters = fabLayoutParams;
				}
				else
				{
					Toast.MakeText(Activity.BaseContext, "Введите корректный номер", ToastLength.Short).Show();
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
			clicked = false;
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