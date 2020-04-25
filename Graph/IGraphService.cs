using System.IO;

namespace Graph
{
    public interface IGraphService
    {
        GraphNode Load(Stream stream);
        GraphNode LoadFromXmlDescriptor(string assetName);
        void Save(Stream stream, GraphNode graph);
    }
}
