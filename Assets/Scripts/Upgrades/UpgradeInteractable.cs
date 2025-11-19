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
