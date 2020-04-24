using System;
using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;

namespace Polynavi.Droid.Fragments
{
    public class DateTimePickerFragment : AppCompatDialogFragment,
        DatePickerDialog.IOnDateSetListener
    {
        public static readonly string DateTimePickerTag = nameof(DateTimePickerFragment).ToUpper(); //TODO typeof?
        private static DateTime? lastDate;
        private Action<DateTime> dateSelectedHandler;

        public static DateTimePickerFragment NewInstance(Action<DateTime> onDateSelected, DateTime? lastDate = null)
        {
            var fragment = new DateTimePickerFragment { dateSelectedHandler = onDateSelected };
            DateTimePickerFragment.lastDate = lastDate;

            return fragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var currentDate = lastDate ?? DateTime.Now;
            var dialog = new DatePickerDialog(Activity, this, currentDate.Year,
                currentDate.Month - 1, currentDate.Day); //TODO -1 ?
            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            var selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth); //TODO +1 ?
            dateSelectedHandler(selectedDate);
        }
    }
}
