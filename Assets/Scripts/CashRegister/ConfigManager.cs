using System.Windows.Forms;
using TMPro;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TMP_InputField storeNameInput;
    [SerializeField] private TMP_Text storeNameDisplay;

    void Start()
    {

        storeNameInput.onValueChanged.AddListener(UpdateStoreName);
    }

    private void UpdateStoreName(string newName)
    {
        storeNameDisplay.text = newName;
    }

    public void ConfirmInputField()
    {
        string text = storeNameInput.text;

        storeNameInput.onEndEdit.Invoke(text);
    }
}
