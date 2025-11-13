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
    }

    public void Highlight()
    {
    }

    public void Unhighlight()
    {
    }
}
