using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PolyNavi
{
	public class AssetsProvider : Graph.IAssetsProvider
	{
		Context context;

		public AssetsProvider(Context context)
		{
			this.context = context;
		}

		public Stream Open(string assetName)
		{
			return context.Assets.Open(assetName);
		}
	}
}