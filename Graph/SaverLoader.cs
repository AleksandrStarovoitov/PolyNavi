using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;

namespace Graph
{
    public class SaverLoader
    {
        private readonly IAssetsProvider assetProvider;

        public SaverLoader(IAssetsProvider assetProvider)
        {
            this.assetProvider = assetProvider;
        }

        public static void Save(Stream stream, GraphNode graph)
        {
            var bf = new BinaryFormatter();
            bf.Serialize(stream, graph);
        }

        public static GraphNode Load(Stream stream)
        {
            var bf = new BinaryFormatter();
            return (GraphNode)bf.Deserialize(stream);
        }

        public GraphNode LoadFromXmlDescriptor(string assetName)
        {
            using (var stream = assetProvider.Open(assetName))
            {
                return LoadFromXmlDescriptor(stream);
            }
        }

        private GraphNode LoadFromXmlDescriptor(Stream stream)
        {
            var doc = XDocument.Load(stream);
            var root = doc.Root;
            var data = root?.Element("Data");
            var linker = root?.Element("StairsLinker");
            var floorGraphes = new List<GraphNode>();
            var floors = data?.Elements("Floor");

            if (floors != null)
            {
                foreach (var floor in floors)
                {
                    var name = floor.Attribute("name")?.Value;

                    using (var graphStream = assetProvider.Open(name))
                    {
                        var floorGraph = Load(graphStream);
                        floorGraphes.Add(floorGraph);
                    }
                }
            }

            var allStairs = linker?.Elements("Stairs");
            foreach (var stairs in allStairs) //
            {
                foreach (var item in stairs.Elements("Item"))
                {
                    var floorId = (int)item.Attribute("id");
                    var floorNumber = (int)item.Attribute("floor");
                    var floorPartNumber = (int?)item.Attribute("part");

                    var others = from i in stairs.Elements("Item")
                                 where (int)i.Attribute("id") != floorId
                                 select i;

                    GraphNode floor, stairsNode;
                    if (floorPartNumber != null)
                    {
                        floor = floorGraphes.Single(g => (g.FloorNumber == floorNumber) && (g.FloorPartNumber == floorPartNumber));
                        stairsNode = Algorithms.FindNodeByIdFloorNumberAndFloorPartNumber(floor, floorId, floorNumber, (int)floorPartNumber);
                    }
                    else
                    {
                        floor = floorGraphes.Single(g => g.FloorNumber == floorNumber);
                        stairsNode = Algorithms.FindNodeByIdAndFloorNumber(floor, floorId, floorNumber);
                    }

                    foreach (var otherItem in others)
                    {
                        var otherId = (int)otherItem.Attribute("id");
                        var otherFloorNumber = (int)otherItem.Attribute("floor");
                        var otherFloorPartNumber = (int?)otherItem.Attribute("part");

                        GraphNode otherFloor, otherStairsNode;
                        if (otherFloorPartNumber != null)
                        {
                            otherFloor = floorGraphes.Single(g => (g.FloorNumber == otherFloorNumber) && (g.FloorPartNumber == otherFloorPartNumber));
                            otherStairsNode = Algorithms.FindNodeByIdFloorNumberAndFloorPartNumber(otherFloor, otherId, otherFloorNumber, (int)otherFloorPartNumber);
                        }
                        else
                        {
                            otherFloor = floorGraphes.Single(g => g.FloorNumber == otherFloorNumber);
                            otherStairsNode = Algorithms.FindNodeByIdAndFloorNumber(otherFloor, otherId, otherFloorNumber);
                        }
                        stairsNode.Neighbours.Add(otherStairsNode);
                    }
                }
            }

            return floorGraphes[0];
        }
    }
}
