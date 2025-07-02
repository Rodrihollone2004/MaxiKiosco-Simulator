using System.Collections.Generic;
using UnityEngine;

public class PlacementZoneProducts : MonoBehaviour
{
    [SerializeField] List<GameObject> visualPlacement;

    private void Awake()
    {
        if (visualPlacement.Count > 0)
            foreach (GameObject visual in visualPlacement)
                visual.SetActive(false);
    }

    public void ShowVisual()
    {
        if (visualPlacement.Count > 0)
            foreach (GameObject visual in visualPlacement)
                visual.SetActive(true);
    }

    public void HideVisual()
    {
        if (visualPlacement.Count > 0)
            foreach (GameObject visual in visualPlacement)
                visual.SetActive(false);
    }
}
