using System;
using System.Collections.Generic;

namespace Graph
{
    public static class Algorithms
    {
        internal class GraphNodeWithParent
        {
            internal GraphNode Data { get; set; }
            internal GraphNodeWithParent Parent { get; set; }
        }

        public static List<GraphNode> CalculateRoute(GraphNode graph, string startName, string finishName)
        {
            var start = FindNodeByName(graph, startName);
            var finish = FindNodeByName(graph, finishName);

            if (start == null)
            {
                throw new GraphRoutingException($"Node with name {startName} could not be found");
            }
            if (finish == null)
            {
                throw new GraphRoutingException($"Node with name {finishName} could not be found");
            }

            var open = new Queue<GraphNodeWithParent>();
            var closed = new List<GraphNode>();

            open.Enqueue(new GraphNodeWithParent() { Data = start });
            while (open.Count > 0)
            {
                var node = open.Dequeue();
                closed.Add(node.Data);
                if (node.Data == finish)
                {
                    var route = new List<GraphNode>();
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
                    var badFloor = (neighbour.FloorNumber != start.FloorNumber && neighbour.FloorNumber != finish.FloorNumber && Math.Abs(neighbour.FloorNumber - start.FloorNumber) > 1);

                    var neighbourWithParent = new GraphNodeWithParent() { Data = neighbour, Parent = node };
                    if (!closed.Contains(neighbour) && (!badFloor || !neighbour.IsStairs))
                    {
                        open.Enqueue(neighbourWithParent);
                    }
                }
            }
            throw new GraphRoutingException($"Can't find route between {startName} and {finishName}");
        }

        internal static GraphNode FindNode(GraphNode graph, Predicate<GraphNode> predicate)
        {
            var bfsQueue = new Queue<GraphNode>();
            var closed = new List<GraphNode>();
            bfsQueue.Enqueue(graph);
            while (bfsQueue.Count > 0)
            {
                var node = bfsQueue.Dequeue();
                closed.Add(node);
                if (predicate(node))
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

        internal static GraphNode FindNodeByName(GraphNode graph, string name)
        {
            return FindNode(graph, (node) =>
            {
                return node.RoomName == name;
            });
        }

        internal static GraphNode FindNodeById(GraphNode graph, int id)
        {
            return FindNode(graph, (node) =>
            {
                return node.Id == id;
            });
        }

        internal static GraphNode FindNodeByIdAndFloorNumber(GraphNode graph, int id, int floorNumber)
        {
            return FindNode(graph, (node) =>
            {
                return node.Id == id && node.FloorNumber == floorNumber;
            });
        }

        internal static GraphNode FindNodeByIdFloorNumberAndFloorPartNumber(GraphNode graph, int id, int floorNumber, int floorPartNumber)
        {
            return FindNode(graph, (node) =>
            {
                return node.Id == id && node.FloorNumber == floorNumber && node.FloorPartNumber == floorPartNumber;
            });
        }
    }
}
