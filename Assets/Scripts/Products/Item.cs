using UnityEngine;

[System.Serializable]
public class Item : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private int _id;
    public string Name => _name;
    public int Id => _id;

#if UNITY_EDITOR
    public void SetId_Editor(int id)
    {
        _id = id;
    }
#endif
}
