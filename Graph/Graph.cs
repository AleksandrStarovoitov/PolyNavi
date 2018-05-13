using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

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
		public string RoomName { get; set; } = "";
		public int FloorNumber { get; set; } = 1;
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

	public static class Algorithms
	{
		[Serializable]
		public class GraphRoutingException : Exception
		{
			public GraphRoutingException() { }
			public GraphRoutingException(string message) : base(message) { }
			public GraphRoutingException(string message, Exception inner) : base(message, inner) { }
			protected GraphRoutingException(
			  System.Runtime.Serialization.SerializationInfo info,
			  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
		}

		internal class GraphNodeWithParent
		{
			internal GraphNode Data { get; set; }
			internal GraphNodeWithParent Parent { get; set; }
		}

		public static List<GraphNode> CalculateRoute(GraphNode graph, string startName, string finishName)
		{
			var start = FindNode(graph, startName);
			var finish = FindNode(graph, finishName);
			if (start == null)
			{
				throw new GraphRoutingException($"Node with name {startName} could not be found");
			}
			if (finish == null)
			{
				throw new GraphRoutingException($"Node with name {finishName} could not be found");
			}

			Queue<GraphNodeWithParent> open = new Queue<GraphNodeWithParent>();
			List<GraphNode> closed = new List<GraphNode>();

			open.Enqueue(new GraphNodeWithParent() { Data = start });
			while (open.Count > 0)
			{
				var node = open.Dequeue();
				closed.Add(node.Data);
				if (node.Data == finish)
				{
					List<GraphNode> route = new List<GraphNode>();
					var routeNode = node;
					while (routeNode.Data != start)
					{
						route.Add(routeNode.Data);
						routeNode = routeNode.Parent;
					}
					route.Add(start);
					route.Reverse();
					return route;
				}
				foreach (var neighbour in node.Data.Neighbours)
				{
					var neighbourWithParent = new GraphNodeWithParent() { Data = neighbour, Parent = node };
					if (!closed.Contains(neighbour))
					{
						open.Enqueue(neighbourWithParent);
					}
					else
					{
						int a = 0;
					}
				}
			}
			throw new GraphRoutingException($"Can't find route between {startName} and {finishName}");
		}

		private static GraphNode FindNode(GraphNode graph, string name)
		{
			Queue<GraphNode> bfsQueue = new Queue<GraphNode>();
			List<GraphNode> closed = new List<GraphNode>();
			bfsQueue.Enqueue(graph);
			while (bfsQueue.Count > 0)
			{
				var node = bfsQueue.Dequeue();
				closed.Add(node);
				if (node.RoomName == name)
				{
					return node;
				}
				else
				{
					foreach (var neighbour in node.Neighbours)
					{
						if (!closed.Contains(neighbour))
						{
							bfsQueue.Enqueue(neighbour);
						}
					}
				}
			}
			return null;
		}
	}
}
