//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Support.V7.Widget;
//using Android.Views;
//using Android.Widget;
//using PolyNaviLib.BL;

//namespace PolyNavi
//{
//	public class ScheduleWeekAdapter : RecyclerView.Adapter
//	{
//		// Provide a direct reference to each of the views within a data item
//		// Used to cache the views within the item layout for fast access
//		internal class ViewHolder : RecyclerView.ViewHolder
//		{
//			// Your holder should contain a member variable
//			// for any view that will be set as you render a row
//			public TextView date;
//			public TextView time;
//			public TextView building;
//			public TextView room;
//			public TextView subject;
//			public View divider;

//			// We also create a constructor that accepts the entire item row
//			// and does the view lookups to find each subview
//			public ViewHolder(View itemView) : base(itemView)
//			{
//				// Stores the itemView in a public final member variable that can be used
//				// to access the context from any ViewHolder instance.
//				date = itemView.FindViewById<TextView>(Resource.Id.textview_dayanddate_row_schedule);
//				time = itemView.FindViewById<TextView>(Resource.Id.textview_time_row_schedule);
//				room = itemView.FindViewById<TextView>(Resource.Id.textview_roomnumber_row_schedule);
//				building = itemView.FindViewById<TextView>(Resource.Id.textview_buildingnumber_row_schedule);
//				subject = itemView.FindViewById<TextView>(Resource.Id.textview_subject_row_schedule);
//				divider = itemView.FindViewById<View>(Resource.Id.divider_bottom_row_schedule);
//			}
//		}

//		private List<Lesson> mLessons;
//		private Context context;
//		private LayoutInflater layoutInflater;
//		private View scheduleView;
//		private ViewHolder viewHolder;

//		// Pass in the contact array into the constructor
//		public ScheduleWeekAdapter(List<Lesson> lessons)
//		{
//			mLessons = lessons;
//		}

//		// Usually involves inflating a layout from XML and returning the holder
//		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
//		{
//			context = parent.Context;
//			layoutInflater = LayoutInflater.From(context);

//			// Inflate the custom layout
//			scheduleView = layoutInflater.Inflate(Resource.Layout.layout_row_schedule, parent, false);

//			// Return a new holder instance
//			viewHolder = new ViewHolder(scheduleView);
//			return viewHolder;
//			//!!!!!cast....
//		}

//		// Involves populating data into the item through holder
//		public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
//		{
//			var vh = (ScheduleWeekAdapter.ViewHolder)viewHolder;

//			// Get the data model based on position
//			Lesson lesson = mLessons[position];

//			// Set item views based on your views and data model
//			TextView date = vh.date;
//			TextView time = vh.time;
//			TextView room = vh.room;
//			TextView building = vh.building;
//			TextView subject = vh.subject;
//			View divider = vh.divider;
			
//			if (lesson.Last)
//			{
//				divider.Visibility = ViewStates.Visible;
//			}
//			else
//			{
//				divider.Visibility = ViewStates.Invisible;
//			}

//			//date.Text = lesson.;
//			time.Text = lesson.Timestr;
//			room.Text = lesson.Room;
//			building.Text = lesson.Building;
//			subject.Text = lesson.Subject;
//		}

//		// Returns the total count of items in the list
//		public override int ItemCount => mLessons.Count;
//	}
//}
