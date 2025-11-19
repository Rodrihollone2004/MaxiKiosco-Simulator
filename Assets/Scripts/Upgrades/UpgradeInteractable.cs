using UnityEngine;

public class UpgradeInteractable : MonoBehaviour, IInteractable
{
    public Upgrade UpgradeData;

    public bool ShowNameOnHighlight => true;
    public bool IsPlaced;

    public bool CanBePickedUp => false;

    public void Interact()
    {
        if (UpgradeData.IsUpgradeChange)
            ProductUIManager.Instance.ShowInfoUpgrade(UpgradeData.Name, UpgradeData.InfoUpgrade);

        if(TryGetComponent<Radio>(out Radio radio))
            radio.StartRadio();
    }

    public void CheckParent(GameObject finalObj, UpgradeInteractable product)
    {
        if (product.UpgradeData.PlaceZone != "Cafetera")
            return;

        PlacementZoneProducts zone = null;
        RaycastHit[] hits = Physics.RaycastAll(finalObj.transform.position, Vector3.down, 8f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.TryGetComponent(out PlacementZoneProducts z))
            {
                zone = z;
            }
        }

        if (zone != null)
        {
            finalObj.transform.SetParent(zone.transform);
        }
    }

    public void UpgradeMoneyEarned(int earnUpgrade)
    {
        PlayerEconomy playerEconomy = FindObjectOfType<PlayerEconomy>();
        playerEconomy.ReceivePayment(earnUpgrade);
    }

    public void Highlight()
    {
    }

    public void Unhighlight()
    {
    }
}
