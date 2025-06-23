using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementZone : MonoBehaviour
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
