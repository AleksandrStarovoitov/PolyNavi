using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Polynavi.Common.Models;

namespace Polynavi.Droid.Adapters
{
    internal class TitleTag
    {
        public DateTime Date { get; set; }
    }

    internal class ScheduleCardFragmentAdapter : RecyclerView.Adapter
    {
        private readonly List<Day> days;
        private Context context;
        public override int ItemCount => days.Count + 1;

        private const int EndTag = 0, CardTag = 1;

        public ScheduleCardFragmentAdapter(IEnumerable<Day> days)
        {
            this.days = days.Where(day => day.Lessons.Count != 0).ToList();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            context = parent.Context;
            var layoutInflater = LayoutInflater.From(context);

            RecyclerView.ViewHolder viewHolder = null;
            View scheduleView;

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
            switch (viewHolder.ItemViewType) //TODO Switch by type?
            {
                case CardTag:
                    SetupScheduleViewHolder(viewHolder, position);
                    break;

                case EndTag:
                    SetupScheduleEndViewHolder(viewHolder);
                    break;
            }
        }

        private void SetupScheduleViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var scheduleCardViewHolder = viewHolder as ScheduleCardFragmentAdapterViewHolder;

            //TODO Null check
            var day = days[position];

            var scheduleRecyclerView = scheduleCardViewHolder.recyclerView;
            scheduleRecyclerView.HasFixedSize = true;

            //TODO
            var lessons = new List<object>() { new TitleTag() { Date = day.Date } };
            lessons.AddRange(day.Lessons);

            var adapter = new ScheduleCardRowAdapter(context, lessons);
            scheduleRecyclerView.SetAdapter(adapter);
            scheduleRecyclerView.SetLayoutManager(new LinearLayoutManager(context));
        }

        private void SetupScheduleEndViewHolder(RecyclerView.ViewHolder viewHolder)
        {
            var scheduleCardEndViewHolder = viewHolder as ScheduleCardEndFragmentAdapterViewHolder;

            //TODO Null check
            var textView = scheduleCardEndViewHolder.textView;

            var updatedOnText = context.GetString(Resource.String.updated);
            var lastUpdatedDate = days.FirstOrDefault()?.WeekRoot?.LastUpdated ?? DateTime.Now; //TODO
            var date = lastUpdatedDate.ToString(CultureInfo.InvariantCulture);
            textView.Text = $"{updatedOnText} {date}";
        }

        public override int GetItemViewType(int position)
        {
            return position == days.Count ? EndTag : CardTag;
        }

        private class ScheduleCardFragmentAdapterViewHolder : RecyclerView.ViewHolder
        {
            internal readonly RecyclerView recyclerView;

            internal ScheduleCardFragmentAdapterViewHolder(View itemView) : base(itemView)
            {
                recyclerView = itemView.FindViewById<RecyclerView>(Resource.Id.recyclerview_card_schedule);
            }
        }

        private class ScheduleCardEndFragmentAdapterViewHolder : RecyclerView.ViewHolder
        {
            internal readonly TextView textView;

            internal ScheduleCardEndFragmentAdapterViewHolder(View itemView) : base(itemView)
            {
                textView = itemView.FindViewById<TextView>(Resource.Id.textview_card_schedule_end);
            }
        }
    }
}
