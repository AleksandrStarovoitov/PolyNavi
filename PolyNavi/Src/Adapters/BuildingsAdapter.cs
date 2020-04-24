using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace Polynavi.Droid.Adapters
{
    public class MainBuildingTag
    {
        public string MainBuildingString { get; set; }

        public override string ToString() => MainBuildingString;
    }

    public class BuildingsAdapter : ArrayAdapter<object>
    {
        private readonly Context context;
        private const int MainBuildingTypeTag = 0, OtherBuildingsTypeTag = 1;
        public override int ViewTypeCount => 2;

        public BuildingsAdapter(Context context, IList<object> buildings) : base(context, 0, buildings)
        {
            this.context = context;
        }

        public override int GetItemViewType(int position)
        {
            return GetItem(position) switch
            {
                string _ => OtherBuildingsTypeTag,
                MainBuildingTag _ => MainBuildingTypeTag,
                _ => -1 //TODO Exception?
            };
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = GetItem(position);
            var itemType = GetItemViewType(position);

            if (convertView == null)
            {
                convertView = GetInflatedLayoutForType(itemType);
            }

            var buildingTextView = convertView.FindViewById<TextView>(Resource.Id.textview_route);
            buildingTextView.Text = item.ToString();

            return convertView;
        }

        private View GetInflatedLayoutForType(int layoutType)
        {
            var inflater = LayoutInflater.From(context);

            return layoutType switch
            {
                MainBuildingTypeTag => inflater.Inflate(Resource.Layout.layout_route_row_mainbuilding, null),
                OtherBuildingsTypeTag => inflater.Inflate(Resource.Layout.layout_route_row_otherbuildings, null),
                _ => inflater.Inflate(Resource.Layout.layout_route_row_mainbuilding, null)
            };
        }
    }
}
