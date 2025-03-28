using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ClientQueueManager : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private GameObject clientPrefab;
    [SerializeField] private Transform payPosition;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int initialPoolSize = 5;
    [SerializeField] private float timeBetweenClients = 2f;

    [Header("Referencias UI")]
    [SerializeField] private TMP_Text payText;

    [Header("Eventos")]
    public UnityEvent<Client> OnClientServed;
    public UnityEvent OnQueueUpdated;

    private Queue<GameObject> _clientQueue = new Queue<GameObject>();
    private Queue<GameObject> _clientPool = new Queue<GameObject>();
    private bool _isMovingClient = false;
    private Dictionary<GameObject, Rigidbody> _clientRigidbodies = new Dictionary<GameObject, Rigidbody>();

    public TMP_Text PayText { get => payText; set => payText = value; }
    public Queue<GameObject> ClientQueue { get => _clientQueue; set => _clientQueue = value; }

    private void Awake()
    {
        InitializePool(initialPoolSize);
    }

    private void Start()
    {
        payText.text = "";
        SpawnInitialClients(3);
    }

    private void InitializePool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject client = Instantiate(clientPrefab, transform.position, Quaternion.identity);
            client.SetActive(false);
            _clientRigidbodies[client] = client.GetComponent<Rigidbody>();
            _clientPool.Enqueue(client);
        }
    }

    private GameObject GetClientFromPool()
    {
        if (_clientPool.Count > 0)
        {
            GameObject client = _clientPool.Dequeue();
            client.SetActive(true);
            return client;
        }
        else
        {
            GameObject newClient = Instantiate(clientPrefab, transform.position, Quaternion.identity);
            _clientRigidbodies[newClient] = newClient.GetComponent<Rigidbody>();
            return newClient;
        }
    }

    private void ReturnClientToPool(GameObject client)
    {
        client.SetActive(false);
        _clientPool.Enqueue(client);
    }

    private void SpawnInitialClients(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnNewClient();
        }
    }

    private void SpawnNewClient()
    {
        GameObject newClient = GetClientFromPool();
        Vector3 spawnPosition = GetSpawnPosition();

        newClient.transform.position = spawnPosition;
        _clientQueue.Enqueue(newClient);

        if (_clientQueue.Count == 1 && !_isMovingClient)
        {
            MoveNextClientToPay();
        }

        OnQueueUpdated?.Invoke();
    }

    private Vector3 GetSpawnPosition()
    {
        if (_clientQueue.Count == 0)
        {
            return transform.position;
        }
        else
        {
            GameObject lastClient = _clientQueue.ToArray()[_clientQueue.Count - 1];
            return lastClient.transform.position - new Vector3(-2f, 0, 0);
        }
    }

    public void RemoveClient()
    {
        if (_clientQueue.Count == 0) return;

        GameObject client = _clientQueue.Dequeue();
        Client clientData = client.GetComponent<Client>();

        OnClientServed?.Invoke(clientData);
        ReturnClientToPool(client);

        StartCoroutine(WaitAndSpawnNewClient());
        MoveNextClientToPay();
        OnQueueUpdated?.Invoke();
    }

    private void MoveNextClientToPay()
    {
        if (_clientQueue.Count == 0 || _isMovingClient) return;

        GameObject nextClient = _clientQueue.Peek();
        StartCoroutine(MoveClientToPosition(nextClient, payPosition.position));
    }

    private IEnumerator MoveClientToPosition(GameObject client, Vector3 targetPosition)
    {
        _isMovingClient = true;

        Rigidbody rb = _clientRigidbodies[client];

        while (Vector3.Distance(client.transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - client.transform.position).normalized;
            rb.MovePosition(client.transform.position + direction * moveSpeed * Time.deltaTime);
            yield return null;
        }

        _isMovingClient = false;
    }

    private IEnumerator WaitAndSpawnNewClient()
    {
        yield return new WaitForSeconds(timeBetweenClients);
        SpawnNewClient();
    }

public void TryProcessPayment(out float amountPaid)
{
    amountPaid = 0f;

    if (_clientQueue.Count > 0)
    {
        Client client = _clientQueue.Peek().GetComponent<Client>();
        amountPaid = client.CalculateCartTotal();

        Dictionary<int, int> paymentDetails = client.GetPaymentDetails(amountPaid);

        string paymentText = $"Pago: ${amountPaid:F2}\n";

        foreach (var bill in paymentDetails)
        {
            paymentText += $"{bill.Value}x ${bill.Key}  ";
        }

        payText.text = paymentText;

        RemoveClient();
    }
}
}