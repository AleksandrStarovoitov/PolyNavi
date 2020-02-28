using System;
using Android.App;
using Android.OS;
using Android.Widget;

namespace PolyNavi.Fragments
{
    public class DateTimePickerFragment : DialogFragment,
                                  DatePickerDialog.IOnDateSetListener
    {
        public static readonly string DateTimePickerTag = "X:" + typeof(DateTimePickerFragment).Name.ToUpper();
        private static DateTime? lastDate;

        private Action<DateTime> dateSelectedHandler = delegate { };

        public static DateTimePickerFragment NewInstance(Action<DateTime> onDateSelected, DateTime? lastDate = null)
        {
            var frag = new DateTimePickerFragment
            {
                dateSelectedHandler = onDateSelected
            };
            DateTimePickerFragment.lastDate = lastDate;

            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var currentDate = (DateTime) (lastDate == null ? DateTime.Now : lastDate);
            var dialog = new DatePickerDialog(Activity,
                                                           this,
                                                           currentDate.Year,
                                                           currentDate.Month - 1,
                                                           currentDate.Day);
            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            dateSelectedHandler(selectedDate);
        }
    }
}