using System.Collections.Generic;
using System.Reflection;
using floyd_warshall;
using Moq;
using NExpect;
using NUnit.Framework;
using static NExpect.Expectations;

namespace FloydWarshallTest
{
    public interface INodeType { }

    public class Common
    {

        public FloydWarshall<INodeType> floydWarshallTwoNodes;
        public Mock<INodeType> node;
        public Mock<INodeType> secondNode;

        [SetUp]
        public void SetUp()
        {
            floydWarshallTwoNodes = new FloydWarshall<INodeType>();

            node = new Mock<INodeType>();
            secondNode = new Mock<INodeType>();

            floydWarshallTwoNodes.AddNode(node.Object);
            floydWarshallTwoNodes.AddNode(secondNode.Object);
            floydWarshallTwoNodes.Connect(node.Object, secondNode.Object, 10);
        }
    }

    [TestFixture]
    public class AddNode : Common
    {
        [Test]
        public void Add_the_node_to_the_graph()
        {
            FloydWarshall<INodeType> floydWarshall = new FloydWarshall<INodeType>();
            floydWarshall.AddNode(node.Object);
            Expect(floydWarshall.Contains(node.Object)).To.Equal(true);
        }

        [Test]
        public void Does_not_allow_equal_nodes()
        {
            FloydWarshall<INodeType> floydWarshall = new FloydWarshall<INodeType>();
            floydWarshall.AddNode(node.Object);
            floydWarshall.AddNode(node.Object);
            List<INodeType> nodes = (List<INodeType>)floydWarshall.GetProperty("nodes");
            Expect(nodes.Count).To.Equal(1);
        }
    }

    [TestFixture]
    public class Connect : Common
    {

        [Test]
        public void Connect_two_nodes()
        {
            FloydWarshall<INodeType> floydWarshall = new FloydWarshall<INodeType>();

            floydWarshall.AddNode(node.Object);
            floydWarshall.AddNode(secondNode.Object);
            floydWarshall.Connect(node.Object, secondNode.Object, 10);
            Expect(floydWarshall.IsConnected(node.Object, secondNode.Object)).To.Equal(true);
        }

        [Test]
        public void Does_not_connect_invalid_node()
        {
            FloydWarshall<INodeType> floydWarshall = new FloydWarshall<INodeType>();

            floydWarshall.Connect(node.Object, secondNode.Object, 10);
            Expect(floydWarshall.IsConnected(node.Object, secondNode.Object)).To.Equal(false);
        }

        [Test]
        public void Does_not_generate_two_connections()
        {
            FloydWarshall<INodeType> floydWarshall = new FloydWarshall<INodeType>();

            floydWarshall.AddNode(node.Object);
            floydWarshall.AddNode(secondNode.Object);

            floydWarshall.Connect(node.Object, secondNode.Object, 10);
            floydWarshall.Connect(node.Object, secondNode.Object, 9);

            Dictionary<INodeType, List<Connection<INodeType>>> _connections = (Dictionary<INodeType, List<Connection<INodeType>>>)floydWarshall.GetProperty("_connections");
            Expect(_connections[node.Object].Count).To.Equal(1);
        }

        [Test]
        public void Node_connection_invalidate_the_current_matrix()
        {
            floydWarshallTwoNodes.Path(node.Object, secondNode.Object);
            Expect(floydWarshallTwoNodes.GetProperty("distanceMatrix")).To.Not.Equal(null);

            var thirdNode = new Mock<INodeType>();
            floydWarshallTwoNodes.AddNode(thirdNode.Object);
            floydWarshallTwoNodes.Connect(node.Object, thirdNode.Object, 10);

            Expect(floydWarshallTwoNodes.GetProperty("distanceMatrix")).To.Equal(null);
        }
    }

    [TestFixture]
    public class GenerateMatrix : Common
    {
        [Test]
        public void Generates_the_correct_matrix_width()
        {
            FloydWarshall<INodeType> floydWarshall = new FloydWarshall<INodeType>();

            floydWarshall.AddNode(node.Object);
            floydWarshall.AddNode(secondNode.Object);
            floydWarshall.Connect(node.Object, secondNode.Object, 10);
            int[,] matrix = (int[,])floydWarshall.call("GenerateMatrix");
            Expect(matrix.GetLength(0)).To.Equal(2);
        }

        [Test]
        public void Generates_the_correct_matrix_height()
        {
            FloydWarshall<INodeType> floydWarshall = new FloydWarshall<INodeType>();

            floydWarshall.AddNode(node.Object);
            floydWarshall.AddNode(secondNode.Object);
            floydWarshall.Connect(node.Object, secondNode.Object, 10);
            int[,] matrix = (int[,])floydWarshall.call("GenerateMatrix");
            Expect(matrix.GetLength(1)).To.Equal(2);
        }

