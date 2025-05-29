using System;
using System.Collections.Generic;
namespace C3.Services
{
    public class AStarSolver
    {
        public List<int> FindRoute(double[,] graph, Dictionary<int, C3.Models.Point> nodes, C3.Models.Point depot, List<C3.Models.Order> orders)
        {
            int start = -1; 
            var targets = new HashSet<int>();
            foreach (var o in orders)
                targets.Add(o.ID);
            var cameFrom = new Dictionary<int, int>();
            var gScore = new Dictionary<int, double>();
            var fScore = new Dictionary<int, double>();
            var allNodes = new HashSet<int>(nodes.Keys);
            allNodes.Add(start);
            foreach (var node in allNodes)
            {
                gScore[node] = double.PositiveInfinity;
                fScore[node] = double.PositiveInfinity;
            }
            gScore[start] = 0;
            fScore[start] = Heuristic(start, targets, nodes);
            var openSet = new SortedSet<(double, int)>(Comparer<(double, int)>.Create((a, b) =>
            {
                int cmp = a.Item1.CompareTo(b.Item1);
                if (cmp == 0) return a.Item2.CompareTo(b.Item2);
                return cmp;
            }));
            openSet.Add((fScore[start], start));
            while (openSet.Count > 0)
            {
                var current = openSet.Min.Item2;
                if (targets.Count == 0 && current == start)
                    break;
                openSet.Remove(openSet.Min);
                if (targets.Contains(current))
                    targets.Remove(current);
                foreach (var neighbor in GetNeighbors(graph, current))
                {
                    double tentative_gScore = gScore[current] + graph[current + 1, neighbor + 1];
                    if (!gScore.ContainsKey(neighbor) || tentative_gScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentative_gScore;
                        fScore[neighbor] = tentative_gScore + Heuristic(neighbor, targets, nodes);
                        var existing = default((double, int));
                        bool found = false;
                        foreach (var x in openSet)
                        {
                            if (x.Item2 == neighbor)
                            {
                                existing = x;
                                found = true;
                                break;
                            }
                        }
                        if (found)
                            openSet.Remove(existing);
                        openSet.Add((fScore[neighbor], neighbor));
                    }
                }
                if (targets.Count == 0 && current == start)
                    break;
            }
            var route = ReconstructPath(cameFrom, start, orders);
            return route;
        }
        private IEnumerable<int> GetNeighbors(double[,] graph, int node)
        {
            int n = graph.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                if (graph[node + 1, i] > 0 && node != i - 1) 
                    yield return i - 1;
            }
        }
        private double Heuristic(int node, HashSet<int> targets, Dictionary<int, C3.Models.Point> nodes)
        {
            if (targets.Count == 0)
                return 0;
            double minDist = double.PositiveInfinity;
            if (!nodes.ContainsKey(node))
                return 0;
            var p1 = nodes[node];
            foreach (var t in targets)
            {
                if (!nodes.ContainsKey(t))
                    continue;
                var p2 = nodes[t];
                double dist = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
                if (dist < minDist)
                    minDist = dist;
            }
            return minDist;
        }
        private List<int> ReconstructPath(Dictionary<int, int> cameFrom, int start, List<C3.Models.Order> orders)
        {
            var route = new List<int>();
            route.Add(-1);
            foreach (var o in orders)
                route.Add(o.ID);
            route.Add(-1);
            return route;
        }
    }
}