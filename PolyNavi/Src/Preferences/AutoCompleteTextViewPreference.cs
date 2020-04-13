using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Preference;
using Java.Lang;
using PolyNavi.Extensions;
using PolyNavi.Services;
using PolyNaviLib.Constants;
using Object = Java.Lang.Object;
using Timer = System.Timers.Timer;

namespace PolyNavi.Preferences
{
    [Activity(Label = "AutoCompleteTextViewPreference")]
    public class AutoCompleteTextViewPreference : EditTextPreference
    {
        private const int ResourceId = Resource.Layout.preference_dialog_autocomplete;
        public string Name { get; private set; }

        protected AutoCompleteTextViewPreference(IntPtr javaReference, JniHandleOwnership transfer) : base(
            javaReference, transfer)
        {
        }

        public AutoCompleteTextViewPreference(Context context) : base(context)
        {
        }

        public AutoCompleteTextViewPreference(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public AutoCompleteTextViewPreference(Context context, IAttributeSet attrs, int defStyleAttr) : base(context,
            attrs, defStyleAttr)
        {
        }

        public AutoCompleteTextViewPreference(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) :
            base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        protected override Object OnGetDefaultValue(TypedArray a, int index)
        {
            return a.GetString(index);
        }

        public void SaveName(string name)
        {
            Name = name;
            PersistString(name);
        }

        protected override void OnSetInitialValue(bool restorePersistedValue, Object defaultValue) //TODO
        {
            Name = restorePersistedValue ? GetPersistedString(Name) : defaultValue.ToString();
        }

        public override int DialogLayoutResource
        {
            get => ResourceId;
            set => base.DialogLayoutResource = value;
        }
    }

    public class AutoCompleteTextViewPreferenceDialogFragmentCompat : PreferenceDialogFragmentCompat, ITextWatcher
    {
        private AutoCompleteTextView autoCompleteTextViewPref;
        private NetworkChecker networkChecker;
        private Dictionary<string, int> suggestionsAndIds;
        private Timer searchTimer;
        private bool isTeacher;
        private const int MillsToSearch = 700;

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

            isTeacher = Preference.SharedPreferences.GetBoolean(PreferenceConstants.IsUserTeacherPreferenceKey, false);
            networkChecker = new NetworkChecker(Activity.BaseContext);
            autoCompleteTextViewPref =
                view.FindViewById<AutoCompleteTextView>(Resource.Id.autocompletetextview_group_pref);
            
            autoCompleteTextViewPref.AddTextChangedListener(this);
            autoCompleteTextViewPref.Hint = Resources.GetString(isTeacher 
                ? Resource.String.auth_teacher_hint
                : Resource.String.edittext_auth);            

            string name = null;
            var preference = Preference;
            if (preference is AutoCompleteTextViewPreference viewPreference)
            {
                name = viewPreference.Name;
            }

            if (name != null)
            {
                autoCompleteTextViewPref.Text = name;
            }
        }

        public override void OnDialogClosed(bool positiveResult)
        {
            if (!positiveResult) return;

            var name = autoCompleteTextViewPref.Text;

            var preference = Preference;
            if (preference is AutoCompleteTextViewPreference autoCompleteTvPreference &&
                autoCompleteTvPreference.CallChangeListener(name)) //TODO
            {
                if (suggestionsAndIds.TryGetValue(name, out var id))
                {
                    if (isTeacher)
                    {
                        autoCompleteTvPreference.SaveName(name);
                        MainApp.Instance.SharedPreferences.Edit().PutInt(PreferenceConstants.TeacherIdPreferenceKey, id)
                            .Apply();
                    } 
                    else
                    {
                        autoCompleteTvPreference.SaveName(name);
                        MainApp.Instance.SharedPreferences.Edit().PutInt(PreferenceConstants.GroupIdPreferenceKey, id)
                            .Apply();
                    }
                }
                else
                {
                    Toast.MakeText(Activity.BaseContext, GetString(Resource.String.wrong_group), ToastLength.Short)
                        .Show(); //TODO Wrong teacher
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
            autoCompleteTextViewPref.Clear();

            SetupTimer(s, before, count);
        }
        
        private void SetupTimer(ICharSequence s, int before, int count) //TODO Duplicate code (auth) //TODO Move to lib, catch ex
        {
            if (searchTimer != null)
            {
                searchTimer.Stop();
            }
            else
            {
                searchTimer = new Timer(MillsToSearch);
                searchTimer.Elapsed += delegate
                {
                    if (networkChecker.IsConnected())
                    {
                        Task.Run(async () =>
                        {
                            suggestionsAndIds = isTeacher
                            ? await Utils.Utils.GetSuggestedTeachersDictionary(s.ToString())
                            : await Utils.Utils.GetSuggestedGroupsDictionary(s.ToString());

                            if (s.Length() > 0 && before != count) //TODO Local method?
                            {
                                Activity.RunOnUiThread(() =>
                                {
                                    autoCompleteTextViewPref.UpdateSuggestions(suggestionsAndIds, Activity);
                                });
                            }
                        });
                    }
                    else
                    {
                        Toast.MakeText(Activity.BaseContext, GetString(Resource.String.no_connection_title),
                            ToastLength.Short).Show();
                    }

                    searchTimer.Close();
                    searchTimer = null;
                };
            }

            searchTimer.Start();
        }
    }
}