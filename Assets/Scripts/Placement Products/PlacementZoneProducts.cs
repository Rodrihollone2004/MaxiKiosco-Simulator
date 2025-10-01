using System.Collections.Generic;
using UnityEngine;

public class PlacementZoneProducts : MonoBehaviour
{
    [SerializeField] List<Zones> listZones;

    public List<Zones> ListZones { get => listZones; private set => listZones = value; }

    private void Awake()
    {
        if (listZones.Count > 0)
            foreach (Zones zones in listZones)
                foreach (GameObject visual in zones.visualPlacements)
                    visual.SetActive(false);
    }

    public void ShowVisual(string category)
    {
        foreach (Zones zones in listZones)
            if (category == zones.name && zones.visualPlacements.Count > 0)
                foreach (GameObject visual in zones.visualPlacements)
                    visual.SetActive(true);
    }

    public void HideVisual()
    {
        foreach (Zones zones in listZones)
            if (zones.visualPlacements.Count > 0)
                foreach (GameObject visual in zones.visualPlacements)
                    visual.SetActive(false);
    }
}

[System.Serializable]
public class Zones
{
    public string name;
    public List<GameObject> visualPlacements;
}
