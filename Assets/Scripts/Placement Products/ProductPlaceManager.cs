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
    private int productsPlacedAmount;

    public bool CanBePickedUp => true;
    public bool IsEmpty { get; private set; } = false;
    public GameObject CurrentPreview { get => currentPreview; set => currentPreview = value; }
    public PlacementZoneProducts[] AllZones { get; private set; }

    public static List<ProductInteractable> productsPlaced = new List<ProductInteractable>();

    private void Awake()
    {
        productsPlacedAmount = 0;
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
        AllZones = new PlacementZoneProducts[0];
        IsEmpty = false;
    }

    public void Interact()
    {
        if (TutorialContent.Instance.CurrentIndexGuide < 12)
            return;

        TutorialContent.Instance.CompleteStep(12);

        if (currentPreview != null)
            Destroy(currentPreview);

        Array.Clear(AllZones, 0, AllZones.Length);
        AllZones = FindObjectsOfType<PlacementZoneProducts>();

        containerPrefab = gameObject;

        if (!IsEmpty)
        {
            buildPrefab = containerPrefab.transform.GetChild(0).GameObject();
            currentPreview = Instantiate(buildPrefab, Vector3.zero, Quaternion.identity);
            currentPreview.SetActive(true);
            SetPreviewColor(currentPreview, new Color(0f, 1f, 0f, 0.5f));

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

                productsPlacedAmount++;
                AnalyticsManager.Instance.ProductsPlaced(productsPlacedAmount);
            }
            else if (currentPreview.TryGetComponent<UpgradeInteractable>(out UpgradeInteractable upgrade))
            {
                UpgradeInteractable productInteractable = upgrade;
                string upgradePlaceZone = productInteractable.UpgradeData.PlaceZone;

                foreach (PlacementZoneProducts zone in AllZones)
                    zone.ShowVisual(upgradePlaceZone);
            }
        }
    }

    public void PlaceProduct()
    {
        PlayerInteraction playerInteraction = FindObjectOfType<PlayerInteraction>();

        if (previewValidator != null && previewValidator.IsValidPlacement)
        {
            GameObject finalObj = Instantiate(buildPrefab, currentPreview.transform.position, currentPreview.transform.rotation);
            finalObj.SetActive(true);
            SetPreviewColor(finalObj, Color.blue);

            Collider col = finalObj.GetComponent<Collider>();
            if (col != null) col.enabled = true;
            Collider[] colliders = finalObj.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders) collider.enabled = true;

            PreviewObject moveObject = finalObj.GetComponent<PreviewObject>();
            if (moveObject != null) moveObject.enabled = false;

            if (finalObj.TryGetComponent<ProductInteractable>(out ProductInteractable interactable))
            {
                ProductInteractable product = interactable;
                product.IsPlaced = true;
                foreach (StockController controllers in StoreUI.allStock)
                    controllers.PlaceProduct(product);

                product.CheckParent(finalObj, product);

                productsPlaced.Add(product);
            }
            else if (finalObj.TryGetComponent<UpgradeInteractable>(out UpgradeInteractable upgrade))
            {
                upgrade.IsPlaced = true;
                Light light = upgrade.GetComponentInChildren<Light>();
                if (light != null)
                {
                    playerInteraction.LightSwitch.totalLights.Add(light);
                    playerInteraction.LightSwitch.UpdateLights();
                }

                UpdateBills(upgrade.UpgradeData);
            }

            foreach (PlacementZoneProducts zone in AllZones) zone.HideVisual();

            Destroy(currentPreview);
            Destroy(buildPrefab);
            IsEmpty = true;

            if (playerInteraction != null)
            {
                playerInteraction.CheckUIText();
                if (playerInteraction.TryGetComponent(out AudioSource src) && playerInteraction.PlaceProduct_ != null)
                    src.PlayOneShot(playerInteraction.PlaceProduct_);
            }

            TutorialContent.Instance.CompleteStep(13);
        }
        else
        {
            if (playerInteraction.IsProductInHand && playerInteraction != null && playerInteraction.TryGetComponent(out AudioSource src))
                src.PlayOneShot(playerInteraction.ErrorSound);
        }
    }

    void UpdateBills(Upgrade upgrade)
    {
        if (upgrade.AmountMin1000 > 0)
            Wallet.AmountMin1000 += upgrade.AmountMin1000;

        if (upgrade.AmountMax1000 > 0)
            Wallet.AmountMax1000 += upgrade.AmountMax1000;

        if (upgrade.AmountMinOthers > 0)
            Wallet.AmountMinOthers += upgrade.AmountMinOthers;

        if (upgrade.AmountMaxOthers > 0)
            Wallet.AmountMaxOthers += upgrade.AmountMaxOthers;

        if (upgrade.MaxProductsToBuy > 0)
            Client.MaxProductsToBuy += upgrade.MaxProductsToBuy;

        if(upgrade.MaxAmountOfProductToBuy > 0)
            Client.MaxAmountToBuy += upgrade.MaxAmountOfProductToBuy;
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
