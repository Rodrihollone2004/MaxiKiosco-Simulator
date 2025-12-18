using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialContent : MonoBehaviour
{
    public static TutorialContent Instance;

    public bool IsComplete { get; set; }
    public bool IsStart { get; set; }

    private string[] textsToGuide;
    private string[] textsToTitles;

    public int markIndex;
    [SerializeField] private int currentIndexGuide = 0;
    public int CurrentIndexGuide => currentIndexGuide;

    private int indexTitles = 0;

    public TMP_Text TasksGuide { get => tasksGuide; set => tasksGuide = value; }

    [SerializeField] private TMP_Text guideText;
    [SerializeField] private TMP_Text guideInWorld;
    [SerializeField] private TMP_Text guideInPcText;
    [SerializeField] private TMP_Text guideInPcTitles;
    [SerializeField] private TMP_Text tasksGuide;
    [SerializeField] private GameObject tasksUI;
    [SerializeField] private GameObject buttonQuitInfo;
    [SerializeField] private GameObject buttonNext;
    [SerializeField] private Toggle toggleTasks;
    [SerializeField] private TutorialGuider guider;
    [SerializeField] private List<GameObject> marks = new List<GameObject>();

    [Header("Videos Settings")]
    [SerializeField] private RawImage rawImage;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private List<VideoClip> videosTuto;

    [Header("Tuto First Actions")]
    [field: SerializeField] public GameObject RepickImage { get; private set; }
    [field: SerializeField] public GameObject F3StartDay { get; private set; }
    [field: SerializeField] public GameObject ThiefImage { get; private set; }
    [field: SerializeField] public GameObject TrashImage { get; private set; }
    [field: SerializeField] public GameObject CartoneroImage { get; private set; }

    private Animator animatorText;
    private bool lockedClick = false;
    public bool isInInfo = false;
    public bool IsFirstPlced { get; set; }
    public bool IsStartDay { get; set; }
    public bool IsFirstThief { get; set; }
    public bool IsFirstTrash { get; set; }
    public bool IsFirstCartonero { get; set; }


    public Action onStartButton;

    private void Awake()
    {
        IsFirstPlced = false;
        IsStartDay = false;
        IsFirstThief = false;
        IsFirstTrash = false;
        IsFirstCartonero = false;

        buttonNext.SetActive(true);
        animatorText = tasksGuide.GetComponent<Animator>();
        toggleTasks.isOn = false;
        buttonQuitInfo.SetActive(false);
        IsStart = false;

        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        textsToTitles = new string[]
        {
            "CAJA",
            "STOCK",
            "PRECIOS",
            "UPGRADES",
            "FONDOS",
            "CUSTOMIZACIÓN"
        };

        textsToGuide = new string[]
            {
            "¡Hola! ¡Te doy la bienvenida a tu primer trabajo!\r\nPor el momento necesito que cubras algunos días de la semana mientras termino de hacer unos trámites.\r\n¡Vamos! Te explico lo que tenés que hacer.\r\n Haceme click para seguir",
            "Todos los días vas a empezar acá.\r\nEse es el Kiosco.\r\nHacé clic sobre la persiana para subirla.",
            "Subir la térmica",
            "Revisar la PC",
            "Leer la información",
            "Cuando llegue un cliente puede abonar en efectivo o con QR\r\nSi paga en efectivo tenés que darle el vuelto.",
            "En esta se hacen los pedidos y se controla el stock.\r\nNavegá por las categorías y cargá el carrito.\r\nSimple. Elegís, pagás y te llega al depósito.",
            "Cuando comprás mercadería va a llegar con su  precio al costo. \r\nModificá los precios para poder obtener ganancias.",
            "Acá vas a poder comprar objetos decorativos para adornar el maxikiosco y mejoras que te van a ayudar en la progresión.",
            "Acá vas a poder cambiar los fondos de pantalla. Podes usar los predeterminados o subir la imagen que vos quieras.",
            "Por último, esta te va a permitir customizar el nombre de la tienda y el color interior del maxikiosco.",
            "Compra una heladera",
            "Hagamos un pedido",
            "Busca los pedidos",
            "Coloca el producto/heladera",
            "Abrir el kiosco",
            ""
            };

        tasksGuide.text = "Hablar con el jefe";
        guideText = guideInWorld;
    }

    private void Start()
    {
        NextTextToGuide();
    }

    public void NextTextToGuide()
    {
        if (currentIndexGuide >= 2 && guideText != tasksGuide)
        {
            guideText = tasksGuide;
        }
        else if (CurrentIndexGuide >= 5 && CurrentIndexGuide < 14)
        {
            if (isInInfo)
                tasksUI.SetActive(false);
            ChangeMarkIcon();
        }
        else if (currentIndexGuide == 15)
        {
            IsStart = true;
            onStartButton.Invoke();
        }
        else if (currentIndexGuide == 16)
            tasksUI.SetActive(false);

        if (currentIndexGuide < 16 && !isInInfo)
        {
            guideText.text = textsToGuide[currentIndexGuide];
            currentIndexGuide++;
            toggleTasks.isOn = false;
        }
    }

    public void CompleteStepCloseInfo()
    {
        isInInfo = false;
        CompleteStep(11);

        if (currentIndexGuide == 11)
            tasksUI.SetActive(true);
    }

    public void CloseInfo()
    {
        Cursor.visible = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void InfoScreen()
    {
        if (indexTitles < textsToTitles.Length && isInInfo)
        {
            videoPlayer.clip = videosTuto[indexTitles];

            guideInPcTitles.text = textsToTitles[indexTitles];
            indexTitles++;

            guideInPcText.text = textsToGuide[currentIndexGuide];
            currentIndexGuide++;

            NextTextToGuide();

            lockedClick = false;
        }

        if (indexTitles >= textsToTitles.Length)
        {
            buttonQuitInfo.SetActive(true);
            buttonNext.SetActive(false);
        }

    }

    public void DesactivateText()
    {
        if (guider != null)
        {
            guider.CanvasTuto.SetActive(false);
            guider.BackToStart();
        }
    }

    public void CompleteStep(int stepIndex)
    {
        if (currentIndexGuide == stepIndex)
        {
            toggleTasks.isOn = true;
            guider.ChangeTutoNode();
            animatorText.SetTrigger("CompleteTask");
            StartCoroutine(WaitForAnimation());
        }
    }

    public void CompleteStepButton()
    {
        lockedClick = true;
        InfoScreen();
    }

    private IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(1f);

        if (currentIndexGuide == 1)
            TutorialContent.Instance.TasksGuide.text = "Levantar la persiana";

        animatorText.SetTrigger("NormalTask");
        NextTextToGuide();
    }

    public void ChangeMarkIcon()
    {
        for (int i = 0; i < marks.Count; i++)
            if (markIndex == i)
            {
                marks[i].SetActive(true);
            }
            else
                marks[i].SetActive(false);

        markIndex++;
    }
}
