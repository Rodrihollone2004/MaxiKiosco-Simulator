using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BoxCollector : MonoBehaviour, IInteractable
{
    public Node currentNode;
    public List<Node> path = new List<Node>();
    public bool isBack;
    public bool canRemove;
    [SerializeField] int amountBoxes = 500;

    private Animator animatorNPC;
    public bool CanBePickedUp => false;

    AudioSource audioSource;
    [SerializeField] AudioClip paymentSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentNode = AStarManager.instance.StartNode;
        animatorNPC = GetComponent<Animator>();
    }

    public void Interact()
    {
        CheckBoxes();
    }

    private void Update()
    {
        if (currentNode != AStarManager.instance.BoxRemoverNode && !isBack)
        {
            CreatePath();
        }
        else if (currentNode == AStarManager.instance.BoxRemoverNode && !isBack)
        {
            canRemove = true;
            //audioSource.PlayOneShot(clientInCashRegister);
            transform.rotation = Quaternion.Euler(0, 90, 0);
            animatorNPC.SetBool("IsWalking", path.Count > 0);
        }
        else if (currentNode != AStarManager.instance.StartNode && isBack)
        {
            CreatePath();
        }
        else if (currentNode == AStarManager.instance.StartNode && isBack)
        {
            gameObject.SetActive(false);
            isBack = false;
        }
    }

    private void CheckBoxes()
    {
        if (canRemove)
        {
            BoxStackZone boxStack = FindObjectOfType<BoxStackZone>();

            if (boxStack != null && boxStack.StackedBoxes.Count > 0)
            {
                for (int i = 0; i < boxStack.StackedBoxes.Count; i++)
                {
                    PlayerEconomy player = FindObjectOfType<PlayerEconomy>();
                    player.ReceivePayment(amountBoxes);

                    if (!audioSource.isPlaying)
                        audioSource.PlayOneShot(paymentSound);

                    Destroy(boxStack.StackedBoxes[i]);
                }

                boxStack.StackedBoxes.Clear();

                BackToStart();
            }
            else
                BackToStart();
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

            path = AStarManager.instance.GeneratePath(currentNode, AStarManager.instance.BoxRemoverNode);
        }
    }

    public void BackToStart()
    {
        if (currentNode == AStarManager.instance.BoxRemoverNode && !isBack)
        {
            isBack = true;

            path.Clear();

            List<Node> backPath = AStarManager.instance.GeneratePath(currentNode, AStarManager.instance.StartNode);
            if (backPath != null && backPath.Count > 0)
            {
                path = backPath;
            }
        }
    }

    public void Highlight()
    {
    }

    public void Unhighlight()
    {
    }
}
