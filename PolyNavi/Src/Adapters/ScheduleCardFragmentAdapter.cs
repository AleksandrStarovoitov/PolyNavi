using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PolyNaviLib.BL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PolyNavi.Adapters
{
    public class TitleTag
    {
        public DateTime Date { get; set; }
    }

    public class ScheduleCardRowAdapter : RecyclerView.Adapter
    {
        private List<object> mLessons;
        private Context context;
        private LayoutInflater layoutInflater;
        private View scheduleView;
        private ScheduleCardRowLessonViewHolder viewHolderLesson;
        private ScheduleCardRowTitleViewHolder viewHolderTitle;
        private RecyclerView.ViewHolder viewHolder;
        private CultureInfo cultureInfo = new CultureInfo("ru-RU");
        private const int TitleConst = 0, LessonConst = 1;

        public ScheduleCardRowAdapter(List<object> lessons, DateTime date)
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
            }
            return viewHolder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            switch (viewHolder.ItemViewType)
            {
                case LessonConst:
                    viewHolderLesson = (ScheduleCardRowLessonViewHolder)viewHolder;
                    var lesson = (Lesson)mLessons[position];

                    var room = viewHolderLesson.room;
                    var building = viewHolderLesson.building;
                    var subject = viewHolderLesson.subject;
                    var startTime = viewHolderLesson.startTime;
                    var endTime = viewHolderLesson.endTime;
                    var type = viewHolderLesson.type;
                    var teacher = viewHolderLesson.teacher;
                    var group = viewHolderLesson.group;

                    room.Text = "ауд. " + lesson.Auditories[0].Name;
                    building.Text = lesson.Auditories[0].Building.Name + ", ";
                    subject.Text = lesson.Subject;
                    startTime.Text = lesson.Time_Start.ToString("HH:mm", cultureInfo);
                    endTime.Text = lesson.Time_End.ToString("HH:mm", cultureInfo);
                    type.Text = lesson.TypeObj.Name.Replace("Лабораторные", "Лаб.");

                    if (lesson.Teachers == null || lesson.Teachers.Count == 0)
                    {
                        var lparams = (RelativeLayout.LayoutParams)group.LayoutParameters;
                        lparams.AddRule(LayoutRules.Below, Resource.Id.textview_card_buildingnumber_row_lesson_schedule);
                        group.LayoutParameters = lparams;

                        ((ViewGroup)scheduleView).RemoveView(viewHolderLesson.teacher);
                    }
                    else
                    {
                        teacher.Text = string.Join(", ", lesson.Teachers.Select(t => t.Full_Name).ToArray());
                    }

                    if (!lesson.Additional_Info.Equals("")) //Поток или подгруппы
                    {
                        group.Text = lesson.Additional_Info;
                    }
                    else
                    {
                        group.Text = lesson.Groups.First().Name;
                    }


                    break;
                case TitleConst:
                    viewHolderTitle = (ScheduleCardRowTitleViewHolder)viewHolder;
                    var title = (TitleTag)mLessons[position];

                    var date = viewHolderTitle.date;
                    var dayOfWeek = viewHolderTitle.dayOfWeek;

                    date.Text = title.Date.ToString("M", cultureInfo); //"dd MMMM"
                    dayOfWeek.Text = title.Date.ToString("dddd", cultureInfo);
                    dayOfWeek.Text = dayOfWeek.Text.Substring(0, 1).ToUpper() + dayOfWeek.Text.Substring(1);
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
            public TextView group;

            public ScheduleCardRowLessonViewHolder(View itemView) : base(itemView)
            {
                room = itemView.FindViewById<TextView>(Resource.Id.textview_card_room_row_lesson_schedule);
                building = itemView.FindViewById<TextView>(Resource.Id.textview_card_buildingnumber_row_lesson_schedule);
                subject = itemView.FindViewById<TextView>(Resource.Id.textview_card_subject_row_lesson_schedule);
                startTime = itemView.FindViewById<TextView>(Resource.Id.textview_card_starttime_row_lesson_schedule);
                endTime = itemView.FindViewById<TextView>(Resource.Id.textview_card_endtime_row_lesson_schedule);
                type = itemView.FindViewById<TextView>(Resource.Id.textview_card_type_row_lesson_schedule);
                teacher = itemView.FindViewById<TextView>(Resource.Id.textview_card_teacher_row_lesson_schedule);
                group = itemView.FindViewById<TextView>(Resource.Id.textview_card_group_row_lesson_schedule);
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

    internal class ScheduleCardFragmentAdapter : RecyclerView.Adapter
    {
        private List<Day> mDays;
        private Context context;
        private LayoutInflater layoutInflater;
        private View scheduleView;
        private RecyclerView.ViewHolder viewHolder;
        private DateTime lastUpdatedDate;
        public RecyclerView recyclerViewSchedule;

        private const int EndConst = 0, CardConst = 1;

        public ScheduleCardFragmentAdapter(List<Day> days)
        {
            mDays = days.Where(day => day.Lessons.Count != 0).ToList();
            lastUpdatedDate = mDays.FirstOrDefault().WeekRoot.LastUpdated;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            context = parent.Context;
            layoutInflater = LayoutInflater.From(context);

            switch (viewType)
            {
                case CardConst:
                    scheduleView = layoutInflater.Inflate(Resource.Layout.layout_card_schedule, parent, false);
                    viewHolder = new ScheduleCardFragmentAdapterViewHolder(scheduleView);
                    break;

                case EndConst:
                    scheduleView = layoutInflater.Inflate(Resource.Layout.layout_card_schedule_end, parent, false);
                    viewHolder = new ScheduleCardEndFragmentAdapterViewHolder(scheduleView);
                    break;
            }

            return viewHolder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            switch (viewHolder.ItemViewType)
            {
                case CardConst:
                    var vh = (ScheduleCardFragmentAdapterViewHolder)viewHolder;

                    var day = mDays[position];

                    recyclerViewSchedule = vh.recyclerView;

                    recyclerViewSchedule.HasFixedSize = true;

                    var adapter = new ScheduleCardRowAdapter(new List<object>(day.Lessons), day.Date);
                    recyclerViewSchedule.SetAdapter(adapter);
                    recyclerViewSchedule.SetLayoutManager(new LinearLayoutManager(context));
                    break;

                case EndConst:
                    var vhe = (ScheduleCardEndFragmentAdapterViewHolder)viewHolder;

                    var tv = vhe.textView;

                    tv.Text = context.GetString(Resource.String.updated) + " " + lastUpdatedDate.ToString(CultureInfo.InvariantCulture);
                    break;
            }
        }

        public override int ItemCount => mDays.Count + 1;

        public override int GetItemViewType(int position)
        {
            if (position == mDays.Count)
            {
                return EndConst;
            }
            else
            {
                return CardConst;
            }
        }

        internal class ScheduleCardFragmentAdapterViewHolder : RecyclerView.ViewHolder
        {
            public RecyclerView recyclerView;

            public ScheduleCardFragmentAdapterViewHolder(View itemView) : base(itemView)
            {
                recyclerView = itemView.FindViewById<RecyclerView>(Resource.Id.recyclerview_card_schedule);
            }
        }

        internal class ScheduleCardEndFragmentAdapterViewHolder : RecyclerView.ViewHolder
        {
            public TextView textView;

            public ScheduleCardEndFragmentAdapterViewHolder(View itemView) : base(itemView)
            {
                textView = itemView.FindViewById<TextView>(Resource.Id.textview_card_schedule_end);
            }
        }
    }
}