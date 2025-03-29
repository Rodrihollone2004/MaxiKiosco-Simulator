using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private bool _isProcessing = false;
    private Coroutine _currentMovement;

    public TMP_Text PayText => payText;
    public Queue<Client> ClientQueue => _clientQueue;

    private void Start()
    {
        InitializePool(5);
        payText.text = "";
        SpawnInitialClients(3);
    }

    // pre-instancia clientes para reutilizar
    private void InitializePool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            CreateNewClientInPool();
        }
    }

    // instanciar un nuevo cliente y añadirlo a la pool
    private GameObject CreateNewClientInPool()
    {
        GameObject client = Instantiate(clientPrefab, queueStartPosition.position, Quaternion.identity);
        client.SetActive(false);
        _clientPool.Add(client);
        return client;
    }

    // obtiene un cliente disponible del pool, si no hay disponible crea uno nuevo, lo activa y devuelve su componente client
    private Client GetClientFromPool()
    {
        GameObject client = _clientPool.Find(c => !c.activeInHierarchy) ?? CreateNewClientInPool();
        client.SetActive(true);
        return client.GetComponent<Client>();
    }

    // recicla un cliente para reutilizarlo
    private void ReturnClientToPool(Client client)
    {
        client.gameObject.SetActive(false);
        client.transform.position = queueStartPosition.position;
    }

    // genera los clientes iniciales cuando arranca el juego
    private void SpawnInitialClients(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnNewClient();
        }
    }

    // añade un nuevo cliente a la cola
    private void SpawnNewClient()
    {
        Client newClient = GetClientFromPool();
        Vector3 spawnPosition = GetLastClientPosition() + new Vector3(distanceBetweenClients, 0, 0);
        newClient.transform.position = spawnPosition;

        _clientQueue.Enqueue(newClient);

        if (_clientQueue.Count == 1)
        {
            if (_currentMovement != null) StopCoroutine(_currentMovement);
            _currentMovement = StartCoroutine(MoveClientToPayPosition(newClient));
        }

        OnQueueUpdated?.Invoke();
    }

    // obtiene la posicion del ultimo cliente de la cola, esto para que pueda spawnear atras
    private Vector3 GetLastClientPosition()
    {
        if (_clientQueue.Count == 0)
        {
            return queueStartPosition.position;
        }
        else
        {
            Client[] clientsArray = _clientQueue.ToArray();
            return clientsArray[clientsArray.Length - 1].transform.position;
        }
    }

    // calcula la posicion en la cola basada en el index
    private Vector3 CalculateQueuePosition(int queueIndex)
    {
        return queueStartPosition.position + new Vector3(queueIndex * distanceBetweenClients, 0, 0);
    }

    // elimina al cliente actual, el cual ya fue atendido y actualiza la cola
    public void RemoveClient()
    {
        if (_clientQueue.Count == 0) return;

        Client client = _clientQueue.Dequeue();
        OnClientServed?.Invoke(client);
        ReturnClientToPool(client);

        UpdateQueuePositions();

        StartCoroutine(WaitAndSpawnNewClient());
        OnQueueUpdated?.Invoke();
        _isProcessing = false;
    }

    // reorganiza la cola despues de atender a un cliente
    private void UpdateQueuePositions()
    {
        int index = 0;
        foreach (Client client in _clientQueue)
        {
            Vector3 targetPos = CalculateQueuePosition(index);
            StartCoroutine(MoveClientToPosition(client.gameObject, targetPos));
            index++;
        }

        if (_clientQueue.Count > 0)
        {
            if (_currentMovement != null) StopCoroutine(_currentMovement);
            _currentMovement = StartCoroutine(MoveClientToPayPosition(_clientQueue.Peek()));
        }
    }

    // mueve el cliente hacia la proxima posicion de la cola (no de pago)
    private IEnumerator MoveClientToPosition(GameObject client, Vector3 targetPosition)
    {
        while (Vector3.Distance(client.transform.position, targetPosition) > 0.1f)
        {
            client.transform.position = Vector3.MoveTowards(
                client.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // mueve un cliente hacia la posicion de pagar
    private IEnumerator MoveClientToPayPosition(Client client)
    {
        while (Vector3.Distance(client.transform.position, payPosition.position) > 0.1f)
        {
            client.transform.position = Vector3.MoveTowards(
                client.transform.position,
                payPosition.position,
                moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // espera un tiempo y genera un nuevo cliente
    private IEnumerator WaitAndSpawnNewClient()
    {
        yield return new WaitForSeconds(timeBetweenClients);
        SpawnNewClient();
    }

    // intenta procesar el pago del primer cliente
    public void ProcessPayment()
    {
        if (_clientQueue.Count == 0 || _isProcessing) return;

        Client client = _clientQueue.Peek();
        float amount = client.CalculateCartTotal();

        var paymentResult = client.TryMakePayment(amount);

        if (paymentResult.success)
        {
            string paymentText = $"Pago: ${amount:F2}\n";
            foreach (var bill in paymentResult.paymentUsed)
            {
                paymentText += $"{bill.Value}x ${bill.Key}  ";
            }

            payText.text = paymentText;
            StartCoroutine(ClearPaymentTextAfterDelay(2f));

            client.CompletePurchase(paymentResult.paymentUsed);
            RemoveClient();
        }
    }

    // limpia el texto de pago despues de que un cliente sea atendido
    private IEnumerator ClearPaymentTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        payText.text = "";
    }
}