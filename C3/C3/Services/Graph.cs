using System.Collections.Generic;
using System.Linq;
using C3.Models;
using BestDelivery;
namespace C3.Services
{
    public class Graph
    {
        public Dictionary<int, C3.Models.Point> Nodes { get; private set; } = new();
        public double[,] AdjacencyMatrix { get; private set; }
        public List<int> NodeKeys { get; private set; } = new List<int>();
        public Dictionary<int, int> KeyToIndex { get; private set; } = new();
        public void BuildMatrix(List<C3.Models.Order> orders, C3.Models.Point depot)
        {
            Nodes.Clear();
            Nodes[-1] = depot;
            foreach (var order in orders)
            {
                Nodes[order.ID] = order.Destination;
            }
            NodeKeys = Nodes.Keys.OrderBy(k => k).ToList();
            KeyToIndex.Clear();
            for (int i = 0; i < NodeKeys.Count; i++)
            {
                KeyToIndex[NodeKeys[i]] = i;
            }
            int n = Nodes.Count;
            AdjacencyMatrix = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    AdjacencyMatrix[i, j] = RoutingTestLogic.CalculateDistance(
                        ConvertToBestPoint(Nodes[NodeKeys[i]]),
                        ConvertToBestPoint(Nodes[NodeKeys[j]])
                    );
                }
            }
        }
        private BestDelivery.Point ConvertToBestPoint(C3.Models.Point p)
        {
            return new BestDelivery.Point { X = p.X, Y = p.Y };
        }
    }
}
