using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;

namespace PolyNavi.Utils
{
    internal static class Utils
    {
        internal static void HideKeyboard(View view, Context context)
        {
            //TODO Check null
            var inputMethodManager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(view.WindowToken, 0);
        }

        internal static async Task<Dictionary<string, int>> GetSuggestedGroupsDictionary(string s) //TODO
        {
            throw new NotImplementedException(); //TODO
        }

        internal static async Task<Dictionary<string, int>> GetSuggestedTeachersDictionary(string s) //TODO
        {
            throw new NotImplementedException(); //TODO
        }
    }
}
