using System.Collections.Generic;

using Android.Content;
using Android.Views;
using Android.Widget;

namespace PolyNavi
{
    public class MainBuildingTag
	{
		public string MainBuildingString { get; set; }

		public override string ToString()
		{
			return MainBuildingString;
		}
	}

	public class BuildingsAdapter : ArrayAdapter<object>
	{
        private Context context;
        private TextView building;
        private object item;
        private int type;
        private const int MainBuilding = 0, OtherBuildings = 1;

		public BuildingsAdapter(Context context, List<object> buildings) : base(context, 0, buildings)
		{
			this.context = context;
		}

		public override int GetItemViewType(int position)
		{
			if (GetItem(position) is string)
			{
				return OtherBuildings;
			}
			else if (GetItem(position) is MainBuildingTag)
			{
				return MainBuilding;
			}
			return -1;
		}

		public override int ViewTypeCount => 2;

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			item = GetItem(position);
			type = GetItemViewType(position);
			if (convertView == null)
			{
				convertView = GetInflatedLayoutForType(type);
			}

			building = convertView.FindViewById<TextView>(Resource.Id.textview_route);
			building.Text = item.ToString();

			return convertView;
		}

		private View GetInflatedLayoutForType(int layoutType)
		{
			switch (layoutType)
			{
				case MainBuilding:
					return LayoutInflater.From(context).Inflate(Resource.Layout.layout_route_row_mainbuilding, null);
				case OtherBuildings:
					return LayoutInflater.From(context).Inflate(Resource.Layout.layout_route_row_otherbuildings, null);
				default:
					return LayoutInflater.From(context).Inflate(Resource.Layout.layout_route_row_mainbuilding, null);
			}
		}
	}
}




