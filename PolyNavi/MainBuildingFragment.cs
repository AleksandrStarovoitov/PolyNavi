using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using static Android.Widget.TextView;

namespace PolyNavi
{
	public class MainBuildingFragment : Fragment, IOnEditorActionListener
	{
		private View view;
		private EditText editTextInputTo, editTextInputFrom;
		private FragmentTransaction fragmentTransaction;
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

			return view;
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
			return false;
		}
	}
}