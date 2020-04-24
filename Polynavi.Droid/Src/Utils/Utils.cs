using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using Polynavi.Droid.Src;

namespace Polynavi.Droid.Utils
{
    internal static class Utils
    {
        internal static void HideKeyboard(View view, Context context)
        {
            //TODO Check null
            var inputMethodManager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(view.WindowToken, 0);
        }
    }
}
