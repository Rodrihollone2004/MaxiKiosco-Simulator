using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProductPlaceManager : MonoBehaviour, IInteractable
{
    [SerializeField] private Color _highlightColor = Color.red;
    [SerializeField] private float _highlightWidth = 1.03f;
    [SerializeField] private GameObject buildPrefab;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
    GameObject currentPreview;
    GameObject containerPrefab;
    PreviewValidator previewValidator;

    public bool CanBePickedUp => true;
    public GameObject CurrentPreview { get => currentPreview; set => currentPreview = value; }
    public PlacementZoneProducts[] AllZones { get; private set; }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
        AllZones = new PlacementZoneProducts[0];
    }

    public void Interact()
    {
        if (currentPreview != null)
            Destroy(currentPreview);

        Array.Clear(AllZones, 0, AllZones.Length);
        AllZones = FindObjectsOfType<PlacementZoneProducts>();

        containerPrefab = gameObject;

        buildPrefab = containerPrefab.transform.GetChild(0).GameObject();
        currentPreview = Instantiate(buildPrefab, Vector3.zero, Quaternion.identity);
        currentPreview.SetActive(true);
        SetPreviewColor(currentPreview, new Color(0f, 1f, 0f, 0.5f));

        previewValidator = currentPreview.AddComponent<PreviewValidator>();
        previewValidator.Initialize(new Color(0f, 1f, 0f, 0.5f), Color.red);

        Collider col = currentPreview.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        ProductInteractable productInteractable = currentPreview.GetComponent<ProductInteractable>();
        string productPlaceZone = productInteractable.ProductData.PlaceZone;

        foreach (PlacementZoneProducts zone in AllZones)
            zone.ShowVisual(productPlaceZone);
    }

    public void PlaceProduct()
    {
        if (previewValidator != null && previewValidator.IsValidPlacement)
        {
            GameObject finalObj = Instantiate(buildPrefab, currentPreview.transform.position, currentPreview.transform.rotation);
            finalObj.SetActive(true);
            SetPreviewColor(finalObj, Color.blue);
            
            ProductInteractable product = finalObj.GetComponent<ProductInteractable>();
            product.IsPlaced = true;

            Collider col = finalObj.GetComponent<Collider>();
            if (col != null) col.enabled = true;

            PreviewObject moveObject = finalObj.GetComponent<PreviewObject>();
            moveObject.enabled = false;

            PlayerInteraction playerInteraction = FindObjectOfType<PlayerInteraction>();
            playerInteraction.DropHintUI.SetActive(false);

            foreach (PlacementZoneProducts zone in AllZones)
                zone.HideVisual();

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
