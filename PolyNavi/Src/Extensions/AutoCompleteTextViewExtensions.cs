using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Widget;

namespace PolyNavi.Extensions
{
    internal static class AutoCompleteTextViewExtensions
    {
        internal static void UpdateSuggestions(this AutoCompleteTextView autoCompleteTextView,
            Dictionary<string, int> groupsDictionary, Activity activity)
        {
            var groupsDictionaryKeys = groupsDictionary.Select(x => x.Key).ToArray();

            var suggestAdapter = new ArrayAdapter(activity.BaseContext,
                Android.Resource.Layout.SimpleDropDownItem1Line, groupsDictionaryKeys); //TODO field?

            autoCompleteTextView.Adapter = null;
            autoCompleteTextView.Adapter = suggestAdapter;

            autoCompleteTextView.ShowDropDown();
        }

        internal static void Clear(this AutoCompleteTextView autoCompleteTextView)
        {
            autoCompleteTextView.Adapter = null;
            autoCompleteTextView.DismissDropDown();
            autoCompleteTextView.Error = null;
        }
    }
}
