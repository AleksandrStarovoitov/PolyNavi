using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using PolyNavi.Extensions;
using PolyNaviLib.BL;

namespace PolyNavi.Adapters
{
    internal class ScheduleCardRowAdapter : RecyclerView.Adapter
    {
        private readonly Context context;
        private readonly List<object> lessons;
        private View scheduleView;
        private ScheduleCardRowLessonViewHolder lessonViewHolder;
        private ScheduleCardRowTitleViewHolder titleViewHolder;
        private readonly CultureInfo cultureInfo = new CultureInfo("ru-RU"); //TODO ?
        private const int TitleTag = 0, LessonTag = 1;
        public override int ItemCount => lessons.Count;

        public ScheduleCardRowAdapter(Context context, List<object> lessons)
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

                case TitleTag:
                    scheduleView = layoutInflater.Inflate(Resource.Layout.layout_card_row_title_schedule, parent, false);
                    viewHolder = new ScheduleCardRowTitleViewHolder(scheduleView);
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

                case TitleTag:
                    SetupTitleViewHolder(viewHolder, position);
                    break;
            }
        }

        private void SetupLessonViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            lessonViewHolder = viewHolder as ScheduleCardRowLessonViewHolder;

            //TODO Null check
            var lesson = (Lesson)lessons[position];

            var roomTextView = lessonViewHolder.roomTextView;
            var buildingTextView = lessonViewHolder.buildingTextView;
            var subjectTextView = lessonViewHolder.subjectTextView;
            var startTimeTextView = lessonViewHolder.startTimeTextView;
            var endTimeTextView = lessonViewHolder.endTimeTextView;
            var typeTextView = lessonViewHolder.typeTextView;
            var teacherTextView = lessonViewHolder.teacherTextView;
            var lmsUrlTextView = lessonViewHolder.lmsUrlTextView;
            var groupTextView = lessonViewHolder.groupTextView;

            roomTextView.Text = "ауд. " + lesson.Auditories[0].Name;
            buildingTextView.Text = lesson.Auditories[0].Building.Name + ", ";
            subjectTextView.Text = lesson.Subject;
            startTimeTextView.Text = lesson.Time_Start.ToString("HH:mm", cultureInfo);
            endTimeTextView.Text = lesson.Time_End.ToString("HH:mm", cultureInfo);
            typeTextView.Text = lesson.TypeObj.Name.Replace("Лабораторные", "Лаб.")
                                            .Replace("Курсовое проектирование", "Курс."); //TODO property in axml?

            //TODO Refactor
            var hasTeachers = lesson.Teachers != null && lesson.Teachers.Any();

            if (hasTeachers)
            {
                CombineTeachers(teacherTextView, lesson);
            }
            else
            {
                AddRulesToView(groupTextView, new[] { (LayoutRules.Below, Resource.Id.textview_card_buildingnumber_row_lesson_schedule) });
                RemoveView(teacherTextView);
            }

            if (String.IsNullOrEmpty(lesson.Lms_Url))
            {
                RemoveView(lmsUrlTextView);

                if (hasTeachers)
                {
                    AddRulesToView(groupTextView, new[] { (LayoutRules.Below, Resource.Id.textview_card_teacher_row_lesson_schedule) });
                }
            }
            else
            {
                lmsUrlTextView.PaintFlags = Android.Graphics.PaintFlags.UnderlineText;
                lmsUrlTextView.Click += delegate
                {
                    var lmsUrl = lesson.Lms_Url;
                    var lmsUrlLink = new Intent(Intent.ActionView, Android.Net.Uri.Parse(lmsUrl));

                    context.StartActivity(lmsUrlLink);
                };

                if (!hasTeachers)
                {
                    AddRulesToView(lmsUrlTextView, new[] { (LayoutRules.Below, Resource.Id.textview_card_buildingnumber_row_lesson_schedule) });
                    AddRulesToView(groupTextView, new[] { (LayoutRules.Below, Resource.Id.textview_card_lms_url_row_lesson_schedule) });
                }
            }

            groupTextView.Text = HasSubgroups() ? GetSubgroup() : GetFirstGroupName();

            bool HasSubgroups() => lesson.Additional_Info.Any();
            string GetSubgroup() => lesson.Additional_Info;
            string GetFirstGroupName() => lesson.Groups.First().Name; //TODO Null check?
        }

        private void AddRulesToView(View view, (LayoutRules rule, int resourceId)[] rulesWithIds)
        {
            var layoutParams = (RelativeLayout.LayoutParams)view.LayoutParameters;

            foreach (var ruleWithId in rulesWithIds)
            {
                layoutParams.AddRule(ruleWithId.rule, ruleWithId.resourceId);
            }
            
            view.LayoutParameters = layoutParams;
        }

        private void RemoveView(View view)
        {
            ((ViewGroup)scheduleView).RemoveView(view);
        }

        private static void CombineTeachers(TextView teacherTextView, Lesson lesson) //TODO Refactor
        {
            teacherTextView.Text = string.Join(", ", lesson.Teachers.Select(t => t.Full_Name).ToArray());
        }

        private void SetupTitleViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            titleViewHolder = viewHolder as ScheduleCardRowTitleViewHolder;

            //TODO Null check
            var title = (TitleTag)lessons[position];
            var dateTextView = titleViewHolder.dateTextView;
            var dayOfWeekTextView = titleViewHolder.dayOfWeekTextView;

            dateTextView.Text = title.Date.ToString("M", cultureInfo);
            var date = title.Date.ToString("dddd", cultureInfo);
            dayOfWeekTextView.Text = date.FirstCharToUpper();
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

            internal ScheduleCardRowLessonViewHolder(View itemView) : base(itemView)
            {
                roomTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_room_row_lesson_schedule);
                buildingTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_buildingnumber_row_lesson_schedule);
                subjectTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_subject_row_lesson_schedule);
                startTimeTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_starttime_row_lesson_schedule);
                endTimeTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_endtime_row_lesson_schedule);
                typeTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_type_row_lesson_schedule);
                teacherTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_teacher_row_lesson_schedule);
                lmsUrlTextView = ItemView.FindViewById<TextView>(Resource.Id.textview_card_lms_url_row_lesson_schedule);
                groupTextView = itemView.FindViewById<TextView>(Resource.Id.textview_card_group_row_lesson_schedule);
            }
        }

        private class ScheduleCardRowTitleViewHolder : RecyclerView.ViewHolder
        {
            internal readonly TextView dateTextView;
            internal readonly TextView dayOfWeekTextView;

            internal ScheduleCardRowTitleViewHolder(View itemView) : base(itemView)
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
}