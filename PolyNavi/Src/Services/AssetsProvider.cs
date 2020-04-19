using System.IO;
using Android.Content;
using Graph;

namespace PolyNavi.Services
{
    public class AssetsProvider : IAssetsProvider
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
