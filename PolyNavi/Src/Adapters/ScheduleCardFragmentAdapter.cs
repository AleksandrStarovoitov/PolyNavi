using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using PolyNaviLib.BL;

namespace PolyNavi
{
    public class TitleTag
	{
		public DateTime Date { get; set; }
	}

	public class ScheduleCardRowAdapter : RecyclerView.Adapter
	{
		List<Object> mLessons;
		Context context;
		LayoutInflater layoutInflater;
		protected View scheduleView;
		ScheduleCardRowLessonViewHolder viewHolderLesson;
		ScheduleCardRowTitleViewHolder viewHolderTitle;
		RecyclerView.ViewHolder viewHolder;
		CultureInfo cultureInfo = new CultureInfo("ru-RU");
		const int TitleConst = 0, LessonConst = 1;

		public ScheduleCardRowAdapter(List<Object> lessons, DateTime date)
		{
			lessons.Insert(0, new TitleTag() { Date = date });
			mLessons = lessons;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			context = parent.Context;
			layoutInflater = LayoutInflater.From(context);

			switch (viewType)
			{
				case LessonConst:
					scheduleView = layoutInflater.Inflate(Resource.Layout.layout_card_row_lesson_schedule, parent, false);
					viewHolder = new ScheduleCardRowLessonViewHolder(scheduleView);
					break;
				case TitleConst:
					scheduleView = layoutInflater.Inflate(Resource.Layout.layout_card_row_title_schedule, parent, false);
					viewHolder = new ScheduleCardRowTitleViewHolder(scheduleView);
					break;
				default:

					break;
			}
			return viewHolder;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
		{
			switch (viewHolder.ItemViewType)
			{
				case LessonConst:
					viewHolderLesson = (ScheduleCardRowLessonViewHolder)viewHolder;
					Lesson lesson = (Lesson)mLessons[position];

					TextView room = viewHolderLesson.room;
					TextView building = viewHolderLesson.building;
					TextView subject = viewHolderLesson.subject;
					TextView startTime = viewHolderLesson.startTime;
					TextView endTime = viewHolderLesson.endTime;
					TextView type = viewHolderLesson.type;
                    TextView teacher = viewHolderLesson.teacher;

					room.Text = "ауд. " + lesson.Auditories[0].Name;
					building.Text = lesson.Auditories[0].Building.Name + ", ";
					subject.Text = lesson.Subject;
					startTime.Text = lesson.Time_Start.ToString("HH:mm", cultureInfo);
					endTime.Text = lesson.Time_End.ToString("HH:mm", cultureInfo);
					type.Text = lesson.TypeObj.Name.Replace("Лабораторные", "Лаб.");
                    if (lesson.Teachers == null || lesson.Teachers.Count == 0)
                    {
                        var relativeLayout = ((ViewGroup)scheduleView).FindViewById(Resource.Id.relativelayout_row_schedule);
                        relativeLayout.Measure(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                        viewHolderLesson.teacher.Measure(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                        
                        var height = relativeLayout.MeasuredHeight;
                        var tvHegiht = viewHolderLesson.teacher.MeasuredHeight;

                        var layoutParams = relativeLayout.LayoutParameters;
                        layoutParams.Height = height - tvHegiht;

                        ((ViewGroup)scheduleView).RemoveView(viewHolderLesson.teacher);
                        relativeLayout.LayoutParameters = layoutParams;
                    }
                    else
                    {
                        teacher.Text = String.Join(", ", lesson.Teachers.Select(t => t.Full_Name).ToArray());
                    }                    
					break;
				case TitleConst:
					viewHolderTitle = (ScheduleCardRowTitleViewHolder)viewHolder;
					TitleTag title = (TitleTag)mLessons[position];

					TextView date = viewHolderTitle.date;
					TextView dayOfWeek = viewHolderTitle.dayOfWeek;

					date.Text = title.Date.ToString("M", cultureInfo); //"dd MMMM"
					dayOfWeek.Text = title.Date.ToString("dddd", cultureInfo);
					dayOfWeek.Text = dayOfWeek.Text.Substring(0, 1).ToUpper() + dayOfWeek.Text.Substring(1);
					break;
				default:
					break;
			}
		}

		public override int ItemCount => mLessons.Count;

		internal class ScheduleCardRowLessonViewHolder : RecyclerView.ViewHolder
		{
			public TextView building;
			public TextView room;
			public TextView subject;
			public TextView startTime;
			public TextView endTime;
			public TextView type;
            public TextView teacher;

			public ScheduleCardRowLessonViewHolder(View itemView) : base(itemView)
			{
				room = itemView.FindViewById<TextView>(Resource.Id.textview_card_room_row_lesson_schedule);
				building = itemView.FindViewById<TextView>(Resource.Id.textview_card_buildingnumber_row_lesson_schedule);
				subject = itemView.FindViewById<TextView>(Resource.Id.textview_card_subject_row_lesson_schedule);
				startTime = itemView.FindViewById<TextView>(Resource.Id.textview_card_starttime_row_lesson_schedule);
				endTime = itemView.FindViewById<TextView>(Resource.Id.textview_card_endtime_row_lesson_schedule);
				type = itemView.FindViewById<TextView>(Resource.Id.textview_card_type_row_lesson_schedule);
                teacher = itemView.FindViewById<TextView>(Resource.Id.textview_card_teacher_row_lesson_schedule);
			}
		}

		internal class ScheduleCardRowTitleViewHolder : RecyclerView.ViewHolder
		{
			public TextView date;
			public TextView dayOfWeek;
			
			public ScheduleCardRowTitleViewHolder(View itemView) : base(itemView)
			{
				date = itemView.FindViewById<TextView>(Resource.Id.textview_card_date_row_title_schedule);
				dayOfWeek = itemView.FindViewById<TextView>(Resource.Id.textview_card_dayofweek_row_title_schedule);
			}
		}

		public override int GetItemViewType(int position)
		{
			if (mLessons[position] is Lesson)
			{
				return LessonConst;
			}
			else if (mLessons[position] is TitleTag)
			{
				return TitleConst;
			}
			return -1;
		}
	}

	class ScheduleCardFragmentAdapter : RecyclerView.Adapter
	{
		List<Day> mDays;
		Context context;
		LayoutInflater layoutInflater;
		View scheduleView;
		ScheduleCardFragmentAdapterViewHolder viewHolder;
		public RecyclerView recyclerViewSchedule;

		public ScheduleCardFragmentAdapter(List<Day> days)
		{
			mDays = days.Where(Day => Day.Lessons.Count != 0).ToList();
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			context = parent.Context;
			layoutInflater = LayoutInflater.From(context);

			scheduleView = layoutInflater.Inflate(Resource.Layout.layout_card_schedule, parent, false);
						
			viewHolder = new ScheduleCardFragmentAdapterViewHolder(scheduleView);
			return viewHolder;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
		{
			var vh = (ScheduleCardFragmentAdapterViewHolder)viewHolder;

			Day day = mDays[position];

			recyclerViewSchedule = viewHolder.ItemView.FindViewById<RecyclerView>(Resource.Id.recyclerview_card_schedule);
			
			recyclerViewSchedule.HasFixedSize = true;

		    var adapter = new ScheduleCardRowAdapter(new List<object>(day.Lessons), day.Date);
			recyclerViewSchedule.SetAdapter(adapter);
			recyclerViewSchedule.SetLayoutManager(new LinearLayoutManager(context));
		}

		public override int ItemCount => mDays.Count;

		internal class ScheduleCardFragmentAdapterViewHolder : RecyclerView.ViewHolder
		{
			public CardView cardView;

			public ScheduleCardFragmentAdapterViewHolder(View itemView) : base(itemView)
			{
				cardView = itemView.FindViewById<CardView>(Resource.Id.cardview_schedule);
			}
		}
	}
}