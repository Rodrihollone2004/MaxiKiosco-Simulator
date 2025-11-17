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
            else if (nodeIndex == 5)
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

            if (!AStarManager.instance.IsAllNodes)
            {
                Node[] nodes = FindObjectsOfType<Node>();
                AStarManager.instance.CreateConnections(nodes);
                AStarManager.instance.IsAllNodes = true;
            }

            path = AStarManager.instance.GeneratePath(currentNode, AStarManager.instance.EndTutoNode);
        }
    }

    public void BackToStart()
    {
        if (currentNode == AStarManager.instance.EndTutoNode && !isBack)
        {
            isBack = true;

            path.Clear();

            CreatePath();

            List<Node> backPath = AStarManager.instance.GeneratePath(currentNode, AStarManager.instance.StartNode);
            if (backPath != null && backPath.Count > 0)
            {
                path = backPath;
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
