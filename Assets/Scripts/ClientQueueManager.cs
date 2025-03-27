using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClientQueueManager : MonoBehaviour
{
    public Queue<GameObject> clientQueue = new Queue<GameObject>();
    public Transform payPosition;
    public float moveSpeed = 3f;
    [SerializeField] private GameObject clientPrefab;
    [SerializeField] private TMP_Text payText;

    public TMP_Text PayText { get => payText; set => payText = value; }

    void Start()
    {
        payText.text = "";
        for (int i = 0; i < 3; i++) 
        {
            GameObject newClient = Instantiate(clientPrefab, transform.position, Quaternion.identity);
            AddClient(newClient);
        }
    }
    public void AddClient(GameObject client)
    {
        Vector3 spawnPosition = transform.position; 

        if (clientQueue.Count > 0)
        {
            GameObject lastClient = clientQueue.ToArray()[clientQueue.Count - 1];
            spawnPosition = lastClient.transform.position - new Vector3(-2f, 0, 0);
        }

        client.transform.position = spawnPosition;
        clientQueue.Enqueue(client);

        if (clientQueue.Count == 1)
        {
            MoveNextClient();
        }
    }

    public void RemoveClient()
    {
        if (clientQueue.Count > 0)
        {
            GameObject client = clientQueue.Dequeue();
            Destroy(client);

            StartCoroutine(WaitAndSpawnNewClient());
            StartCoroutine(WaitAndMoveNext());
        }
    }

    private IEnumerator WaitAndSpawnNewClient()
    {
        yield return new WaitForSeconds(2f);

        GameObject newClient = Instantiate(clientPrefab, transform.position, Quaternion.identity);
        AddClient(newClient);
    }

    private IEnumerator WaitAndMoveNext()
    {
        yield return null;
        MoveNextClient();
    }

    private void MoveNextClient()
    {
        if (clientQueue.Count > 0)
        {
            GameObject nextClient = clientQueue.Peek();
            StartCoroutine(MoveClientToPosition(nextClient, payPosition.position));
        }
    }

    private IEnumerator MoveClientToPosition(GameObject client, Vector3 targetPosition)
    {
        while (client != null && Vector3.Distance(client.transform.position, targetPosition) > 0.1f)
        {
            client.transform.position = Vector3.MoveTowards(client.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}