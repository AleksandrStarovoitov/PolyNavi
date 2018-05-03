using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections;
using Android.Content;
using System.Collections.Generic;
using PolyNaviLib.BL;

namespace PolyNavi
{
	public class ScheduleCardRowAdapter : RecyclerView.Adapter
	{
		private List<Lesson> mLessons;
		private Context context;
		private LayoutInflater layoutInflater;
		private View scheduleView;
		private ScheduleCardRowAdapterViewHolder viewHolder;

		public ScheduleCardRowAdapter(List<Lesson> lessons)
		{
			mLessons = lessons;
		}

		// Create new views (invoked by the layout manager)
		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			context = parent.Context;
			layoutInflater = LayoutInflater.From(context);

			// Inflate the custom layout
			scheduleView = layoutInflater.Inflate(Resource.Layout.layout_card_row_lesson_schedule, parent, false);

			// Return a new holder instance
			viewHolder = new ScheduleCardRowAdapterViewHolder(scheduleView);
			return viewHolder;

		}

		// Replace the contents of a view (invoked by the layout manager)
		public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
		{
			var vh = (ScheduleCardRowAdapter.ScheduleCardRowAdapterViewHolder)viewHolder;

			// Get the data model based on position
			Lesson lesson = mLessons[position];

			// Set item views based on your views and data model
			//TextView date = vh.date;
			TextView time = vh.time;
			TextView room = vh.room;
			TextView building = vh.building;
			TextView subject = vh.subject;

			//date.Text = lesson.;
			time.Text = lesson.Timestr;
			room.Text = lesson.Room;
			building.Text = lesson.Building;
			subject.Text = lesson.Subject;
		}

		public override int ItemCount => mLessons.Count;

		internal class ScheduleCardRowAdapterViewHolder : RecyclerView.ViewHolder
		{
			public TextView date;
			public TextView time;
			public TextView building;
			public TextView room;
			public TextView subject;

			public ScheduleCardRowAdapterViewHolder(View itemView) : base(itemView)
			{
				//itemView.Click += (sender, e) => clickListener(new ScheduleCardFragmentAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
				//itemView.LongClick += (sender, e) => longClickListener(new ScheduleCardFragmentAdapterClickEventArgs { View = itemView, Position = AdapterPosition });

				//date = itemView.FindViewById<TextView>(Resource.Id.textview_card_dayanddate_row_lesson_schedule);
				time = itemView.FindViewById<TextView>(Resource.Id.textview_card_time_row_lesson_schedule);
				room = itemView.FindViewById<TextView>(Resource.Id.textview_card_room_row_lesson_schedule);
				building = itemView.FindViewById<TextView>(Resource.Id.textview_card_buildingnumber_row_lesson_schedule);
				subject = itemView.FindViewById<TextView>(Resource.Id.textview_card_subject_row_lesson_schedule);
			}
		}

		public class ScheduleCarRowAdapterClickEventArgs : EventArgs
		{
			public View View { get; set; }
			public int Position { get; set; }
		}
	}



	class ScheduleCardFragmentAdapter : RecyclerView.Adapter
	{
		public event EventHandler<ScheduleCardFragmentAdapterClickEventArgs> ItemClick;
		public event EventHandler<ScheduleCardFragmentAdapterClickEventArgs> ItemLongClick;

		private List<Day> mDays;
		private Context context;
		private LayoutInflater layoutInflater;
		private View scheduleView;
		private ScheduleCardFragmentAdapterViewHolder viewHolder;
		public RecyclerView recyclerViewSchedule;

		public ScheduleCardFragmentAdapter(List<Day> days)
		{
			mDays = days;
		}

		// Create new views (invoked by the layout manager)
		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			context = parent.Context;
			layoutInflater = LayoutInflater.From(context);

			// Inflate the custom layout
			scheduleView = layoutInflater.Inflate(Resource.Layout.layout_card_schedule, parent, false);
						
			// Return a new holder instance
			viewHolder = new ScheduleCardFragmentAdapterViewHolder(scheduleView);
			return viewHolder;

		}

		// Replace the contents of a view (invoked by the layout manager)
		public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
		{
			var vh = (ScheduleCardFragmentAdapter.ScheduleCardFragmentAdapterViewHolder)viewHolder;

			// Get the data model based on position
			Day day = mDays[position];

			recyclerViewSchedule = viewHolder.ItemView.FindViewById<RecyclerView>(Resource.Id.recyclerview_card_schedule);
			
			recyclerViewSchedule.HasFixedSize = true;
		    var adapter = new ScheduleCardRowAdapter(day.Lessons);
			recyclerViewSchedule.SetAdapter(adapter);
			recyclerViewSchedule.SetLayoutManager(new LinearLayoutManager(context));


			// Set item views based on your views and data model
			CardView cardView = vh.cardView;
			
			//cardView.
			//building.Text = lesson.Building;
			//subject.Text = lesson.Subject;
		}

		public override int ItemCount => mDays.Count;

		void OnClick(ScheduleCardFragmentAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
		void OnLongClick(ScheduleCardFragmentAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

		internal class ScheduleCardFragmentAdapterViewHolder : RecyclerView.ViewHolder
		{
			public CardView cardView;

			public ScheduleCardFragmentAdapterViewHolder(View itemView) : base(itemView)
			{
				cardView = itemView.FindViewById<CardView>(Resource.Id.cardview_schedule);
			}
		}

		public class ScheduleCardFragmentAdapterClickEventArgs : EventArgs
		{
			public View View { get; set; }
			public int Position { get; set; }
		}
	}
}