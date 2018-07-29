using System;

using Android.App;
using Android.OS;
using Android.Widget;

namespace PolyNavi
{
    public class DateTimePickerFragment : DialogFragment,
                                  DatePickerDialog.IOnDateSetListener
    {
        public static readonly string TAG = "X:" + typeof(DateTimePickerFragment).Name.ToUpper();
        static DateTime? _lastDate;

        Action<DateTime> dateSelectedHandler = delegate { };

        public static DateTimePickerFragment NewInstance(Action<DateTime> onDateSelected, DateTime? lastDate = null)
        {
            var frag = new DateTimePickerFragment
            {
                dateSelectedHandler = onDateSelected
            };
            _lastDate = lastDate;

            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var currentDate = (DateTime) (_lastDate == null ? DateTime.Now : _lastDate);
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