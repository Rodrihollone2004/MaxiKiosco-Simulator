using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGuider : MonoBehaviour
{
    public Node currentNode;
    public List<Node> path = new List<Node>();
    public bool isBack;
    private Animator animatorNPC;
    [SerializeField] private int nodeIndex;
    [SerializeField] int nodesAmount = 3;

    [SerializeField] List<Node> nodesTuto = new List<Node>();
    [field: SerializeField] public GameObject CanvasTuto { get; private set; }
    private void Start()
    {
        nodeIndex = 0;
        currentNode = AStarManager.instance.StartTutoNode;
        animatorNPC = GetComponent<Animator>();
        ChangeTutoNode();
    }

    private void Update()
    {
        if (currentNode != AStarManager.instance.EndTutoNode && !isBack)
        {
            CreatePath();
        }
        else if (currentNode == AStarManager.instance.EndTutoNode && !isBack)
        {
            if (nodeIndex == 3)
                transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            else if(nodeIndex == 5)
                transform.rotation = Quaternion.Euler(0f, -90f, 0f);

            animatorNPC.SetBool("IsWalking", path.Count > 0);
        }
        else if (currentNode != AStarManager.instance.StartNode && isBack)
        {
            CreatePath();
        }
        else if (currentNode == AStarManager.instance.StartNode && isBack)
        {
            Destroy(gameObject);
        }
    }

    public void CreatePath()
    {
        animatorNPC.SetBool("IsWalking", path.Count > 0);

        if (path.Count > 0)
        {
            int x = 0;
            Vector3 targetPos = path[x].transform.position;

            Vector3 direction = (targetPos - transform.position).normalized;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 3 * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
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
                if (randomNode != currentNode && randomNode != AStarManager.instance.EndTutoNode && !intermediateNodes.Contains(randomNode))
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

            List<Node> finalPath = AStarManager.instance.GeneratePath(lastNode, AStarManager.instance.EndTutoNode);
            if (finalPath != null && finalPath.Count > 0)
            {
                totalPath.AddRange(finalPath);
            }

            path = totalPath;
        }
    }

    public void BackToStart()
    {
        if (currentNode == AStarManager.instance.EndTutoNode && !isBack)
        {
            isBack = true;
            List<Node> backPath = AStarManager.instance.GeneratePath(currentNode, AStarManager.instance.StartNode);
            if (backPath != null && backPath.Count > 0)
            {
                path.AddRange(backPath);
            }
        }
    }

    public void ChangeTutoNode()
    {
        if (nodesTuto.Count == nodeIndex)
            return;

        path.Clear();
        nodeIndex++;
        AStarManager.instance.EndTutoNode = nodesTuto[nodeIndex - 1];
    }

}
