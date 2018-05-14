using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Linq;
using System.Linq;

namespace Graph
{
	public class SaverLoader
	{
		IAssetsProvider assetProvider;

		public SaverLoader(IAssetsProvider assetProvider)
		{
			this.assetProvider = assetProvider;
		}

		public static void Save(Stream stream, GraphNode graph)
		{
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(stream, graph);
		}

		public static GraphNode Load(Stream stream)
		{
			BinaryFormatter bf = new BinaryFormatter();
			return (GraphNode)bf.Deserialize(stream);
		}

		public GraphNode LoadFromXmlDescriptor(string assetName)
		{
			using (var stream = assetProvider.Open(assetName))
			{
				return LoadFromXmlDescriptor(stream);
			}
		}
		
		public GraphNode LoadFromXmlDescriptor(Stream stream)
		{
			XDocument doc = XDocument.Load(stream);
			var root = doc.Root;
			var data = root.Element("Data");
			var linker = root.Element("StairsLinker");
			int floorCount = (int)root.Attribute("floorCount");

			List<GraphNode> floorGraphes = new List<GraphNode>();

			var floors = data.Elements("Floor");
			foreach (var floor in floors)
			{
				string name = floor.Attribute("name").Value;
				int number = (int)floor.Attribute("number");

				using (var graphStream = assetProvider.Open(name))
				{
					GraphNode floorGraph = Load(graphStream);
					if (floorGraph.FloorNumber == 1)
					{
						var n = Algorithms.FindNodeById(floorGraph, 161);
						int a = 0;
					}
					floorGraphes.Add(floorGraph);
				}
			}

			var allStairs = linker.Elements("Stairs");
			foreach (var stairs in allStairs)
			{
				foreach (var item in stairs.Elements("Item"))
				{
					int floorId = (int)item.Attribute("id");
					int floorNumber = (int)item.Attribute("floor");
					var others = from i in stairs.Elements("Item")
								 where (int)i.Attribute("id") != floorId
								 select i;
					GraphNode floor = floorGraphes.Where(g => g.FloorNumber == floorNumber).Single();
					GraphNode stairsNode = Algorithms.FindNodeByIdAndFloorNumber(floor, floorId, floorNumber);
					foreach (var otherItem in others)
					{
						int otherId = (int)otherItem.Attribute("id");
						int otherFloorNumber = (int)otherItem.Attribute("floor");
						GraphNode otherFloor = floorGraphes.Where(g => g.FloorNumber == otherFloorNumber).Single();
						GraphNode otherStairsNode = Algorithms.FindNodeByIdAndFloorNumber(otherFloor, otherId, otherFloorNumber);
						stairsNode.Neighbours.Add(otherStairsNode);
					}
				}
			}
			
			return floorGraphes[0];
		}
	}
}
