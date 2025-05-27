using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [field: SerializeField] public Node cameFrom { get; set; }
    [field: SerializeField] public List<Node> connections { get; private set; }

    public float gScore;
    public float hScore;

    public float FScore()
    {
        return gScore + hScore;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (connections.Count > 0)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                Gizmos.DrawLine(transform.position, connections[i].transform.position);
            }
        }
    }
}