        [Test]
        public void Initialize_the_correct_initial_distances()
        {
            int[,] matrix = (int[,])floydWarshallTwoNodes.call("GenerateMatrix");
            Expect(matrix[0, 1]).To.Equal(10);
        }

    }

    [TestFixture]
    public class Distance : Common
    {
        [Test]
        public void Return_the_distance_between_two_nodes()
        {
            Expect(
                floydWarshallTwoNodes.call(
                    "VertexDistance",
                    new object[] { node.Object, secondNode.Object }
                )
            ).To.Equal(10);
        }

        [Test]
        public void Return_max_int_value_if_nodes_are_not_connected()
        {
            Expect(
                floydWarshallTwoNodes.call(
                    "VertexDistance",
                    new object[] { secondNode.Object, node.Object }
                )
            ).To.Equal(int.MaxValue / 2);
        }

        [Test]
        public void Returns_0_if_nodes_are_equal()
        {

            Expect(
                floydWarshallTwoNodes.call(
                    "VertexDistance",
                    new object[] { node.Object, node.Object }
                )
            ).To.Equal(0);
        }
    }

    [TestFixture]
    public class DistanceMatrix : Common
    {
        [Test]
        public void Returns_the_distance_between_the_nodes()
        {
            int[,] matrix = (int[,])floydWarshallTwoNodes.call("GenerateMatrix");
            int[,] distanceMatrix = (int[,])floydWarshallTwoNodes.call(
                "DistanceMatrix",
                matrix
            );
            Expect(distanceMatrix[0, 1]).To.Equal(10);
        }
    }

    [TestFixture]
    public class Path
    {
        [Test]
        public void Return_the_path_to_the_node()
        {
            var floydWarshall = new FloydWarshall<INodeType>();

            var intialNode = new Mock<INodeType>();
            var middleNode = new Mock<INodeType>();
            var lastNode = new Mock<INodeType>();

            floydWarshall.AddNode(intialNode.Object);
            floydWarshall.AddNode(middleNode.Object);
            floydWarshall.AddNode(lastNode.Object);

            floydWarshall.Connect(intialNode.Object, middleNode.Object, 7);
            floydWarshall.Connect(middleNode.Object, lastNode.Object, 10);

            Expect(floydWarshall.Path(intialNode.Object, lastNode.Object))
                .To.Contain(middleNode.Object);
            Expect(floydWarshall.Path(intialNode.Object, lastNode.Object).Count)
                .To.Equal(3);
        }

        [Test]
        public void Returns_the_shortest_path()
        {
            var floydWarshall = new FloydWarshall<INodeType>();

            var intialNode = new Mock<INodeType>();
            var middleNode = new Mock<INodeType>();
            var secondMiddleNode = new Mock<INodeType>();
            var lastNode = new Mock<INodeType>();

            floydWarshall.AddNode(intialNode.Object);
            floydWarshall.AddNode(middleNode.Object);
            floydWarshall.AddNode(secondMiddleNode.Object);
            floydWarshall.AddNode(lastNode.Object);

            floydWarshall.Connect(intialNode.Object, middleNode.Object, 7);
            floydWarshall.Connect(middleNode.Object, lastNode.Object, 10);
            floydWarshall.Connect(intialNode.Object, secondMiddleNode.Object, 10);
            floydWarshall.Connect(secondMiddleNode.Object, lastNode.Object, 6);

            Expect(floydWarshall.Path(intialNode.Object, lastNode.Object))
                .To.Not.Contain(middleNode.Object);
            Expect(floydWarshall.Path(intialNode.Object, lastNode.Object))
                .To.Contain(secondMiddleNode.Object);
        }

        [Test]
        public void Return_empty_array_if_no_path()
        {
            var floydWarshall = new FloydWarshall<INodeType>();

            var intialNode = new Mock<INodeType>();
            var middleNode = new Mock<INodeType>();
            var lastNode = new Mock<INodeType>();

            floydWarshall.AddNode(intialNode.Object);
            floydWarshall.AddNode(middleNode.Object);
            floydWarshall.AddNode(lastNode.Object);

            floydWarshall.Connect(intialNode.Object, middleNode.Object, 7);

            Expect(floydWarshall.Path(intialNode.Object, lastNode.Object))
                .To.Be.Empty();
        }
    }

    static class AccessExtensions
    {
        public static object call(this object o, string methodName, params object[] args)
        {
            var mi = o.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (mi != null)
            {
                return mi.Invoke(o, args);
            }
            return null;
        }

        public static object GetProperty(this object o, string propertyName)
        {
            var mi = o.GetType().GetField(propertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return mi.GetValue(o);
        }
    }
}
