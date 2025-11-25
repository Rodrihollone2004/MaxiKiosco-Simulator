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
    private int currentIndexGuide = 0;
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
    [SerializeField] private Toggle toggleTasks;
    [SerializeField] private TutorialGuider guider;
    [SerializeField] private List<GameObject> marks = new List<GameObject>();

    [Header("Videos Settings")]
    [SerializeField] private RawImage rawImage;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private List<VideoClip> videosTuto;

    private Animator animatorText;
    private bool lockedClick = false;
    public bool isInInfo = false;

    public Action onStartButton;

    private void Awake()
    {
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
            "¡Hola! ¡Te doy la bienvenida a tu primer trabajo!\r\nQuizás no sea mucho, pero antes que nada. ¡Todo suma!\r\nPor el momento necesito que cubras algunos días de la semana mientras termino de hacer unos trámites.\r\n¡Vamos! Te explico lo que tenés que hacer.\r\n Hacme click para seguir",
            "Todos los días vas a empezar acá.\r\nEse es el Kiosco.\r\nHacé clic sobre la persiana para subirla.",
            "Subir la térmica",
            "Revisar la PC",
            "Leer la información",
            "Cuando llegue un cliente tenés que usar esta.\r\nAcá te dice qué se van a llevar, cuánto sale, cómo paga y esas cosas.",
            "En esta se hacen los pedidos y se controla el stock.\r\nTenés categorías y tenés que elegir cuántos paquetes querés de cada cosa.\r\nSimple. Elegís y te llega al depósito.",
            "Esta es la más importante si no querés perder plata.\r\nAcá vas a tener que ir categoría en categoría viendo los productos que tenés a la venta y poniéndoles un precio. \r\nSi es muy caro, puede que algunos clientes no lo compren.",
            "En esta te voy a dejar que gastes algo de la plata del kiosco en hacer que se vea un poco mejor.\r\nSi el kiosco se ve bien va a entrar mas gente\r\nCompra una heladera como primer gasto",
            "Esta sección de acá te va a permitir cambiar el fondo de la pantalla para que te sea más ameno a la vista",
            "Por último, esta te va a permitir customizar el nombre de la tienda a gusto",
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
        else if (CurrentIndexGuide >= 5 && CurrentIndexGuide < 13)
        {
            if (isInInfo)
                tasksUI.SetActive(false);
            ChangeMarkIcon();
        }
        else if (currentIndexGuide == 14)
        {
            IsStart = true;
            onStartButton.Invoke();
        }
        else if (currentIndexGuide == 15)
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
            buttonQuitInfo.SetActive(true);

    }

    public void DesactivateText()
    {
        guider.CanvasTuto.SetActive(false);
        guider.BackToStart();
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
