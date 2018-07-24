using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
        [OptionalField]
        public int FloorPartNumber = 0;
		public List<GraphNode> Neighbours { get; set; } = new List<GraphNode>();
		public bool IsIntermediate { get; set; } = false;
		public bool IsStairs { get; set; } = false;
		public Point Point { get; set; } = new Point();
    }

	
}
