using UnityEngine;

public class PlacementZoneProducts : MonoBehaviour
{
    [SerializeField] private GameObject visual;

    private void Awake()
    {
        if (visual != null)
            visual.SetActive(false);
    }

    public void ShowVisual()
    {
        if (visual != null)
            visual.SetActive(true);
    }

    public void HideVisual()
    {
        if (visual != null)
            visual.SetActive(false);
    }
}
