using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace PolyNavi
{
    [Activity(Label = "AutoCompleteTextViewPreference")]
    public class AutoCompleteTextViewPreference : EditTextPreference
    {
        int resourceId = Resource.Layout.preference_dialog_autocomplete;
        public string GroupName { get; set; }

        public AutoCompleteTextViewPreference(Context context) : this(context, null)
        {
        }

        public AutoCompleteTextViewPreference(Context context, IAttributeSet attrs) : this(context, attrs, Resource.Attribute.editTextPreferenceStyle)
        {
        }

        public AutoCompleteTextViewPreference(Context context, IAttributeSet attrs, int defStyleAttr) : this(context, attrs, defStyleAttr, defStyleAttr)
        {
        }

        public AutoCompleteTextViewPreference(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        protected AutoCompleteTextViewPreference(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override Java.Lang.Object OnGetDefaultValue(TypedArray a, int index)
        {
            return a.GetString(index);
        }

        public void SaveGroupName(string name)
        {
            GroupName = name;
            PersistString(name);
        }

        protected override void OnSetInitialValue(bool restorePersistedValue, Java.Lang.Object defaultValue)
        {
            GroupName = restorePersistedValue ? GetPersistedString(GroupName) : defaultValue.ToString();
        }

        public override int DialogLayoutResource { get => resourceId; set => base.DialogLayoutResource = value; }
    }


    public class AutoCompleteTextViewPreferenceDialogFragmentCompat : PreferenceDialogFragmentCompat
    {
        AutoCompleteTextView autoCompleteTextViewPref;
        ProgressBar progressBar;
        ImageView imageViewRefresh;
        ArrayAdapter suggestAdapter;
        NetworkChecker networkChecker;
        Dictionary<string, int> groupsDictionary;
        static CancellationTokenSource cts;

        string[] array;

        public static AutoCompleteTextViewPreferenceDialogFragmentCompat NewInstance(string key, CancellationTokenSource ctSource)
        {
            AutoCompleteTextViewPreferenceDialogFragmentCompat fragment = new AutoCompleteTextViewPreferenceDialogFragmentCompat();
            Bundle b = new Bundle(1);
            b.PutString("key", key);
            fragment.Arguments = b;
            cts = ctSource;

            return fragment;
        }

        protected override void OnBindDialogView(View view)
        {
            base.OnBindDialogView(view);

            networkChecker = new NetworkChecker(Activity.BaseContext);
            autoCompleteTextViewPref = view.FindViewById<AutoCompleteTextView>(Resource.Id.autocompletetextview_group_pref);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressbar_group_pref);
            imageViewRefresh = view.FindViewById<ImageView>(Resource.Id.imageview_group_pref);
            imageViewRefresh.Click += ImageViewRefresh_Click;

            imageViewRefresh.Visibility = ViewStates.Invisible;
            progressBar.Visibility = ViewStates.Visible;
            Task.Run(async () =>
            {
                groupsDictionary = await MainApp.Instance.GroupsDictionary;
                array = groupsDictionary.Select(x => x.Key).ToArray();

                Activity.RunOnUiThread(() =>
                {
                    suggestAdapter = new ArrayAdapter(Activity.BaseContext, Android.Resource.Layout.SimpleDropDownItem1Line, array);

                    autoCompleteTextViewPref.Adapter = suggestAdapter;

                    progressBar.Visibility = ViewStates.Invisible;
                    imageViewRefresh.Visibility = ViewStates.Visible;
                });
            });
            
            string groupName = null;
            DialogPreference preference = Preference;
            if (preference is AutoCompleteTextViewPreference)
            {
                groupName = ((AutoCompleteTextViewPreference)preference).GroupName;
            }

            if (groupName != null)
            {
                autoCompleteTextViewPref.Text = groupName;
            }
        }

        private void ImageViewRefresh_Click(object sender, EventArgs e)
        {
            if (networkChecker.Check())
            {
                imageViewRefresh.Visibility = ViewStates.Invisible;
                progressBar.Visibility = ViewStates.Visible;
                Task.Run(async () =>
                {
                    MainApp.Instance.GroupsDictionary = new Nito.AsyncEx.AsyncLazy<Dictionary<string, int>>(async () => { return await MainApp.FillGroupsDictionary(true, cts.Token); });
                    var newGroupsDictionary = await MainApp.Instance.GroupsDictionary;
                    array = newGroupsDictionary.Select(x => x.Key).ToArray();
                    groupsDictionary = newGroupsDictionary;

                    Activity.RunOnUiThread(() =>
                    {
                        suggestAdapter = new ArrayAdapter(Activity.BaseContext, Android.Resource.Layout.SimpleDropDownItem1Line, array);

                        autoCompleteTextViewPref.Adapter = null;
                        autoCompleteTextViewPref.Adapter = suggestAdapter;

                        imageViewRefresh.Visibility = ViewStates.Visible;
                        progressBar.Visibility = ViewStates.Invisible;
                    });
                });
            }
            else
            {
                Toast.MakeText(Activity.BaseContext, GetString(Resource.String.no_connection_title), ToastLength.Short).Show();
            }
        }

        public override void OnDialogClosed(bool positiveResult)
        {
            if (positiveResult)
            {
                var groupName = autoCompleteTextViewPref.Text;

                DialogPreference preference = Preference;
                if (preference is AutoCompleteTextViewPreference autoCompleteTVPreference)
                {
                    if (autoCompleteTVPreference.CallChangeListener(groupName))
                    {
                        if (groupsDictionary.ContainsKey(groupName))
                        {
                            autoCompleteTVPreference.SaveGroupName(groupName);
                        }
                        {
                            Toast.MakeText(Activity.BaseContext, GetString(Resource.String.wrong_group), ToastLength.Short).Show();
                        }
                    }                    
                }
            }
        }
    }
}