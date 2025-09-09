using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProductPriceInput : MonoBehaviour
{
    [SerializeField] private TMP_Text priceText;
    private Product _productData;
    
    [SerializeField] private float holdDelay = 0.2f; // Tiempo entre cada suma/resta
    private float holdTimer = 0f;

    private bool isHolding = false;

    private enum ActionType { None, Add, Subtract }
    private ActionType currentAction = ActionType.None;

    private int currentAmount = 0;

    public void Initialize(Product data)
    {
        _productData = data;
        UpdatePriceText();
    }

    private void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdDelay)
            {
                switch (currentAction)
                {
                    case ActionType.Add:
                        _productData.Price += currentAmount;
                        break;
                    case ActionType.Subtract:
                        _productData.Price -= currentAmount;
                        break;
                }

                UpdatePriceText();
                holdTimer = 0f; 
            }
        }
    }

    private void UpdatePriceText()
    {
        priceText.text = $"{_productData.Price}";
    }

    // Llamá estos métodos desde el EventTrigger, pasando el valor
    public void StartAddingPrice(int amount)
    {
        isHolding = true;
        currentAmount = amount;
        currentAction = ActionType.Add;
    }

    public void StartSubtractingPrice(int amount)
    {
        isHolding = true;
        currentAmount = amount;
        currentAction = ActionType.Subtract;
    }

    public void StopButton()
    {
        isHolding = false;
        currentAction = ActionType.None;
        holdTimer = 0f; 
    }
}