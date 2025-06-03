using System;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    public Node currentNode;
    public List<Node> path = new List<Node>();

    public bool isBack;

    [SerializeField] int nodesAmount = 3;

    public bool isInCashRegister;
    public bool isInDequeue;


    public static event Action onShowScreen;
    Client client;

    private void Start()
    {
        currentNode = AStarManager.instance.StartNode;
        client = GetComponent<Client>();
    }

    private void Update()
    {
        if (currentNode != AStarManager.instance.EndNode && !isBack)
        {
            isInCashRegister = false;
            CreatePath();
        }
        else if(currentNode == AStarManager.instance.EndNode && !isBack && isInCashRegister == false)
        {
            isInCashRegister = true;

            ClientQueueManager queueManager = FindObjectOfType<ClientQueueManager>();
            queueManager._clientQueue.Enqueue(client);
            queueManager.UpdateQueuePositions();
            onShowScreen?.Invoke();
        }
        else if (currentNode != AStarManager.instance.StartNode && isBack)
        {
            isInCashRegister = false;
            CreatePath();
        }
        else if (currentNode == AStarManager.instance.StartNode && isBack)
        {
            isBack = false;
            isInDequeue = true;

            CashRegisterInteraction.onFinishPath -= BackToStart;
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

            if (!AStarManager.instance.IsAllNodes)
            {
                AStarManager.instance.CreateConnections(nodes);
                AStarManager.instance.IsAllNodes = true;
            }

            List<Node> intermediateNodes = new List<Node>();

            while (intermediateNodes.Count < nodesAmount)
            {
                Node randomNode = nodes[UnityEngine.Random.Range(0, nodes.Length)];
                if (randomNode != currentNode && randomNode != AStarManager.instance.EndNode && !intermediateNodes.Contains(randomNode))
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

            List<Node> finalPath = AStarManager.instance.GeneratePath(lastNode, AStarManager.instance.EndNode);
            if (finalPath != null && finalPath.Count > 0)
            {
                totalPath.AddRange(finalPath);
            }

            path = totalPath;
        }
    }

    public void BackToStart()
    {
        if (currentNode == AStarManager.instance.EndNode && !isBack)
        {
            isInCashRegister = false;
            isBack = true;
            List<Node> backPath = AStarManager.instance.GeneratePath(currentNode, AStarManager.instance.StartNode);
            if (backPath != null && backPath.Count > 0)
            {
                path.AddRange(backPath);
            }
        }
    }
}
