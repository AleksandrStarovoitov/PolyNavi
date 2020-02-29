using System.IO;

namespace Graph
{
    public interface IAssetsProvider
    {
        Stream Open(string asset);
    }
}
