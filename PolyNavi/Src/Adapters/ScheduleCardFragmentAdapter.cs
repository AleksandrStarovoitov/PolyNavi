using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using PolyNaviLib.BL;

namespace PolyNavi.Adapters
{
    public class TitleTag
    {
        public DateTime Date { get; set; }
    }

    public class ScheduleCardRowAdapter : RecyclerView.Adapter
    {
        private readonly List<object> lessons;
        private Context context;
        private LayoutInflater layoutInflater;
        private View scheduleView;
        private ScheduleCardRowLessonViewHolder lessonViewHolder;
        private ScheduleCardRowTitleViewHolder titleViewHolder;
        private RecyclerView.ViewHolder viewHolder;
        private readonly CultureInfo cultureInfo = new CultureInfo("ru-RU"); //TODO ?
        private const int TitleTag = 0, LessonTag = 1;

        public ScheduleCardRowAdapter(List<object> lessons, DateTime date)
        {
            lessons.Insert(0, new TitleTag() { Date = date });
            this.lessons = lessons;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            context = parent.Context;
            layoutInflater = LayoutInflater.From(context);

            switch (viewType)
            {
                case LessonTag:
                    scheduleView = layoutInflater.Inflate(Resource.Layout.layout_card_row_lesson_schedule, parent, false);
                    viewHolder = new ScheduleCardRowLessonViewHolder(scheduleView);
                    break;
                case TitleTag:
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
                case LessonTag:
                    lessonViewHolder = (ScheduleCardRowLessonViewHolder)viewHolder;
                    var lesson = (Lesson)lessons[position];

                    var room = lessonViewHolder.roomTextView;
                    var building = lessonViewHolder.buildingTextView;
                    var subject = lessonViewHolder.subjectTextView;
                    var startTime = lessonViewHolder.startTimeTextView;
                    var endTime = lessonViewHolder.endTimeTextView;
                    var type = lessonViewHolder.typeTextView;
                    var teacher = lessonViewHolder.teacherTextView;
                    var group = lessonViewHolder.groupTextView;

                    room.Text = "ауд. " + lesson.Auditories[0].Name;
                    building.Text = lesson.Auditories[0].Building.Name + ", ";
                    subject.Text = lesson.Subject;
                    startTime.Text = lesson.Time_Start.ToString("HH:mm", cultureInfo);
                    endTime.Text = lesson.Time_End.ToString("HH:mm", cultureInfo);
                    type.Text = lesson.TypeObj.Name.Replace("Лабораторные", "Лаб.")
                                                    .Replace("Курсовое проектирование", "Курс."); //TODO in xml

                    if (lesson.Teachers == null || lesson.Teachers.Count == 0)
                    {
                        var layoutParams = (RelativeLayout.LayoutParams)group.LayoutParameters;
                        layoutParams.AddRule(LayoutRules.Below, Resource.Id.textview_card_buildingnumber_row_lesson_schedule);
                        group.LayoutParameters = layoutParams;

                        ((ViewGroup)scheduleView).RemoveView(lessonViewHolder.teacherTextView);
                    }
                    else
                    {
                        teacher.Text = string.Join(", ", lesson.Teachers.Select(t => t.Full_Name).ToArray());
                    }

                    if (!lesson.Additional_Info.Equals("")) //Поток или подгруппы //TODO
                    {
                        group.Text = lesson.Additional_Info;
                    }
                    else
                    {
                        group.Text = lesson.Groups.First().Name;
                    }
                    break;

                case TitleTag:
                    titleViewHolder = (ScheduleCardRowTitleViewHolder)viewHolder;
                    var title = (TitleTag)lessons[position];

                    var date = titleViewHolder.dateTextView;
                    var dayOfWeek = titleViewHolder.dayOfWeekTextView;

                    date.Text = title.Date.ToString("M", cultureInfo); //"dd MMMM"
                    dayOfWeek.Text = title.Date.ToString("dddd", cultureInfo);
                    dayOfWeek.Text = dayOfWeek.Text.Substring(0, 1).ToUpper() + dayOfWeek.Text.Substring(1);
                    break;
            }
        }

        public override int ItemCount => lessons.Count;

