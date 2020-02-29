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
using PolyNavi.Services;
using PolyNaviLib.BL;
using PolyNaviLib.SL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PolyNavi.Preferences
{
    [Activity(Label = "AutoCompleteTextViewPreference")]
    public class AutoCompleteTextViewPreference : EditTextPreference
    {
        private const int ResourceId = Resource.Layout.preference_dialog_autocomplete;
        public string GroupName { get; private set; }
        
        protected AutoCompleteTextViewPreference(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public AutoCompleteTextViewPreference(Context context) : base(context)
        {
        }

        public AutoCompleteTextViewPreference(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public AutoCompleteTextViewPreference(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public AutoCompleteTextViewPreference(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
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

        public override int DialogLayoutResource { get => ResourceId; set => base.DialogLayoutResource = value; }
    }


    public class AutoCompleteTextViewPreferenceDialogFragmentCompat : PreferenceDialogFragmentCompat, ITextWatcher
    {
        private AutoCompleteTextView autoCompleteTextViewPref;
        private ArrayAdapter suggestAdapter;
        private NetworkChecker networkChecker;
        private Dictionary<string, int> groupsDictionary;
        private System.Timers.Timer searchTimer;
        private const int MillsToSearch = 700;
        private string[] array;
        private const string GroupSearchLink =
            "http://m.spbstu.ru/p/proxy.php?csurl=http://ruz.spbstu.ru/api/v1/ruz/search/groups&q=";

        public static AutoCompleteTextViewPreferenceDialogFragmentCompat NewInstance(string key)
        {
            var fragment = new AutoCompleteTextViewPreferenceDialogFragmentCompat();
            var b = new Bundle(1);
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
            var preference = Preference;
            if (preference is AutoCompleteTextViewPreference viewPreference)
            {
                groupName = viewPreference.GroupName;
            }

            if (groupName != null)
            {
                autoCompleteTextViewPref.Text = groupName;
            }
        }

        public override void OnDialogClosed(bool positiveResult)
        {
            if (!positiveResult) return;

            var groupName = autoCompleteTextViewPref.Text;

            var preference = Preference;
            if (preference is AutoCompleteTextViewPreference autoCompleteTvPreference && autoCompleteTvPreference.CallChangeListener(groupName)) //TODO
            {
                if (groupsDictionary.TryGetValue(groupName, out var groupId))
                {
                    autoCompleteTvPreference.SaveGroupName(groupName);
                    MainApp.Instance.SharedPreferences.Edit().PutInt("groupid", groupId).Apply();
                }
                else
                {
                    Toast.MakeText(Activity.BaseContext, GetString(Resource.String.wrong_group), ToastLength.Short).Show();
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
                searchTimer = new System.Timers.Timer(MillsToSearch);
                searchTimer.Elapsed += delegate
                {
                    if (networkChecker.Check())
                    {
                        var client = new HttpClient();
                        Task.Run(async () =>
                        {
                            var resultJson = await HttpClientService.GetResponseAsync(client,
                                GroupSearchLink +
                                s.ToString(), new CancellationToken());
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