using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Preferences;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Newtonsoft.Json;
using PolyNaviLib.BL;
using PolyNaviLib.SL;

namespace PolyNavi
{
    [Activity(Label = "AutoCompleteTextViewPreference")]
    public class AutoCompleteTextViewPreference : EditTextPreference
    {
        private readonly int resourceId = Resource.Layout.preference_dialog_autocomplete;
        public string GroupName { get; private set; }

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


    public class AutoCompleteTextViewPreferenceDialogFragmentCompat : PreferenceDialogFragmentCompat, ITextWatcher
    {
        private AutoCompleteTextView autoCompleteTextViewPref;
        private ArrayAdapter suggestAdapter;
        private NetworkChecker networkChecker;
        private Dictionary<string, int> groupsDictionary;
        private CancellationTokenSource cts;
        private System.Timers.Timer searchTimer;
        private const int millsToSearch = 700;
        private string[] array;
        private const string groupSearchLink =
            "http://m.spbstu.ru/p/proxy.php?csurl=http://ruz.spbstu.ru/api/v1/ruz/search/groups&q=";

        public static AutoCompleteTextViewPreferenceDialogFragmentCompat NewInstance(string key)
        {
            AutoCompleteTextViewPreferenceDialogFragmentCompat fragment = new AutoCompleteTextViewPreferenceDialogFragmentCompat();
            Bundle b = new Bundle(1);
            b.PutString("key", key);
            fragment.Arguments = b;

            return fragment;
        }

        protected override void OnBindDialogView(View view)
        {
            base.OnBindDialogView(view);

            networkChecker = new NetworkChecker(Activity.BaseContext);
            autoCompleteTextViewPref = view.FindViewById<AutoCompleteTextView>(Resource.Id.autocompletetextview_group_pref);

            autoCompleteTextViewPref.AddTextChangedListener(this);
           
            string groupName = null;
            DialogPreference preference = Preference;
            if (preference is AutoCompleteTextViewPreference viewPreference)
            {
                groupName = viewPreference.GroupName;
            }

            if (groupName != null)
            {
                autoCompleteTextViewPref.Text = groupName;
            }

            cts = new CancellationTokenSource();
        }

        public override void OnDialogClosed(bool positiveResult)
        {
            if (!positiveResult) return;

            var groupName = autoCompleteTextViewPref.Text;

            DialogPreference preference = Preference;
            if (preference is AutoCompleteTextViewPreference autoCompleteTVPreference)
            {
                if (autoCompleteTVPreference.CallChangeListener(groupName))
                {
                    if (groupsDictionary.TryGetValue(groupName, out int groupId))
                    {
                        autoCompleteTVPreference.SaveGroupName(groupName);
                        MainApp.Instance.SharedPreferences.Edit().PutInt("groupid", groupId).Apply();
                    }
                    else
                    {
                        Toast.MakeText(Activity.BaseContext, GetString(Resource.String.wrong_group), ToastLength.Short).Show();
                    }
                }                    
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
            autoCompleteTextViewPref.Adapter = null;
            autoCompleteTextViewPref.DismissDropDown();
            if (autoCompleteTextViewPref.Error != null && s.ToString().Length != 0)
            {
                autoCompleteTextViewPref.Error = null;
            }

            if (searchTimer != null)
            {
                searchTimer.Stop();
            }
            else
            {
                searchTimer = new System.Timers.Timer(millsToSearch);
                searchTimer.Elapsed += delegate
                {
                    if (networkChecker.Check())
                    {
                        var client = new HttpClient();
                        Task.Run(async () =>
                        {
                            var resultJson = await HttpClientSL.GetResponseAsync(client,
                                groupSearchLink +
                                s.ToString(), new System.Threading.CancellationToken());
                            var groups = JsonConvert.DeserializeObject<GroupRoot>(resultJson);
                            groupsDictionary = groups.Groups.ToDictionary(x => x.Name, x => x.Id);
                            array = groupsDictionary.Select(x => x.Key).ToArray();
                            Activity.RunOnUiThread(() =>
                            {
                                suggestAdapter = new ArrayAdapter(Activity.BaseContext,
                                    Android.Resource.Layout.SimpleDropDownItem1Line,
                                    array);
                                autoCompleteTextViewPref.Adapter = null;
                                autoCompleteTextViewPref.Adapter = suggestAdapter;
                                if (s.Length() > 0 && before != count)
                                {
                                    autoCompleteTextViewPref.ShowDropDown();
                                }
                            });
                        });
                    }
                    else
                    {
                        Toast.MakeText(Activity.BaseContext, GetString(Resource.String.no_connection_title), ToastLength.Short).Show();
                    }

                    searchTimer.Close();
                    searchTimer = null;
                };
            }

            searchTimer.Start();
        }
    }
}