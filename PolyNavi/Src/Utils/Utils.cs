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
    }
}