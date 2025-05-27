using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    public Node currentNode;
    public List<Node> path = new List<Node>();

    public bool isAllNodes;
    public bool isBack;

    [SerializeField] int nodesAmount = 3;

    private void Start()
    {
        currentNode = AStarManager.instance.startNode;
        CashRegisterInteraction.onFinishPath += BackToStart;
    }

    private void Update()
    {
        if (currentNode != AStarManager.instance.endNode && !isBack)
            CreatePath();
        else if (currentNode != AStarManager.instance.startNode && isBack)
            CreatePath();
        else if (currentNode == AStarManager.instance.startNode && isBack)
        {
            isBack = false;

            ClientQueueManager queueManager = FindObjectOfType<ClientQueueManager>();
            queueManager.RemoveClient();
        }

    }

    public void CreatePath()
    {
        if (path.Count > 0)
        {
            int x = 0;
            transform.position = Vector3.MoveTowards(transform.position, path[x].transform.position, 3 * Time.deltaTime);

            if (Vector3.Distance(transform.position, path[x].transform.position) < 0.05f)
            {
                currentNode = path[x];
                path.RemoveAt(x);
            }
        }
        else
        {
            Node[] nodes = FindObjectsOfType<Node>();

            if (!isAllNodes)
            {
                AStarManager.instance.CreateConnections(nodes);
                isAllNodes = true;
            }

            List<Node> intermediateNodes = new List<Node>();

            while (intermediateNodes.Count < nodesAmount)
            {
                Node randomNode = nodes[Random.Range(0, nodes.Length)];
                if (randomNode != currentNode && randomNode != AStarManager.instance.endNode && !intermediateNodes.Contains(randomNode))
                {
                    intermediateNodes.Add(randomNode);
                }
            }

            List<Node> totalPath = new List<Node>();

            Node lastNode = currentNode;

            foreach (Node intermediate in intermediateNodes)
            {
                List<Node> partialPath = AStarManager.instance.GeneratePath(lastNode, intermediate);
                if (partialPath != null && partialPath.Count > 0)
                {
                    totalPath.AddRange(partialPath);
                    lastNode = intermediate;
                }
            }

            List<Node> finalPath = AStarManager.instance.GeneratePath(lastNode, AStarManager.instance.endNode);
            if (finalPath != null && finalPath.Count > 0)
            {
                totalPath.AddRange(finalPath);
            }

            path = totalPath;
        }
    }

    public void BackToStart()
    {
        if (currentNode == AStarManager.instance.endNode && !isBack)
        {
            isBack = true;
            List<Node> backPath = AStarManager.instance.GeneratePath(currentNode, AStarManager.instance.startNode);
            if (backPath != null && backPath.Count > 0)
            {
                path.AddRange(backPath);
            }
        }
    }
}
