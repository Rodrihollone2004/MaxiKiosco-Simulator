using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ThiefController : MonoBehaviour
{
    public Node currentNode;
    public List<Node> path = new List<Node>();
    public bool isBack;
    public bool WasHit { get; set; } = false;
    public bool IsStealing { get; set; }

    List<ProductInteractable> productsInWorld;
    public int MaxProductsToThief { get; set; } = 6;
    public int MaxAmountToThief { get; set; } = 4;

    public int newTotal { get; set; } = 0;

    public bool IsInRagdoll { get; private set; } = false;

    private void Start()
    {
        currentNode = AStarManager.instance.StartNode;
    }

    private void Update()
    {
        if (IsInRagdoll) return;

        if (currentNode != AStarManager.instance.EndNode && !isBack)
        {
            IsStealing = false;
            CreatePath();
        }
        else if (currentNode == AStarManager.instance.EndNode && !isBack)
        {
            IsStealing = true;
            AddRandomProducts();
            BackToStart();
            //audioSource.PlayOneShot(clientInCashRegister);
        }
        else if (currentNode != AStarManager.instance.StartNode && isBack)
        {
            CreatePath();
        }
        else if (currentNode == AStarManager.instance.StartNode && isBack)
        {
            gameObject.SetActive(false);
            newTotal = 0;
            isBack = false;
            WasHit = false;
        }
    }

    public void ResumeMovement()
    {
        IsInRagdoll = false;

        this.enabled = true;
    }

    public void GetHit()
    {
        if (!WasHit)
        {
            WasHit = true;

            Ragdoll ragdoll = GetComponent<Ragdoll>();
            if (ragdoll != null)
            {
                ragdoll.ActivateTemporaryRagdoll(2.5f);
                IsInRagdoll = true;

            }

            BackToStart();
        }
    }

    public void CreatePath()
    {
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
            isBack = true;

            path.Clear();

            List<Node> backPath = AStarManager.instance.GeneratePath(currentNode, AStarManager.instance.StartNode);
            if (backPath != null && backPath.Count > 0)
            {
                path = backPath;
            }
        }
        else if (!isBack && WasHit)
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

    public void AddRandomProducts()
    {
        productsInWorld = FindObjectsOfType<ProductInteractable>().ToList();
        productsInWorld.OrderBy(x => Random.value).ToList(); // Mezclar productos para que agarre random

        int productsToBuy = Random.Range(3, MaxProductsToThief); //variable que va a variar con mejoras

        int total = 0;
        int amountThief = 0;

        foreach (ProductInteractable productInWorld in productsInWorld)
        {
            if (amountThief > productsToBuy)
                continue;

            int amountProduct = Random.Range(1, MaxAmountToThief); //variable que va a variar con mejoras

            if (amountProduct > productInWorld.CurrentAmountProduct)
                amountProduct = productInWorld.CurrentAmountProduct;

            newTotal += total + (productInWorld.ProductData.Price * amountProduct);

            productInWorld.SubtractAmount(amountProduct);
            productInWorld.CheckDelete();
            amountThief++;
            Debug.Log($"Añadido al carrito: {productInWorld.ProductData.Name} (${productInWorld.ProductData.Price})");
        }

    }
}
