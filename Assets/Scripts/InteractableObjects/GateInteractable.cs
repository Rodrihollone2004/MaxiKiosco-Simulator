using UnityEngine;

public class GateInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Color highlightColor = Color.green;
    [SerializeField] private float highlightWidth = 1.03f;
    [SerializeField] private float openHeight = 3f;
    [SerializeField] private float openSpeed = 2f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isMoving = false;
    private bool isOpen = false;
    private bool isClosing = false;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    public bool CanBePickedUp => false;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }
    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;
    }

    public void Interact()
    {
        if(TutorialContent.Instance.CurrentIndexGuide == 1)
            return;

        if (!isMoving)
        {
            if (!isOpen)
            {
                TutorialContent.Instance.CompleteStep(2);

                isMoving = true;
                isClosing = false;
            }
            else
            {
                isMoving = true;
                isClosing = true;
            }
        }
    }
    private void Update()
    {
        if (isMoving)
        {
            if (!isClosing)
            {
                transform.position = Vector3.MoveTowards(transform.position, openPosition, openSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, openPosition) < 0.01f)
                {
                    transform.position = openPosition;
                    isMoving = false;
                    isOpen = true;
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, closedPosition, openSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, closedPosition) < 0.01f)
                {
                    transform.position = closedPosition;
                    isMoving = false;
                    isOpen = false;
                    isClosing = false;
                }
            }
        }
    }

    public void Highlight()
    {
        if (!_renderer) return;
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_Color", highlightColor);
        _propBlock.SetFloat("_Scale", highlightWidth);
        _renderer.SetPropertyBlock(_propBlock);
    }

    public void Unhighlight()
    {
        if (!_renderer) return;
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat("_Scale", 0f);
        _renderer.SetPropertyBlock(_propBlock);
    }
}
