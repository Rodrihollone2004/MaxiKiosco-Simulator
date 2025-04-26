using UnityEngine;

[System.Serializable]
public class Item : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;
}
