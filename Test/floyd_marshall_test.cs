using System.Reflection;
using Moq;
using NExpect;
using NUnit.Framework;
using static NExpect.Expectations;

namespace FloydMarshalTest
{
    public interface INodeType { }

    public class Common {

        public FloydMarshall<INodeType> floydMarshallTwoNodes;
        public Mock<INodeType> node;
        public Mock<INodeType> secondNode;

        [SetUp]
        public void SetUp() {
            floydMarshallTwoNodes = new FloydMarshall<INodeType>();

            node = new Mock<INodeType>();
            secondNode = new Mock<INodeType>();

            floydMarshallTwoNodes.AddNode(node.Object);
            floydMarshallTwoNodes.AddNode(secondNode.Object);
            floydMarshallTwoNodes.Connect(node.Object, secondNode.Object, 10);
        }
    }

    [TestFixture]
    public class AddNode : Common
    {
        [Test]
        public void Add_the_node_to_the_graph()
        {
            FloydMarshall<INodeType> floydMarshall = new FloydMarshall<INodeType>();
            floydMarshall.AddNode(node.Object);
            Expect(floydMarshall.Contains(node.Object)).To.Equal(true);
        }
    }

    [TestFixture]
    public class Connect : Common {

        [Test]
        public void Connect_two_nodes()
        {
            FloydMarshall<INodeType> floydMarshall = new FloydMarshall<INodeType>();

            floydMarshall.AddNode(node.Object);
            floydMarshall.AddNode(secondNode.Object);
            floydMarshall.Connect(node.Object, secondNode.Object, 10);
            Expect(floydMarshall.IsConnected(node.Object, secondNode.Object)).To.Equal(true);
        }

        [Test]
        public void Does_not_connect_invalid_node()
        {
            FloydMarshall<INodeType> floydMarshall = new FloydMarshall<INodeType>();


            floydMarshall.Connect(node.Object, secondNode.Object, 10);
            Expect(floydMarshall.IsConnected(node.Object, secondNode.Object)).To.Equal(false);
        }

        [Test]
        public void Node_connection_invalidate_the_current_matrix()
        {
            floydMarshallTwoNodes.Path(node.Object, secondNode.Object);
            Expect(floydMarshallTwoNodes.GetProperty("distanceMatrix")).To.Not.Equal(null);
            floydMarshallTwoNodes.Connect(node.Object, secondNode.Object, 10);
            Expect(floydMarshallTwoNodes.GetProperty("distanceMatrix")).To.Equal(null);
        }
    }

    [TestFixture]
    public class GenerateMatrix : Common
    {
        [Test]
        public void Generates_the_correct_matrix_width()
        {
            FloydMarshall<INodeType> floydMarshall = new FloydMarshall<INodeType>();

            floydMarshall.AddNode(node.Object);
            floydMarshall.AddNode(secondNode.Object);
            floydMarshall.Connect(node.Object, secondNode.Object, 10);
            int[,] matrix = (int[,])floydMarshall.call("GenerateMatrix");
            Expect(matrix.GetLength(0)).To.Equal(2);
        }

        [Test]
        public void Generates_the_correct_matrix_height()
        {
            FloydMarshall<INodeType> floydMarshall = new FloydMarshall<INodeType>();

            floydMarshall.AddNode(node.Object);
            floydMarshall.AddNode(secondNode.Object);
            floydMarshall.Connect(node.Object, secondNode.Object, 10);
            int[,] matrix = (int[,])floydMarshall.call("GenerateMatrix");
            Expect(matrix.GetLength(1)).To.Equal(2);
        }

        [Test]
        public void Initialize_the_correct_initial_distances()
        {
            int[,] matrix = (int[,])floydMarshallTwoNodes.call("GenerateMatrix");
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
                floydMarshallTwoNodes.call(
                    "VertexDistance",
                    new object[] { node.Object, secondNode.Object }
                )
            ).To.Equal(10);
        }

        [Test]
        public void Return_max_int_value_if_nodes_are_not_connected()
        {
            Expect(
                floydMarshallTwoNodes.call(
                    "VertexDistance",
                    new object[] { secondNode.Object, node.Object }
                )
            ).To.Equal(int.MaxValue / 2);
        }

        [Test]
        public void Returns_0_if_nodes_are_equal()
        {

            Expect(
                floydMarshallTwoNodes.call(
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
            int[,] matrix = (int[,])floydMarshallTwoNodes.call("GenerateMatrix");
            int[,] distanceMatrix = (int[, ])floydMarshallTwoNodes.call(
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
            var floydMarshall= new FloydMarshall<INodeType>();

            var intialNode = new Mock<INodeType>();
            var middleNode = new Mock<INodeType>();
            var lastNode = new Mock<INodeType>();

            floydMarshall.AddNode(intialNode.Object);
            floydMarshall.AddNode(middleNode.Object);
            floydMarshall.AddNode(lastNode.Object);

            floydMarshall.Connect(intialNode.Object, middleNode.Object, 7);
            floydMarshall.Connect(middleNode.Object, lastNode.Object, 10);

            Expect(floydMarshall.Path(intialNode.Object, lastNode.Object))
                .To.Contain(middleNode.Object);
        }

        [Test]
        public void Returns_the_shortest_path()
        {
            var floydMarshall = new FloydMarshall<INodeType>();

            var intialNode = new Mock<INodeType>();
            var middleNode = new Mock<INodeType>();
            var secondMiddleNode = new Mock<INodeType>();
            var lastNode = new Mock<INodeType>();

            floydMarshall.AddNode(intialNode.Object);
            floydMarshall.AddNode(middleNode.Object);
            floydMarshall.AddNode(secondMiddleNode.Object);
            floydMarshall.AddNode(lastNode.Object);

            floydMarshall.Connect(intialNode.Object, middleNode.Object, 7);
            floydMarshall.Connect(middleNode.Object, lastNode.Object, 10);
            floydMarshall.Connect(intialNode.Object, secondMiddleNode.Object, 10);
            floydMarshall.Connect(secondMiddleNode.Object, lastNode.Object, 6);

            Expect(floydMarshall.Path(intialNode.Object, lastNode.Object))
                .To.Not.Contain(middleNode.Object);
            Expect(floydMarshall.Path(intialNode.Object, lastNode.Object))
                .To.Contain(secondMiddleNode.Object);
        }

        [Test]
        public void Return_null_if_no_path()
        {
            var floydMarshall = new FloydMarshall<INodeType>();

            var intialNode = new Mock<INodeType>();
            var middleNode = new Mock<INodeType>();
            var lastNode = new Mock<INodeType>();

            floydMarshall.AddNode(intialNode.Object);
            floydMarshall.AddNode(middleNode.Object);
            floydMarshall.AddNode(lastNode.Object);

            floydMarshall.Connect(intialNode.Object, middleNode.Object, 7);

            Expect(floydMarshall.Path(intialNode.Object, lastNode.Object))
                .To.Equal(null);
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