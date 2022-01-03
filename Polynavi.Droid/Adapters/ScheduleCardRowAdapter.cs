using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Polynavi.Common.Models;
using Polynavi.Droid.Extensions;

namespace Polynavi.Droid.Adapters
{
    internal class ScheduleCardRowAdapter : RecyclerView.Adapter
    {
        private readonly Context context;
        private readonly List<Lesson> lessons;
        private View scheduleView;
        private ScheduleCardRowLessonViewHolder lessonViewHolder;
        private readonly CultureInfo cultureInfo = new CultureInfo("ru-RU"); //TODO ?
        private const int LessonTag = 1;
        public override int ItemCount => lessons.Count;

        public ScheduleCardRowAdapter(Context context, List<Lesson> lessons)
        {
            this.context = context;
            this.lessons = lessons;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var context = parent.Context;
            var layoutInflater = LayoutInflater.From(context);
            RecyclerView.ViewHolder viewHolder = null;

            switch (viewType)
            {
                case LessonTag:
                    scheduleView = layoutInflater.Inflate(Resource.Layout.layout_card_row_lesson_schedule, parent, false);
                    viewHolder = new ScheduleCardRowLessonViewHolder(scheduleView);
                    break;
            }

            return viewHolder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            switch (viewHolder.ItemViewType) //TODO Switch by type?
            {
                case LessonTag:
                    SetupLessonViewHolder(viewHolder, position);
                    break;
            }
        }

        private void SetupLessonViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            lessonViewHolder = viewHolder as ScheduleCardRowLessonViewHolder;

            //TODO Null check
            var lesson = lessons[position];

            var roomTextView = lessonViewHolder.roomTextView;
            var buildingTextView = lessonViewHolder.buildingTextView;
            var subjectTextView = lessonViewHolder.subjectTextView;
            var startTimeTextView = lessonViewHolder.startTimeTextView;
            var endTimeTextView = lessonViewHolder.endTimeTextView;
            var typeTextView = lessonViewHolder.typeTextView;
            var teacherTextView = lessonViewHolder.teacherTextView;
            var lmsUrlTextView = lessonViewHolder.lmsUrlTextView;
            var groupTextView = lessonViewHolder.groupTextView;
            var dayOfWeekTextView = lessonViewHolder.dayOfWeekTextView;
            var dateTextView = lessonViewHolder.dateTextView;

            roomTextView.Text = "ауд. " + lesson.Auditories[0].Name;
            buildingTextView.Text = lesson.Auditories[0].Building.Name + ", ";
            subjectTextView.Text = lesson.Subject;
            startTimeTextView.Text = lesson.Time_Start.ToString("HH:mm", cultureInfo) + " -";
            endTimeTextView.Text = lesson.Time_End.ToString("HH:mm", cultureInfo);
            typeTextView.Text = lesson.TypeObj.Name.Replace("Лабораторные", "Лаб.")
                .Replace("Курсовое проектирование", "Курс."); //TODO property in axml?
            dateTextView.Text = lesson.Day.Date.ToString("M", cultureInfo);
            dayOfWeekTextView.Text = lesson.Day.Date.ToString("dddd", cultureInfo).FirstCharToUpper();
                
            //TODO Refactor
            var hasTeachers = lesson.Teachers != null && lesson.Teachers.Any();

            if (hasTeachers)
            {
                CombineTeachers(teacherTextView, lesson);
            }
            else
            {
                AddRulesToView(groupTextView,
                    new[] { (LayoutRules.Below, Resource.Id.textview_card_buildingnumber_row_lesson_schedule) });
                RemoveView(lessonViewHolder.relativeLayout, teacherTextView);
            }

