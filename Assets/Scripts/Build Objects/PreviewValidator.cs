using UnityEngine;

public class PreviewValidator : MonoBehaviour
{
    [SerializeField] private LayerMask blockedLayers;

    private Color validColor;
    private Color invalidColor;
    private MeshRenderer[] meshRenderer;
    private MaterialPropertyBlock block;

    public bool IsValidPlacement { get; private set; }

    private void Awake()
    {
        meshRenderer = gameObject.GetComponentsInChildren<MeshRenderer>();
        blockedLayers = ~(1 << LayerMask.NameToLayer("Water"));
    }

    public void Initialize(Color valid, Color invalid)
    {
        validColor = valid;
        invalidColor = invalid;
    }

    private void Update()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null)
        {
            IsValidPlacement = false;
            return;
        }

        Vector3 halfExtents = Vector3.Scale(box.size, transform.lossyScale) * 0.5f;
        Vector3 worldCenter = transform.TransformPoint(box.center);
        Quaternion rotation = transform.rotation;

        Collider[] hits = Physics.OverlapBox(worldCenter, halfExtents, rotation, blockedLayers, QueryTriggerInteraction.Collide);

        bool overlap = false;
        foreach (Collider hit in hits)
        {
            if (hit.gameObject != gameObject)
            {
                overlap = true;
                break;
            }
        }

        IsValidPlacement = !overlap;

        Color targetColor = IsValidPlacement ? validColor : invalidColor;
        block = new MaterialPropertyBlock();
        block.SetColor("_Color", targetColor);
        block.SetFloat("_Scale", 1.03f);

        foreach (MeshRenderer mesh in meshRenderer)
            mesh.SetPropertyBlock(block);
    }

    public void BackToNormal()
    {
        block.SetColor("_Color", Color.black);
        block.SetFloat("_Scale", 0f);

        foreach (MeshRenderer mesh in meshRenderer)
            mesh.SetPropertyBlock(block);
    }

    private void OnDrawGizmosSelected()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Vector3 halfExtents = Vector3.Scale(box.size, transform.lossyScale) * 0.5f;
        Vector3 worldCenter = transform.TransformPoint(box.center);

        Gizmos.color = Color.cyan;
        Gizmos.matrix = Matrix4x4.TRS(worldCenter, transform.rotation, halfExtents * 2);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
