using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PolyNaviLib.BL;

namespace PolyNavi
{
	public class ScheduleWeekAdapter : RecyclerView.Adapter
	{
		// Provide a direct reference to each of the views within a data item
		// Used to cache the views within the item layout for fast access
		internal class ViewHolder : RecyclerView.ViewHolder
		{
			// Your holder should contain a member variable
			// for any view that will be set as you render a row
			public TextView _date;
			public TextView _time;
			public TextView _building;
			public TextView _room;
			public TextView _subject;
			public View _divider;
			// We also create a constructor that accepts the entire item row
			// and does the view lookups to find each subview
			public ViewHolder(View itemView) : base(itemView)
			{
				// Stores the itemView in a public final member variable that can be used
				// to access the context from any ViewHolder instance.
				_date = itemView.FindViewById<TextView>(Resource.Id.textview_dayanddate_row_schedule);
				_time = itemView.FindViewById<TextView>(Resource.Id.textview_time_row_schedule);
				_room = itemView.FindViewById<TextView>(Resource.Id.textview_roomnumber_row_schedule);
				_building = itemView.FindViewById<TextView>(Resource.Id.textview_buildingnumber_row_schedule);
				_subject = itemView.FindViewById<TextView>(Resource.Id.textview_subject_row_schedule);
				_divider = itemView.FindViewById<View>(Resource.Id.divider_bottom_row_schedule);
			}
		} //end of vh

		private List<Lesson> mLessons;

		// Pass in the contact array into the constructor
		public ScheduleWeekAdapter(List<Lesson> lessons)
		{
			mLessons = lessons;
		}

		// Usually involves inflating a layout from XML and returning the holder
		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			Context context = parent.Context;
			LayoutInflater inflater = LayoutInflater.From(context);

			// Inflate the custom layout
			View timetableView = inflater.Inflate(Resource.Layout.layout_row_schedule, parent, false);

			// Return a new holder instance
			ViewHolder viewHolder = new ViewHolder(timetableView);
			return viewHolder;
			//!!!!!cast....
		}

		// Involves populating data into the item through holder
		public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
		{
			var vh = (ScheduleWeekAdapter.ViewHolder)viewHolder;

			// Get the data model based on position
			Lesson lesson = mLessons[position];

			// Set item views based on your views and data model
			TextView date = vh._date;
			TextView time = vh._time;
			TextView room = vh._room;
			TextView building = vh._building;
			TextView subject = vh._subject;
			View divider = vh._divider;
			
			if (lesson.Last)
			{
				divider.Visibility = ViewStates.Visible;
			}
			else
			{
				divider.Visibility = ViewStates.Invisible;
			}

			//date.Text = lesson.;
			time.Text = lesson.Timestr;
			room.Text = lesson.Room;
			building.Text = lesson.Building;
			subject.Text = lesson.Subject;
		}

		// Returns the total count of items in the list
		public override int ItemCount => mLessons.Count;
	}
}
