using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using PolyNaviLib.BL;
using System.Threading.Tasks;

namespace PolyNavi.Utils
{
    internal static class Utils
    {
        internal static void HideKeyboard(View view, Context context)
        {
            //TODO Check null
            var inputMethodManager = (InputMethodManager) context.GetSystemService(Context.InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(view.WindowToken, 0);
        }
        
        internal static async Task<Dictionary<string, int>> GetSuggestedGroupsDictionary(string s)
        {
            var groups = await PolyManager.GetSuggestedGroups(s);

            return groups.Groups.ToDictionary(x => x.Name, x => x.Id);
        }
    }
}