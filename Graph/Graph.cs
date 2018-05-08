using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Graph
{
	[Serializable]
	public struct Point
	{
		public int X;
		public int Y;

		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

	[Serializable]
    public class GraphNode
    {
		public int Id { get; set; } = 0;
		public int RoomNumber { get; set; } = 0;
		public List<GraphNode> Neighbours { get; set; } = new List<GraphNode>();
		public bool IsIntermediate { get; set; } = false;
		public bool IsStairs { get; set; } = false;
		public Point Point { get; set; } = new Point();
    }

	public static class SaverLoader
	{
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
	}
}