            if (string.IsNullOrEmpty(lesson.Lms_Url))
            {
                RemoveView(lessonViewHolder.relativeLayout, lmsUrlTextView);

                if (hasTeachers)
                {
                    AddRulesToView(groupTextView,
                        new[] { (LayoutRules.Below, Resource.Id.textview_card_teacher_row_lesson_schedule) });
                }
            }
            else
            {
                lmsUrlTextView.PaintFlags = PaintFlags.UnderlineText;
                lmsUrlTextView.Click += delegate
                {
                    var lmsUrl = lesson.Lms_Url;
                    var lmsUrlLink = new Intent(Intent.ActionView, Uri.Parse(lmsUrl));

                    context.StartActivity(lmsUrlLink);
                };

                if (!hasTeachers)
                {
                    AddRulesToView(lmsUrlTextView,
                        new[] { (LayoutRules.Below, Resource.Id.textview_card_buildingnumber_row_lesson_schedule) });
                    AddRulesToView(groupTextView,
                        new[] { (LayoutRules.Below, Resource.Id.textview_card_lms_url_row_lesson_schedule) });
                }
            }

            groupTextView.Text = HasSubgroups(lesson) ? GetSubgroup(lesson) : GetFirstGroupName(lesson);

            if (position != 0)
            {
                var dayOfWeek = lessonViewHolder.dayOfWeekTextView;
                var date = lessonViewHolder.dateTextView;
                RemoveView(lessonViewHolder.relativeLayout, dayOfWeek);
                RemoveView(lessonViewHolder.relativeLayout, date);
            }
        }

        private static string GetFirstGroupName(Lesson lesson) => lesson.Groups.First().Name;

        private static string GetSubgroup(Lesson lesson) => lesson.Additional_Info;

        private static bool HasSubgroups(Lesson lesson) => lesson.Additional_Info.Any();

        private static void AddRulesToView(View view, IEnumerable<(LayoutRules rule, int resourceId)> rulesWithIds)
        {
            var layoutParams = (RelativeLayout.LayoutParams)view.LayoutParameters;

            foreach (var (rule, resourceId) in rulesWithIds)
            {
                layoutParams.AddRule(rule, resourceId);
            }

            view.LayoutParameters = layoutParams;
        }

        private void RemoveView(View from, View view)
        {
            ((ViewGroup)from).RemoveViewInLayout(view);
        }

        private static void CombineTeachers(TextView teacherTextView, Lesson lesson) //TODO Refactor
        {
            teacherTextView.Text = string.Join(", ", lesson.Teachers.Select(t => t.Full_Name).ToArray());
        }

        private class ScheduleCardRowLessonViewHolder : RecyclerView.ViewHolder
        {
            internal readonly TextView buildingTextView;
            internal readonly TextView roomTextView;
            internal readonly TextView subjectTextView;
            internal readonly TextView startTimeTextView;
            internal readonly TextView endTimeTextView;
            internal readonly TextView typeTextView;
            internal readonly TextView teacherTextView;
            internal readonly TextView lmsUrlTextView;
            internal readonly TextView groupTextView;
            internal readonly TextView dayOfWeekTextView;
            internal readonly TextView dateTextView;
            internal readonly RelativeLayout relativeLayout;

            internal ScheduleCardRowLessonViewHolder(View itemView) : base(itemView)
            {
                roomTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_room_row_lesson_schedule);
                buildingTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_buildingnumber_row_lesson_schedule);
                subjectTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_subject_row_lesson_schedule);
                startTimeTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_starttime_row_lesson_schedule);
                endTimeTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_endtime_row_lesson_schedule);
                typeTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_type_row_lesson_schedule);
                teacherTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_teacher_row_lesson_schedule);
                lmsUrlTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_lms_url_row_lesson_schedule);
                groupTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_group_row_lesson_schedule);
                relativeLayout = itemView.FindViewById<RelativeLayout>(Resource.Id.relativelayout_row_schedule);
                dayOfWeekTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_date_row_title_schedule);
                dateTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_dayofweek_row_title_schedule);
            }
        }

        public override int GetItemViewType(int position)
        {
            return lessons[position] switch
            {
                Lesson _ => LessonTag,
                _ => -1
            };
        }
    }
}
