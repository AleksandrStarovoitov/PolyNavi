using System;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Preference;
using Polynavi.Common.Constants;
using Polynavi.Droid.Utils;
using Object = Java.Lang.Object;

namespace Polynavi.Droid.Preferences
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

    public class AutoCompleteTextViewPreferenceDialogFragmentCompat : PreferenceDialogFragmentCompat
    {
        private const int BundleCapacity = 1;
        private AutoCompleteTextView autoCompleteTextView;
        private TextChangeListener textChangeListener;

        private bool isTeacher;

        public static AutoCompleteTextViewPreferenceDialogFragmentCompat NewInstance(string key)
        {
            var bundle = new Bundle(BundleCapacity);
            bundle.PutString("key", key);

            var fragment = new AutoCompleteTextViewPreferenceDialogFragmentCompat()
            {
                Arguments = bundle
            };

            return fragment;
        }

        protected override void OnBindDialogView(View view)
        {
            base.OnBindDialogView(view);

            isTeacher = Preference.SharedPreferences.GetBoolean(PreferenceConstants.IsUserTeacherPreferenceKey, false);

            autoCompleteTextView =
                view.FindViewById<AutoCompleteTextView>(Resource.Id.autocompletetextview_group_pref);            
            autoCompleteTextView.Hint = Resources.GetString(isTeacher
                ? Resource.String.auth_teacher_hint
                : Resource.String.edittext_auth);

            textChangeListener = new TextChangeListener(Activity, autoCompleteTextView);
            autoCompleteTextView.AddTextChangedListener(textChangeListener);

            if (Preference is AutoCompleteTextViewPreference autoCompletePreference)
            {
                autoCompleteTextView.Text = autoCompletePreference.Name;
            }
        }

        public override void OnDialogClosed(bool positiveResult)
        {
            var preference = Preference as AutoCompleteTextViewPreference;
            var name = autoCompleteTextView.Text;

            if (!positiveResult || preference == null || name.Equals(preference.Name))
            {
                return;
            }

            if (!preference.CallChangeListener(name))
            {
                return;
            }

            if (textChangeListener.SuggestionsAndIds.TryGetValue(name, out var id))
            {
                preference.SaveName(name);

                MainApp.Instance.SharedPreferences
                    .Edit()
                    .PutInt(isTeacher
                        ? PreferenceConstants.TeacherIdPreferenceKey
                        : PreferenceConstants.GroupIdPreferenceKey, id)
                    .Apply();
            }
            else
            {
                Toast.MakeText(Activity.BaseContext, GetString(isTeacher
                        ? Resource.String.wrong_teacher
                        : Resource.String.wrong_group), ToastLength.Short)
                    .Show();
            }
        }        
    }
}
