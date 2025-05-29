using BestDelivery;
using C3.Models;
using C3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
namespace C3
{
    public partial class MainWindow : Window
    {
        private List<C3.Models.Order> orders = new();
        private C3.Models.Point depot = new() { X = 55.75, Y = 37.61 };
        private Graph graph = new();
        private TrafficFactors trafficFactors = new();
        private AStarSolver aStarSolver = new();
        public MainWindow()
        {
            InitializeComponent();
            ordersGrid.ItemsSource = orders;
        }
        private VectorInfluence vectorInfluence = new VectorInfluence();
        private void btnGetOrders_Click(object sender, RoutedEventArgs e)
        {
            orders = OrderArrays.GetOrderArray1()
                .Select(o => new C3.Models.Order
                {
                    ID = o.ID,
                    Priority = o.Priority,
                    Destination = new C3.Models.Point { X = o.Destination.X, Y = o.Destination.Y }
                })
                .ToList();
            ordersGrid.ItemsSource = null;
            ordersGrid.ItemsSource = orders;
            BuildAndDisplayRoute();
        }
        private void btnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            var rnd = new Random();
            var newOrder = new C3.Models.Order
            {
                ID = orders.Any() ? orders.Max(o => o.ID) + 1 : 1,
                Destination = new C3.Models.Point
                {
                    X = depot.X + rnd.NextDouble() * 0.1 - 0.05,
                    Y = depot.Y + rnd.NextDouble() * 0.1 - 0.05
                },
                Priority = Math.Round(rnd.NextDouble(), 2)
            };
            orders.Add(newOrder);
            ordersGrid.ItemsSource = null;
            ordersGrid.ItemsSource = orders;
            BuildAndDisplayRoute();
        }
        private void BuildAndDisplayRoute()
        {
            if (orders == null || orders.Count == 0)
            {
                MessageBox.Show("Нет заказов для построения маршрута.");
                routeTextBox.Text = "";
                routeCostTextBox.Text = "";
                graphCanvas.Children.Clear();
                return;
            }
            graph.BuildMatrix(orders, depot);

            Console.WriteLine("Узлы графа:");
            foreach (var key in graph.NodeKeys)
            {
                if (graph.Nodes.TryGetValue(key, out var p))
                    Console.WriteLine($"Ключ: {key}, X: {p.X}, Y: {p.Y}");
            }
            vectorInfluence.Initialize(graph.Nodes.Count);
            var adjustedMatrix = (double[,])graph.AdjacencyMatrix.Clone();
            vectorInfluence.ApplyFactors(ref adjustedMatrix);
            var route = aStarSolver.FindRoute(adjustedMatrix, graph.Nodes, depot, orders);
            if (route == null || route.Count == 0)
            {
                MessageBox.Show("Не удалось построить маршрут.");
                routeTextBox.Text = "";
                routeCostTextBox.Text = "";
                graphCanvas.Children.Clear();
                return;
            }
            if (route.Count > 2)
            {
                var subRoute = route.Skip(1).Take(route.Count - 2);
                routeTextBox.Text = string.Join(" -> ", subRoute);
            }
            else
            {
                routeTextBox.Text = "";
            }

            var bestOrders = ConvertOrders(orders);
            var bestDepot = ConvertPoint(depot);
            if (RoutingTestLogic.TestRoutingSolution(bestDepot, bestOrders, route.ToArray(), out double cost))
                routeCostTextBox.Text = cost.ToString("F2");
            else
                routeCostTextBox.Text = "Ошибка маршрута";

            DrawGraph(route);

            Console.WriteLine($"Количество заказов: {orders.Count}");
            Console.WriteLine($"Количество узлов графа: {graph.NodeKeys?.Count ?? 0}");
            Console.WriteLine($"Маршрут: {(route == null ? "null" : string.Join("->", route))}");
        }
        private BestDelivery.Point ConvertPoint(C3.Models.Point p) => new() { X = p.X, Y = p.Y };
        private BestDelivery.Order[] ConvertOrders(List<C3.Models.Order> orders)
        {
            return orders.Select(o => new BestDelivery.Order
            {
                ID = o.ID,
                Priority = o.Priority,
                Destination = ConvertPoint(o.Destination)
            }).ToArray();
        }
        private void DrawGraph(List<int> route)
        {
            graphCanvas.Children.Clear();
            if (graph.Nodes.Count == 0 || graph.NodeKeys.Count == 0)
                return;
            double padding = 20;
            double width = graphCanvas.ActualWidth - 2 * padding;
            double height = graphCanvas.ActualHeight - 2 * padding;
            double minX = graph.Nodes.Values.Min(p => p.X);
            double maxX = graph.Nodes.Values.Max(p => p.X);
            double minY = graph.Nodes.Values.Min(p => p.Y);
            double maxY = graph.Nodes.Values.Max(p => p.Y);
            double scaleX = width / (maxX - minX == 0 ? 1 : maxX - minX);
            double scaleY = height / (maxY - minY == 0 ? 1 : maxY - minY);
            double scale = Math.Min(scaleX, scaleY);
            double offsetX = (graphCanvas.ActualWidth - ((maxX - minX) * scale)) / 2;
            double offsetY = (graphCanvas.ActualHeight - ((maxY - minY) * scale)) / 2;
            var orderKeys = orders.Select((_, i) => i).ToHashSet();
            Dictionary<int, (double x, double y)> nodeCoords = new();
            foreach (int key in graph.NodeKeys)
            {
                if (!graph.Nodes.TryGetValue(key, out var point))
                    continue;
                double x = (point.X - minX) * scale + offsetX;
                double y = (point.Y - minY) * scale + offsetY;
                nodeCoords[key] = (x, y);
                var color = key == -1 ? Brushes.Red :
                            orderKeys.Contains(key) ? Brushes.Orange :
                            Brushes.Blue;
                var ellipse = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    Fill = color,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                Canvas.SetLeft(ellipse, x - 10);
                Canvas.SetTop(ellipse, y - 10);
                graphCanvas.Children.Add(ellipse);
                var label = new Label
                {
                    Content = key == -1 ? "Depot" : key.ToString(),
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    Background = Brushes.White,
                    Padding = new Thickness(0),
                    Margin = new Thickness(0)
                };
                Canvas.SetLeft(label, x + 10);
                Canvas.SetTop(label, y - 10);
                graphCanvas.Children.Add(label);
            }
            for (int i = 0; i < route.Count - 1; i++)
            {
                int fromKey = route[i];
                int toKey = route[i + 1];

                if (!nodeCoords.ContainsKey(fromKey) || !nodeCoords.ContainsKey(toKey))
                    continue;
                var (x1, y1) = nodeCoords[fromKey];
                var (x2, y2) = nodeCoords[toKey];
                var line = new Line
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    Stroke = Brushes.Green,
                    StrokeThickness = 3
                };
                graphCanvas.Children.Add(line);
                DrawArrow(x1, y1, x2, y2);
            }
        }
        private void DrawArrow(double x1, double y1, double x2, double y2)
        {
            double angle = Math.Atan2(y2 - y1, x2 - x1);
            double arrowLength = 10;
            double arrowAngle = Math.PI / 6;
            var arrowPoint1 = new System.Windows.Point(
                x2 - arrowLength * Math.Cos(angle - arrowAngle),
                y2 - arrowLength * Math.Sin(angle - arrowAngle));
            var arrowPoint2 = new System.Windows.Point(
                x2 - arrowLength * Math.Cos(angle + arrowAngle),
                y2 - arrowLength * Math.Sin(angle + arrowAngle));
            var arrowLine1 = new Line
            {
                X1 = x2,
                Y1 = y2,
                X2 = arrowPoint1.X,
                Y2 = arrowPoint1.Y,
                Stroke = Brushes.Green,
                StrokeThickness = 2
            };
            var arrowLine2 = new Line
            {
                X1 = x2,
                Y1 = y2,
                X2 = arrowPoint2.X,
                Y2 = arrowPoint2.Y,
                Stroke = Brushes.Green,
                StrokeThickness = 2
            };
            graphCanvas.Children.Add(arrowLine1);
            graphCanvas.Children.Add(arrowLine2);
        }
    }
}