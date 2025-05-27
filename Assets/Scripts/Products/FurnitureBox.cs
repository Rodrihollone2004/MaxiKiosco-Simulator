using Unity.VisualScripting;
using UnityEngine;

public class FurnitureBox : MonoBehaviour, IInteractable
{
    [SerializeField] private Color _highlightColor = Color.red;
    [SerializeField] private float _highlightWidth = 1.03f;
    [SerializeField] private GameObject buildPrefab;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private Transform nodeContainer;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
    GameObject currentPreview;
    GameObject containerPrefab;
    PreviewValidator previewValidator;

    public bool CanBePickedUp => true;
    public GameObject CurrentPreview { get => currentPreview; set => currentPreview = value; }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    public void Interact()
    {
        if (currentPreview != null)
            Destroy(currentPreview);

        containerPrefab = gameObject;

        buildPrefab = containerPrefab.transform.GetChild(0).GameObject();
        currentPreview = Instantiate(buildPrefab, Vector3.zero, buildPrefab.transform.localRotation);
        currentPreview.SetActive(true);
        SetPreviewColor(currentPreview, new Color(0f, 1f, 0f, 0.5f));

        previewValidator = currentPreview.AddComponent<PreviewValidator>();
        previewValidator.Initialize(new Color(0f, 1f, 0f, 0.5f), Color.red);

        Collider col = currentPreview.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    public void PlaceFurniture()
    {
        if (previewValidator != null && previewValidator.IsValidPlacement)
        {
            GameObject finalObj = Instantiate(buildPrefab, currentPreview.transform.position, currentPreview.transform.rotation);
            finalObj.SetActive(true);
            SetPreviewColor(finalObj, Color.blue);

            Collider col = finalObj.GetComponent<Collider>();
            if (col != null) col.enabled = true;

            PreviewObject moveObject = finalObj.GetComponent<PreviewObject>();
            moveObject.enabled = false;

            Vector3 nodePos = finalObj.transform.position + finalObj.transform.up + finalObj.transform.forward;
            Instantiate(nodePrefab, nodePos, finalObj.transform.rotation, nodeContainer);
            AStarManager.instance.IsAllNodes = false;

            Destroy(currentPreview);
            Destroy(containerPrefab);
        }
        else
        {
            Debug.LogWarning("Posición inválida para colocar el objeto.");
        }
    }

    private void SetPreviewColor(GameObject obj, Color color)
    {
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetColor("_Color", color);
            renderer.SetPropertyBlock(block);
        }
    }

    public void Highlight()
    {
        if (!_renderer) return;
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_Color", _highlightColor);
        _propBlock.SetFloat("_Scale", _highlightWidth);
        _renderer.SetPropertyBlock(_propBlock);
    }

    public void Unhighlight()
    {
        if (!_renderer) return;
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat("_Scale", 0f);
        _renderer.SetPropertyBlock(_propBlock);
    }
}
