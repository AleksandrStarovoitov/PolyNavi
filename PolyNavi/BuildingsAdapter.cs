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
	public class MainBuildingTag
	{
		public string MainBuildingString { get; set; }
	}

	public class BuildingsAdapter : ArrayAdapter<object>
	{
		private Context context;
		private TextView building;
		private string text;
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

			building = null;
			text = "";
			type = GetItemViewType(position);
			if (convertView == null)
			{

				convertView = GetInflatedLayoutForType(type);
			}

			building = convertView.FindViewById<TextView>(Resource.Id.textview_route);

			switch (type)
			{
				case MainBuilding:
					text = ((MainBuildingTag)item).MainBuildingString;
					break;
				case OtherBuildings:
					text = (string)item;
					break;
				default:
					text = (string)item;
					break;
			}

			building.Text = text;

			return convertView;
		}

		private View GetInflatedLayoutForType(int type)
		{
			switch (type)
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




