using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Graph
{
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
