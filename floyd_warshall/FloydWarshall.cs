﻿using System.Collections.Generic;
using FloydWarshall;

public class FloydWarshall<T>
{

    const int MAX_VALUE = int.MaxValue / 2;
    private readonly List<T> nodes = new List<T>();
    private readonly Dictionary<T, List<Connection<T>>> _connections = new Dictionary<T, List<Connection<T>>>();
    private int[,] pathMatrix;
    private int[,] distanceMatrix;

    private int[,] DistanceMatrix(int[,] graph)
    {
        int verticesCount = graph.GetLength(0);
        int[,] distance = new int[verticesCount, verticesCount];
        this.pathMatrix = new int[verticesCount, verticesCount];

        for (int i = 0; i < verticesCount; ++i)
            for (int j = 0; j < verticesCount; ++j)
            {
                distance[i, j] = graph[i, j];
                pathMatrix[i, j] = j;
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
                        pathMatrix[i, j] = pathMatrix[i, k];
                    }
                }
            }
        }
        distanceMatrix = distance;
        return distance;
    }

    private int[,] GenerateMatrix()
    {
        int size = nodes.Count;
        int[,] matrix = new int[size, size];
        for(int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                matrix[i, j] = VertexDistance(nodes[i], nodes[j]);    
            }
        }
        return matrix;
    }

    private int VertexDistance(T origin, T destiny)
    {
        if (origin.Equals(destiny))
        {
            return 0;
        }

        foreach (Connection<T> c in _connections[origin])
        {
            if (c.node.Equals(destiny))
            {
                return c.weight;
            }
        }

        return MAX_VALUE;
    }

    public bool IsConnected(T node, T secondNode)
    {
        if (!nodes.Contains(node) || !nodes.Contains(secondNode))
            return false;
        return _connections[node].Exists(c => c.node.Equals(secondNode));
    }

    public void Connect(T node, T secondNode, int weight)
    {
        if (!nodes.Contains(node) ||
            !nodes.Contains(secondNode) ||
            _connections[node].Exists(c => c.node.Equals(secondNode))
        )
        {
            return;
        }
        distanceMatrix = null;
        var connection = new Connection<T>
        {
            node = secondNode,
            weight = weight
        };
        _connections[node].Add(connection);
    }

    public bool Contains(T node)
    {
        return nodes.Contains(node);
    }

    public void AddNode(T node)
    {
        if (!Contains(node))
        {
            _connections[node] = new List<Connection<T>>();
            nodes.Add(node);
        }
    }

    public T[] Path(T origin, T destiny)
    {
        if (distanceMatrix == null)
        {
            DistanceMatrix(GenerateMatrix());
        }
        int origin_position = nodes.IndexOf(origin);
        int destiny_position = nodes.IndexOf(destiny);
        if (distanceMatrix[origin_position, destiny_position] == MAX_VALUE)
        {
            return new T[] { };
        }

        List<T> temp = new List<T>();
        
        temp.Add(nodes[origin_position]);
        while (origin_position != destiny_position)
        {
            origin_position = pathMatrix[origin_position, destiny_position];
            temp.Add(nodes[origin_position]);
        }
        return temp.ToArray();
    }
}