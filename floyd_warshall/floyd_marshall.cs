// A C# program for Floyd Warshall All 
// Pairs Shortest Path algorithm. 

using System;
using System.Collections.Generic;
using System.Linq;
using floyd_warshall;

public class FloydMarshall<T>
{

    const int MAX_VALUE = int.MaxValue / 2;

    List<T> nodes = new List<T>();
    Dictionary<T, List<Connection<T>>> _connections = new Dictionary<T, List<Connection<T>>>();
    public int[,] path;

    public int[,] DistanceMatrix(int[,] graph)
    {
        int verticesCount = graph.GetLength(0);
        int[,] distance = new int[verticesCount, verticesCount];
        path = new int[verticesCount, verticesCount];

        for (int i = 0; i < verticesCount; ++i)
            for (int j = 0; j < verticesCount; ++j)
            {
                distance[i, j] = graph[i, j];
                if (i != j)
                    path[i, j] = j + 1;
            }

        for (int k = 0; k < verticesCount; k++)
        {
            for (int i = 0; i < verticesCount; i++)
            {
                for (int j = 0; j < verticesCount; j++)
                {
                    if (distance[i, k] + distance[k, j] < distance[i, j])
                    {
                        distance[i, j] = distance[i, k] + distance[k, j];
                        path[i, j] = path[i, k];
                    }
                }
            }
        }
        return distance;
    }

    public int[,] GenerateMatrix()
    {
        int size = nodes.Count();
        int[,] matrix = new int[size, size];
        for(int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                matrix[i, j] = Distance(nodes[i], nodes[j]);    
            }
        }
        return matrix;
    }

    public int Distance(T origin, T destiny)
    {
        if (origin.Equals(destiny))
            return 0;
        var connection = _connections[origin].FirstOrDefault(c => c.node.Equals(destiny));
        if (connection == null)
            return MAX_VALUE;
        return connection.weight;
    }

    public bool IsConnected(T node, T secondNode)
    {
        if (!nodes.Contains(node) || !nodes.Contains(secondNode))
            return false;
        return _connections[node].Exists(c => c.node.Equals(secondNode));
    }

    public void Connect(T node, T secondNode, int weight)
    {
        if (!nodes.Contains(node) || !nodes.Contains(secondNode))
            return;
        var connection = new Connection<T>
        {
            node = secondNode,
            weight = weight
        };
        _connections[node].Add(connection);
    }

    public bool Contains(T node)
    {
        return (nodes.Contains(node));
    }

    public void AddNode(T node)
    {
        _connections[node] = new List<Connection<T>>();
        nodes.Add(node);
    }

    public List<T> Path(T object1, T object2)
    {
        
    }
}
