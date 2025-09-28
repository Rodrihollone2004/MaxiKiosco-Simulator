using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public static AStarManager instance;

    [field: SerializeField] public Node StartNode { get; private set; }
    [field: SerializeField] public Node EndNode { get; private set; }
    [field: SerializeField] public Node BoxRemoverNode { get; private set; }

    [field: SerializeField] public bool IsAllNodes { get; set; }

    private void Awake()
    {
        instance = this;
    }

    public List<Node> GeneratePath(Node start, Node end)
    {
        List<Node> openSet = new List<Node>();

        foreach (Node n in FindObjectsOfType<Node>())
        {
            n.gScore = float.MaxValue;
        }

        start.gScore = 0;
        start.hScore = Vector3.Distance(start.transform.position, end.transform.position);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            int lowestF = default;

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FScore() < openSet[lowestF].FScore())
                {
                    lowestF = i;
                }
            }

            Node currentNode = openSet[lowestF];
            openSet.Remove(currentNode);

            if (currentNode == end)
            {
                List<Node> path = new List<Node>();

                path.Insert(0, end);

                while (currentNode != start)
                {
                    currentNode = currentNode.cameFrom;
                    path.Add(currentNode);
                }

                path.Reverse();
                return path;
            }

            foreach (Node connectedNode in currentNode.connections)
            {
                float heldGScore = currentNode.gScore + Vector3.Distance(currentNode.transform.position, connectedNode.transform.position);

                if (heldGScore < connectedNode.gScore)
                {
                    connectedNode.cameFrom = currentNode;
                    connectedNode.gScore = heldGScore;
                    connectedNode.hScore = Vector3.Distance(connectedNode.transform.position, end.transform.position);

                    if (!openSet.Contains(connectedNode))
                    {
                        openSet.Add(connectedNode);
                    }
                }
            }
        }

        return null;
    }

    public void CreateConnections(Node[] nodeList)
    {
        for (int i = 0; i < nodeList.Length; i++)
        {
            for (int j = i + 1; j < nodeList.Length; j++)
            {
                if (Vector3.Distance(nodeList[i].transform.position, nodeList[j].transform.position) <= 3f)
                {
                    ConnectNodes(nodeList[i], nodeList[j]);
                    ConnectNodes(nodeList[j], nodeList[i]);
                }
            }
        }
    }

    private void ConnectNodes(Node from, Node to)
    {
        if (from == to) return;

        if (!from.connections.Contains(to))
            from.connections.Add(to);

    }
}
