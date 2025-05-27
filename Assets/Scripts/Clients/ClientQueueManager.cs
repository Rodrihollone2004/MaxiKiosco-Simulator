using System;
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

    [Header("References UI")]
    [SerializeField] private TMP_Text payText;

    [Header("Events")]
    public UnityEvent<Client> OnClientServed;
    public UnityEvent OnQueueUpdated;

    private Queue<Client> _clientQueue = new Queue<Client>();
    private List<GameObject> _clientPool = new List<GameObject>();

    public Queue<Client> ClientQueue { get => _clientQueue; set => _clientQueue = value; }
    public TMP_Text PayText { get => payText; set => payText = value; }

    private void Start()
    {
        InitializePool(5);
        SpawnInitialClients(1);
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
        return client.GetComponent<Client>();
    }

    public void RemoveClient()
    {
        if (_clientQueue.Count == 0) return;

        Client client = _clientQueue.Dequeue();
        OnClientServed?.Invoke(client);
        ReturnClientToPool(client);

        UpdateQueuePositions();

        StartCoroutine(WaitAndSpawnNewClient());
        OnQueueUpdated?.Invoke();
    }

    private void UpdateQueuePositions()
    {
        int index = 0;
        foreach (Client client in _clientQueue)
        {
            Vector3 targetPos = CalculateQueuePosition(index);
            StartCoroutine(MoveClientToPosition(client.gameObject, targetPos));
            index++;
        }
    }

    private Vector3 CalculateQueuePosition(int index)
    {
        return queueStartPosition.position + Vector3.back * distanceBetweenClients * index;
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

    private void ReturnClientToPool(Client client)
    {
        client.gameObject.SetActive(false);
        client.transform.position = queueStartPosition.position;
    }

    private void SpawnInitialClients(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Client client = GetClientFromPool();
            _clientQueue.Enqueue(client);
            client.transform.position = CalculateQueuePosition(i);
        }
    }

    private IEnumerator WaitAndSpawnNewClient()
    {
        yield return new WaitForSeconds(timeBetweenClients);
        Client newClient = GetClientFromPool();
        _clientQueue.Enqueue(newClient);
        newClient.transform.position = CalculateQueuePosition(_clientQueue.Count - 1);
    }
}