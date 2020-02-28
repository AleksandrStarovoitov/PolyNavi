using System.IO;

using Android.Content;

namespace PolyNavi
{
    public class AssetsProvider : Graph.IAssetsProvider
	{
        private readonly Context context;

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