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

    public bool IsEmpty { get; private set; } = false;

    private GameObject currentProduct;

    public bool CanBePickedUp => true;
    public GameObject CurrentPreview { get => currentPreview; set => currentPreview = value; }
    public PlacementZoneProducts[] AllZones { get; private set; }

    public static List<ProductInteractable> productsPlaced = new List<ProductInteractable>();

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
        AllZones = new PlacementZoneProducts[0];

        if (transform.childCount > 0)
        {
            currentProduct = transform.GetChild(0).gameObject;
            buildPrefab = currentProduct;

            PreviewObject previewObj = currentProduct.GetComponent<PreviewObject>();
            if (previewObj != null)
            {
                previewObj.enabled = false;
            }

            IsEmpty = false;
        }
        else
        {
            IsEmpty = true;
        }
    }

    public void Interact()
    {
        if (IsEmpty)
        {
            TryPickUpProduct();
            return;
        }

        if (currentPreview != null && currentPreview.activeSelf)
        {
            return;
        }

        CreatePreviewImmediately();
    }

    private void TryPickUpProduct()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2f))
        {
            if (hit.collider.TryGetComponent<ProductInteractable>(out ProductInteractable product))
            {
                if (product.IsPlaced)
                {
                    product.IsPlaced = false;
                    if (productsPlaced.Contains(product))
                    {
                        productsPlaced.Remove(product);
                    }
                }

                currentProduct = hit.collider.gameObject;
                currentProduct.transform.SetParent(transform);
                currentProduct.transform.localPosition = Vector3.zero;
                currentProduct.transform.localRotation = Quaternion.identity;

                currentProduct.transform.localScale = Vector3.one * 0.5f;

                Rigidbody productRb = currentProduct.GetComponent<Rigidbody>();
                Collider productCol = currentProduct.GetComponent<Collider>();

                if (productRb != null) productRb.isKinematic = true;
                if (productCol != null) productCol.enabled = false;

                currentProduct.SetActive(false);

                PreviewObject previewObj = currentProduct.GetComponent<PreviewObject>();
                if (previewObj != null)
                {
                    previewObj.enabled = false;
                }

                ProductInteractable productInteractable = currentProduct.GetComponent<ProductInteractable>();
                if (productInteractable != null && productInteractable.OriginalPrefab != null)
                {
                    buildPrefab = productInteractable.OriginalPrefab;
                }
                else
                {
                    buildPrefab = currentProduct;
                }

                IsEmpty = false;
                CreatePreviewImmediately();
                UpdatePlayerInteractionReference();
            }
            else if (hit.collider.TryGetComponent<UpgradeInteractable>(out UpgradeInteractable upgrade))
            {
                if (upgrade.IsPlaced)
                {
                    upgrade.IsPlaced = false;
                }

                currentProduct = hit.collider.gameObject;
                currentProduct.transform.SetParent(transform);
                currentProduct.transform.localPosition = Vector3.zero;
                currentProduct.transform.localRotation = Quaternion.identity;

                currentProduct.transform.localScale = Vector3.one * 0.5f;

                Rigidbody upgradeRb = currentProduct.GetComponent<Rigidbody>();
                Collider upgradeCol = currentProduct.GetComponent<Collider>();

                if (upgradeRb != null) upgradeRb.isKinematic = true;
                if (upgradeCol != null) upgradeCol.enabled = false;

                currentProduct.SetActive(false);

                PreviewObject previewObj = currentProduct.GetComponent<PreviewObject>();
                if (previewObj != null)
                {
                    previewObj.enabled = false;
                }

                UpgradeInteractable upgradeInteractable = currentProduct.GetComponent<UpgradeInteractable>();
                if (upgradeInteractable != null)
                {
                    buildPrefab = currentProduct;
                }

                IsEmpty = false;
                CreatePreviewImmediately();
                UpdatePlayerInteractionReference();
            }
        }
    }

    private void CreatePreviewImmediately()
    {
        if (currentPreview != null)
            Destroy(currentPreview);

        Array.Clear(AllZones, 0, AllZones.Length);
        AllZones = FindObjectsOfType<PlacementZoneProducts>();

        containerPrefab = gameObject;

        if (buildPrefab == null)
        {
            return;
        }

        currentPreview = Instantiate(buildPrefab, Vector3.zero, Quaternion.identity);
        currentPreview.SetActive(true);
        currentPreview.transform.localScale = Vector3.one;
        SetPreviewColor(currentPreview, new Color(0f, 1f, 0f, 0.5f));

        PreviewObject previewObj = currentPreview.GetComponent<PreviewObject>();
        if (previewObj != null)
        {
            previewObj.enabled = true;
        }

        previewValidator = currentPreview.AddComponent<PreviewValidator>();
        previewValidator.Initialize(new Color(0f, 1f, 0f, 0.5f), Color.red);

        Collider col = currentPreview.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (currentPreview.TryGetComponent<ProductInteractable>(out ProductInteractable interactable))
        {
            ProductInteractable productInteractable = interactable;
            string productPlaceZone = productInteractable.ProductData.PlaceZone;

            foreach (PlacementZoneProducts zone in AllZones)
                zone.ShowVisual(productPlaceZone);
        }
        else if (currentPreview.TryGetComponent<UpgradeInteractable>(out UpgradeInteractable upgrade))
        {
            UpgradeInteractable upgradeInteractable = upgrade;
            string upgradePlaceZone = upgradeInteractable.UpgradeData.PlaceZone;

            foreach (PlacementZoneProducts zone in AllZones)
                zone.ShowVisual(upgradePlaceZone);
        }
    }

    private void UpdatePlayerInteractionReference()
    {
        PlayerInteraction playerInteraction = FindObjectOfType<PlayerInteraction>();
        if (playerInteraction != null)
        {
            playerInteraction.ForceUpdateHeldObjectReference(this);
        }
    }

    public void PlaceProduct()
    {
        if (previewValidator != null && previewValidator.IsValidPlacement)
        {
            GameObject finalObj = Instantiate(buildPrefab, currentPreview.transform.position, currentPreview.transform.rotation);
            finalObj.SetActive(true);
            finalObj.transform.localScale = Vector3.one;

            SetPreviewColor(finalObj, Color.blue);

            Collider col = finalObj.GetComponent<Collider>();
            if (col != null) col.enabled = true;

            PreviewObject moveObject = finalObj.GetComponent<PreviewObject>();
            if (moveObject != null)
            {
                moveObject.enabled = false;
            }

            PlayerInteraction playerInteraction = FindObjectOfType<PlayerInteraction>();
            if (playerInteraction != null)
            {
                playerInteraction.DropHintUI.SetActive(false);
                playerInteraction.UpdateHoldObjectUI();
            }

            foreach (PlacementZoneProducts zone in AllZones)
                zone.HideVisual();

            if (finalObj.TryGetComponent<ProductInteractable>(out ProductInteractable interactable))
            {
                ProductInteractable product = interactable;
                product.IsPlaced = true;

                foreach (StockController controllers in Stock.allStock)
                    controllers.PlaceProduct(product);

                productsPlaced.Add(product);
            }
            else if (finalObj.TryGetComponent<UpgradeInteractable>(out UpgradeInteractable upgrade))
            {
                UpgradeInteractable upgradeObj = upgrade;
                upgradeObj.IsPlaced = true;
            }

            Destroy(currentPreview);
            currentPreview = null;
            previewValidator = null;

            if (currentProduct != null)
            {
                Destroy(currentProduct);
                currentProduct = null;
            }
            IsEmpty = true;
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