        private class ScheduleCardRowLessonViewHolder : RecyclerView.ViewHolder
        {
            public readonly TextView buildingTextView;
            public readonly TextView roomTextView;
            public readonly TextView subjectTextView;
            public readonly TextView startTimeTextView;
            public readonly TextView endTimeTextView;
            public readonly TextView typeTextView;
            public readonly TextView teacherTextView;
            public readonly TextView groupTextView;

            public ScheduleCardRowLessonViewHolder(View itemView) : base(itemView)
            {
                roomTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_room_row_lesson_schedule);
                buildingTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_buildingnumber_row_lesson_schedule);
                subjectTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_subject_row_lesson_schedule);
                startTimeTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_starttime_row_lesson_schedule);
                endTimeTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_endtime_row_lesson_schedule);
                typeTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_type_row_lesson_schedule);
                teacherTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_teacher_row_lesson_schedule);
                groupTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_group_row_lesson_schedule);
            }
        }

        private class ScheduleCardRowTitleViewHolder : RecyclerView.ViewHolder
        {
            public readonly TextView dateTextView;
            public readonly TextView dayOfWeekTextView;

            public ScheduleCardRowTitleViewHolder(View itemView) : base(itemView)
            {
                dateTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_date_row_title_schedule);
                dayOfWeekTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_dayofweek_row_title_schedule);
            }
        }

        public override int GetItemViewType(int position)
        {
            return lessons[position] switch
            {
                Lesson _ => LessonTag,
                TitleTag _ => TitleTag,
                _ => -1
            };
        }
    }

    internal class ScheduleCardFragmentAdapter : RecyclerView.Adapter
    {
        private readonly List<Day> days;
        private Context context;
        private LayoutInflater layoutInflater;
        private View scheduleView;
        private RecyclerView.ViewHolder viewHolder;
        private readonly DateTime lastUpdatedDate;
        private RecyclerView recyclerViewSchedule;

        private const int EndTag = 0, CardTag = 1;

        public ScheduleCardFragmentAdapter(IEnumerable<Day> days)
        {
            this.days = days.Where(day => day.Lessons.Count != 0).ToList();
            lastUpdatedDate = this.days.FirstOrDefault().WeekRoot.LastUpdated; //TODO
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            context = parent.Context;
            layoutInflater = LayoutInflater.From(context);

            switch (viewType)
            {
                case CardTag:
                    scheduleView = layoutInflater.Inflate(Resource.Layout.layout_card_schedule, parent, false);
                    viewHolder = new ScheduleCardFragmentAdapterViewHolder(scheduleView);
                    break;

                case EndTag:
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
                case CardTag:
                    var vh = (ScheduleCardFragmentAdapterViewHolder)viewHolder;

                    var day = days[position];

                    recyclerViewSchedule = vh.recyclerView;

                    recyclerViewSchedule.HasFixedSize = true;

                    var adapter = new ScheduleCardRowAdapter(new List<object>(day.Lessons), day.Date);
                    recyclerViewSchedule.SetAdapter(adapter);
                    recyclerViewSchedule.SetLayoutManager(new LinearLayoutManager(context));
                    break;

                case EndTag:
                    var vhe = (ScheduleCardEndFragmentAdapterViewHolder)viewHolder;

                    var tv = vhe.textView;

                    tv.Text = context.GetString(Resource.String.updated) + " " + lastUpdatedDate.ToString(CultureInfo.InvariantCulture);
                    break;
            }
        }

        public override int ItemCount => days.Count + 1;

        public override int GetItemViewType(int position)
        {
            return position == days.Count ? EndTag : CardTag;
        }

        private class ScheduleCardFragmentAdapterViewHolder : RecyclerView.ViewHolder
        {
            public readonly RecyclerView recyclerView;

            public ScheduleCardFragmentAdapterViewHolder(View itemView) : base(itemView)
            {
                recyclerView = itemView.FindViewById<RecyclerView>(Resource.Id.recyclerview_card_schedule);
            }
        }

        private class ScheduleCardEndFragmentAdapterViewHolder : RecyclerView.ViewHolder
        {
            public readonly TextView textView;

            public ScheduleCardEndFragmentAdapterViewHolder(View itemView) : base(itemView)
            {
                textView = itemView.FindViewById<TextView>(Resource.Id.textview_card_schedule_end);
            }
        }
    }
}