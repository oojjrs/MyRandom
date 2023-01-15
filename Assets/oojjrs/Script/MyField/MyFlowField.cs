using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.oojjrs.Script.MyField
{
    public class MyFlowField
    {
        public interface NodeInterface
        {
            Vector2? Direction { get; }
            bool Target { get; }
            TileInterface Tile { get; }
        }

        public interface TileInterface
        {
            Vector2Int Coordinate { get; }
            IEnumerable<Vector2Int> Neighbors { get; }
            Vector2 Position { get; }
            bool Walkable { get; }
        }

        private class Node : NodeInterface
        {
            public Vector2? Direction { get; set; }
            public bool Fresh { get; set; }
            public bool Target { get; }
            public TileInterface Tile { get; }

            public Node(TileInterface tile, bool target)
            {
                Fresh = true;
                Target = target;
                Tile = tile;
            }
        }

        private Dictionary<Vector2Int, Node> Nodes { get; }
        private Node TargetNode { get; }

        public MyFlowField(TileInterface[] tiles, Vector2Int target)
        {
            Nodes = tiles.ToDictionary(tile => tile.Coordinate, tile => new Node(tile, tile.Coordinate == target));
            TargetNode = Nodes[target];
        }

        public IEnumerator CalculateAsync(Action onFinish, Func<bool> keepGoingOn)
        {
            if (keepGoingOn == default)
                keepGoingOn = () => true;

            var q = new Queue<Node>();
            q.Enqueue(TargetNode);

            var time = Time.time;
            while (keepGoingOn() && (q.Count > 0))
            {
                var node = q.Dequeue();
                var ret = GetNeighbors(node);
                if (ret.Any())
                {
                    var nodePosition = node.Tile.Position;
                    var neighbors = ret.ToArray();
                    foreach (var n in neighbors)
                    {
                        n.Fresh = false;

                        if ((n.Target == false) && n.Tile.Walkable)
                        {
                            n.Direction = (nodePosition - n.Tile.Position).normalized;
                            q.Enqueue(n);
                        }
                    }
                }

                if (Time.time - time > 0.01)
                {
                    yield return default;

                    time = Time.time;
                }
            }

            if (keepGoingOn())
                onFinish?.Invoke();
        }

        private IEnumerable<Node> GetNeighbors(Node node)
        {
            foreach (var coordinate in node.Tile.Neighbors)
            {
                if (Nodes.TryGetValue(coordinate, out var value) && value.Fresh)
                    yield return value;
            }
        }

        public NodeInterface GetNode(Vector2Int coordinate)
        {
            Nodes.TryGetValue(coordinate, out var node);
            return node;
        }
    }
}
