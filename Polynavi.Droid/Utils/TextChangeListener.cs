using System.Collections.Generic;
using System.Timers;
using Android.App;
using Android.Text;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.Lang;
using Polynavi.Common.Constants;
using Polynavi.Common.Exceptions;
using Polynavi.Droid.Extensions;

namespace Polynavi.Droid.Utils
{
    //TODO Attributes?
    internal class TextChangeListener : AppCompatActivity, ITextWatcher
    {
        private const int MillsToSearch = 700;
        private readonly Activity activity;
        private readonly AutoCompleteTextView autoCompleteTextView;
        private readonly bool isTeacher;
        private Timer searchTimer;

        public Dictionary<string, int> SuggestionsAndIds { get; private set; }
        
        public TextChangeListener(Activity activity, AutoCompleteTextView autoCompleteTextView)
        {
            this.activity = activity;
            this.autoCompleteTextView = autoCompleteTextView;
            isTeacher = AndroidDependencyContainer.Instance.SettingsStorage
                .GetBoolean(PreferenceConstants.IsUserTeacherPreferenceKey, false);
        }

        public void AfterTextChanged(IEditable s)
        {
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            autoCompleteTextView.Clear();

            SetupTimer(s, before, count);
        }

        private void SetupTimer(ICharSequence s, int before, int count) //TODO Move to lib ?
        {
            if (s.Length() == 0 || before == count)
            {
                return;
            }

            if (searchTimer != null)
            {
                searchTimer.Stop();
                searchTimer.Start();
                return;
            }

            searchTimer = new Timer(MillsToSearch);
            searchTimer.Elapsed += async delegate
            {
                try
                {
                    searchTimer.Close();
                    searchTimer = null;

                    var suggestionsService = AndroidDependencyContainer.Instance.SuggestionsService;

                    SuggestionsAndIds = isTeacher //TODO NULL
                            ? await suggestionsService.GetSuggestedGroupsAsync(s.ToString())
                            : await suggestionsService.GetSuggestedGroupsAsync(s.ToString());

                    activity.RunOnUiThread(() =>
                    {
                        autoCompleteTextView.UpdateSuggestions(SuggestionsAndIds, activity);
                    });
                }
                catch (NetworkException)
                {
                    activity.RunOnUiThread(() =>
                    {
                        Toast.MakeText(activity, activity.GetString(Resource.String.no_connection_title), ToastLength.Short).Show();
                    });
                }
            };

            searchTimer.Start();
        }
    }
}