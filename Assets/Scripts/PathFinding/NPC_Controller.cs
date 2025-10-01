using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class NPC_Controller : MonoBehaviour
{
    public Node currentNode;
    public List<Node> path = new List<Node>();

    public bool isBack;

    public bool isInCashRegister;
    public bool isInDequeue;
    public bool isPaying;

    [Header("Sounds")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip clientInCashRegister;

    public static event Action onShowScreen;
    public Client client { get; private set; }
    [field: SerializeField] public Animator animatorNPC { get; private set; }

    private void Start()
    {
        currentNode = AStarManager.instance.StartNode;
        client = GetComponent<Client>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (currentNode != AStarManager.instance.EndNode && !isBack)
        {
            isInCashRegister = false;
            CreatePath();
        }
        else if (currentNode == AStarManager.instance.EndNode && !isBack && isInCashRegister == false)
        {
            isInCashRegister = true;
            client.AddRandomProductsToCart();
            client.CalculateCost();

            audioSource.PlayOneShot(clientInCashRegister);

            ClientQueueManager queueManager = FindObjectOfType<ClientQueueManager>();

            if (client.GetCart().Count > 0)
            {
                queueManager._clientQueue.Enqueue(client);
                queueManager.UpdateQueuePositions();

                if (client == queueManager.ClientQueue.Peek())
                    onShowScreen?.Invoke();
            }
            else if (client.GetCart().Count == 0)
            {
                BackToStart();
                StartCoroutine(queueManager.RemoveClient0(client));
            }

            transform.rotation = Quaternion.identity;
            animatorNPC.SetBool("IsWalking", path.Count > 0);
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
            isPaying = false;
            client.CanvasClientManager.ClearText();

            CashRegisterInteraction.onFinishPath -= BackToStart;
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

            path = AStarManager.instance.GeneratePath(currentNode, AStarManager.instance.EndNode);
        }
    }

    public void BackToStart()
    {
        if (currentNode == AStarManager.instance.EndNode && !isBack)
        {
            isInCashRegister = false;
            isBack = true;

            path.Clear();

            List<Node> backPath = AStarManager.instance.GeneratePath(currentNode, AStarManager.instance.StartNode);
            if (backPath != null && backPath.Count > 0)
            {
                path = backPath;
            }
        }
    }
}
