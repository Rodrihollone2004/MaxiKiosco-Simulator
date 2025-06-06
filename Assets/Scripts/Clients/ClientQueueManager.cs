using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ClientQueueManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameObject clientPrefab;
    [SerializeField] private Transform payPosition;
    [SerializeField] private Transform queueStartPosition;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float timeBetweenClients = 2f;
    [SerializeField] private float distanceBetweenClients = 1.5f;
    [SerializeField] private ClientTrashSpawner trashSpawner;
    [SerializeField] private GateInteractable gate;

    [Header("References UI")]
    [SerializeField] private TMP_Text payText;

    [Header("Events")]
    public UnityEvent<Client> OnClientServed;
    public UnityEvent OnQueueUpdated;

    public Queue<Client> _clientQueue = new Queue<Client>();
    private List<GameObject> _clientPool = new List<GameObject>();

    public Queue<Client> ClientQueue { get => _clientQueue; set => _clientQueue = value; }
    public TMP_Text PayText { get => payText; set => payText = value; }

    private void Start()
    {
        InitializePool(5);
        SpawnInitialClients(2);
    }

    private void InitializePool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            CreateNewClientInPool();
        }
    }

    private GameObject CreateNewClientInPool()
    {
        GameObject client = Instantiate(clientPrefab, queueStartPosition.position, Quaternion.identity);
        client.SetActive(false);
        _clientPool.Add(client);
        return client;
    }

    private Client GetClientFromPool()
    {
        GameObject client = _clientPool.Find(c => !c.activeInHierarchy) ?? CreateNewClientInPool();
        client.SetActive(true);
        Client clientComp = client.GetComponent<Client>();

        NPC_Controller npc = client.GetComponent<NPC_Controller>();
        clientComp.NpcController = npc;

        return clientComp;
    }

    public IEnumerator RemoveClient()
    {
        if (_clientQueue.Count == 0) yield return null;

        Client client = _clientQueue.Dequeue();
        OnClientServed?.Invoke(client);

        Debug.Log("esperando que termine la vuelta");
        yield return new WaitUntil(() => client.NpcController.isInDequeue);
        Debug.Log("eliminando cliente: " + client.name);

        if (Random.value < 0.25f)
        {
            trashSpawner.SpawnTrash();
        }

        ReturnClientToPool(client);
        StartCoroutine(WaitAndSpawnNewClient());
        OnQueueUpdated?.Invoke();
    }

    public void UpdateQueuePositions()
    {
        int index = 0;
        foreach (Client client in _clientQueue)
        {
            if (client.NpcController.isInCashRegister)
            {
                Vector3 targetPos = CalculateQueuePosition(index, payPosition);
                StartCoroutine(MoveClientToPosition(client.gameObject, targetPos));
                index++;
            }
        }
    }

    private Vector3 CalculateQueuePosition(int index, Transform pos)
    {
        return pos.position + Vector3.back * distanceBetweenClients * index;
    }

    private IEnumerator MoveClientToPosition(GameObject client, Vector3 targetPos)
    {
        float elapsedTime = 0f;
        Vector3 startPos = client.transform.position;

        while (elapsedTime < 1f)
        {
            client.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime);
            elapsedTime += Time.deltaTime * moveSpeed;
            yield return null;
        }

        client.transform.position = targetPos;
    }

    public void ReturnClientToPool(Client client)
    {
        client.gameObject.SetActive(false);
        client.transform.position = queueStartPosition.position;
    }

    private void SpawnInitialClients(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Client client = GetClientFromPool();
            client.transform.position = CalculateQueuePosition(i, queueStartPosition);
        }
    }

    private IEnumerator WaitAndSpawnNewClient()
    {
        yield return new WaitForSeconds(timeBetweenClients);

        Client newClient = GetClientFromPool();
        newClient.AddRandomProductsToCart();
        newClient.CalculateCost();
        newClient.NpcController.isInDequeue = false;
        newClient.transform.position = CalculateQueuePosition(_clientQueue.Count - 1, queueStartPosition);
    }
}