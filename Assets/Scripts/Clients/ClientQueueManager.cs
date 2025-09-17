using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClientQueueManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private List<GameObject> clientPrefab;
    [SerializeField] private Transform payPosition;
    [SerializeField] private Transform queueStartPosition;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float minTimeBetweenClients = 60f;
    [SerializeField] private float maxTimeBetweenClients = 180f;
    [SerializeField] private float distanceBetweenClients = 1.5f;
    [SerializeField] private ClientTrashSpawner trashSpawner;
    [SerializeField] private DayNightCycle dayNightCycle;

    [Header("Daily Clients")]
    [SerializeField] private int maxClientsPerDay = 20;
    private int clientsSpawnedToday = 0;

    [Header("Events")]
    public UnityEvent<Client> OnClientServed;
    public UnityEvent OnQueueUpdated;

    public Queue<Client> _clientQueue = new Queue<Client>();
    private List<GameObject> _clientPool = new List<GameObject>();

    public Queue<Client> ClientQueue { get => _clientQueue; set => _clientQueue = value; }

    private Coroutine clientSpawnCoroutine;


    private int clientsServedToday = 0;
    private int moneyEarnedToday = 0;
    public int GetClientsServedToday() => clientsServedToday;
    public int GetMoneyEarnedToday() => moneyEarnedToday;

    private void Start()
    {
        InitializePool(5);

        clientsSpawnedToday = 0;
    }

    private void Update()
    {
        bool shouldSpawnClients = !dayNightCycle.IsPaused && clientsSpawnedToday < maxClientsPerDay && _clientQueue.Count < 3;

        if (shouldSpawnClients && clientSpawnCoroutine == null)
        {
            StartClientSpawning();
        }
        else if (!shouldSpawnClients && clientSpawnCoroutine != null)
        {
            StopClientSpawning();
        }
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
        GameObject client = Instantiate(clientPrefab[Random.Range(0, clientPrefab.Count)], queueStartPosition.position, Quaternion.identity);
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
        if (_clientQueue.Count == 0) yield break;

        Client client = _clientQueue.Dequeue();
        OnClientServed?.Invoke(client);

        clientsServedToday++;
        moneyEarnedToday += client.CalculateCartTotal();

        yield return new WaitUntil(() => client.NpcController.isInDequeue);

        if (Random.value < 0.25f && trashSpawner != null)
            trashSpawner.SpawnTrash();

        ReturnClientToPool(client);
        OnQueueUpdated?.Invoke();

        if (_clientQueue.Count < 3 && clientSpawnCoroutine == null && clientsSpawnedToday < maxClientsPerDay)
            StartClientSpawning();
    }

    public IEnumerator RemoveThief(Client client)
    {
        Client currentClient = client;
        OnClientServed?.Invoke(currentClient);

        clientsServedToday++;

        yield return new WaitUntil(() => currentClient.NpcController.isInDequeue);

        if (currentClient.WasHit)
            moneyEarnedToday += currentClient.CalculateCartTotal();

        if (Random.value < 0.25f && trashSpawner != null)
            trashSpawner.SpawnTrash();

        ReturnClientToPool(currentClient);
        OnQueueUpdated?.Invoke();

        if (_clientQueue.Count < 3 && clientSpawnCoroutine == null && clientsSpawnedToday < maxClientsPerDay)
            StartClientSpawning();
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
        return pos.position + Vector3.left * distanceBetweenClients * index;
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

    private IEnumerator ClientSpawnRoutine()
    {
        while (!dayNightCycle.IsPaused && clientsSpawnedToday < maxClientsPerDay)
        {
            float randomWaitTime = Random.Range(minTimeBetweenClients, maxTimeBetweenClients);
            Debug.Log($"Next client in: {randomWaitTime:F1} seconds.");
            yield return new WaitForSeconds(randomWaitTime);

            if (_clientQueue.Count >= 3)
            {
                clientSpawnCoroutine = null;
                yield break;
            }

            if (!dayNightCycle.IsPaused && clientsSpawnedToday < maxClientsPerDay)
            {
                Client newClient = GetClientFromPool();
                newClient.CheckThief();
                newClient.WasHit = false;
                newClient.NpcController.isInDequeue = false;
                newClient.transform.position = CalculateQueuePosition(_clientQueue.Count - 1, queueStartPosition);

                clientsSpawnedToday++;

                UpdateQueuePositions();
            }
        }
        clientSpawnCoroutine = null;
    }

    public void StartClientSpawning()
    {
        if (clientSpawnCoroutine == null)
        {
            clientSpawnCoroutine = StartCoroutine(ClientSpawnRoutine());
        }
    }

    public void StopClientSpawning()
    {
        if (clientSpawnCoroutine != null)
        {
            StopCoroutine(clientSpawnCoroutine);
            clientSpawnCoroutine = null;
        }
    }

    public void ResetDailyStats()
    {
        clientsServedToday = 0;
        moneyEarnedToday = 0;
        clientsSpawnedToday = 0;
    }
}