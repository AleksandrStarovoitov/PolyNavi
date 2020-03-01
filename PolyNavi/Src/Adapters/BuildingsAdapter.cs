using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace PolyNavi.Adapters
{
    public class MainBuildingTag
    {
        public string MainBuildingString { get; set; }

        public override string ToString() => MainBuildingString;
    }

    public class BuildingsAdapter : ArrayAdapter<object>
    {
        private readonly Context context;
        private TextView buildingTextView;
        private object item;
        private int itemType;
        private const int MainBuildingTypeTag = 0, OtherBuildingsTypeTag = 1;

        public BuildingsAdapter(Context context, IList<object> buildings) : base(context, 0, buildings)
        {
            this.context = context;
        }

        public override int GetItemViewType(int position)
        {
            if (GetItem(position) is string)
            {
                return OtherBuildingsTypeTag;
            }

            if (GetItem(position) is MainBuildingTag)
            {
                return MainBuildingTypeTag;
            }
            return -1;
        }

        public override int ViewTypeCount => 2;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            item = GetItem(position);
            itemType = GetItemViewType(position);
            if (convertView == null)
            {
                convertView = GetInflatedLayoutForType(itemType);
            }

            buildingTextView = convertView.FindViewById<TextView>(Resource.Id.textview_route);
            buildingTextView.Text = item.ToString();

            return convertView;
        }

        private View GetInflatedLayoutForType(int layoutType)
        {
            switch (layoutType)
            {
                case MainBuildingTypeTag:
                    return LayoutInflater.From(context).Inflate(Resource.Layout.layout_route_row_mainbuilding, null);
                case OtherBuildingsTypeTag:
                    return LayoutInflater.From(context).Inflate(Resource.Layout.layout_route_row_otherbuildings, null);
                default:
                    return LayoutInflater.From(context).Inflate(Resource.Layout.layout_route_row_mainbuilding, null);
            }
        }
    }
}