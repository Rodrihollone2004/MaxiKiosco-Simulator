using UnityEngine;

public class UpgradeInteractable : MonoBehaviour, IInteractable
{
    public Upgrade UpgradeData;

    public bool ShowNameOnHighlight => true;
    public bool IsPlaced;

    public bool CanBePickedUp => false;

    public void Interact()
    {
    }

    public void Highlight()
    {
    }

    public void Unhighlight()
    {
    }
}
