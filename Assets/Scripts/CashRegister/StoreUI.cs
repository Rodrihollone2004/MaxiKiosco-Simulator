using TMPro;
using UnityEngine;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private ProductDataBase database;
    [SerializeField] private PlayerEconomy playerEconomy;
    [SerializeField] private Transform productButtonContainer;
    [SerializeField] private GameObject productButtonPrefab;

    void Start()
    {
        PopulateStore();
    }

    void PopulateStore()
    {
        foreach (ProductCategory category in database.categories)
        {
            foreach (Product product in category.products)
            {
                GameObject buttonGO = Instantiate(productButtonPrefab, productButtonContainer);
                TMP_Text text = buttonGO.GetComponentInChildren<TMP_Text>();
                text.text = $"{product.Name} - ${product.Price}";

                buttonGO.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                {
                    playerEconomy.TryPurchase(0);
                });
            }
        }
    }
}