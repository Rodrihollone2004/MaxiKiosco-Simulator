using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class TutorialContent : MonoBehaviour
{

    public static TutorialContent Instance;

    public bool IsComplete { get; set; }

    private string[] textsToGuide;

    public int markIndex;
    private int currentIndexGuide = 0;
    public int CurrentIndexGuide => currentIndexGuide;

    [SerializeField] private TMP_Text guideText;
    [SerializeField] private TMP_Text guideInPcText;
    [SerializeField] private TMP_Text guideInWorld;
    [SerializeField] private TutorialGuider guider;
    [SerializeField] private List<GameObject> marks = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        textsToGuide = new string[]
            {
            "¡Hola! ¡Te doy la bienvenida a tu primer trabajo!\r\nQuizás no sea mucho, pero antes que nada. ¡Todo suma!\r\nPor el momento necesito que cubras algunos días de la semana mientras termino de hacer unos trámites.\r\n¡Vamos! Te explico lo que tenés que hacer.\r\n Hacme click para seguir",
            "Todos los días vas a empezar acá.\r\nEse es el Kiosco.\r\nHacé clic sobre la persiana para subirla.",
            "¡Perfecto!\r\nSubí la térmica para prender las luces con el clic izquierdo.",
            "Ahora hace clic izquierdo en el monitor para gestionar la compu",
            "Cuando llegue un cliente tenés que usar esta.\r\nAcá te dice qué se van a llevar, cuánto sale, cómo paga y esas cosas.",
            "En esta se hacen los pedidos y se controla el stock.\r\nTenés categorías y tenés que elegir cuántos paquetes querés de cada cosa.\r\nSimple. Elegís y te llega al depósito.",
            "En esta te voy a dejar que gastes algo de la plata del kiosco en hacer que se vea un poco mejor.\r\nSi el kiosco se ve bien va a entrar mas gente.",
            "Esta es la más importante si no querés perder plata.\r\nAcá vas a tener que ir categoría en categoría viendo los productos que tenés a la venta y poniéndoles un precio. \r\nSi es muy caro, puede que algunos clientes no lo compren.",
            "Esta sección de acá te va a permitir cambiar el fondo de la pantalla para que te sea más ameno a la vista",
            "Por último, esta te va a permitir customizar el nombre de la tienda a gusto",
            "Hagamos un pedido.\r\nEntrá en la aplicación del Stock y compra una caja de todo lo que veas.",
            "¡Perfecto! Tenemos con qué empezar.\r\nAndá a buscarlos y los vas poniendo en la estantería.",
            "¡Como toda la vida!\r\nLas botellas a la heladera y el resto donde quede mas lindo!\r\nCuando las cajas esten vacías dejalas arriba del pallet, alguien va a venir a buscarlas después",
            "Cuando pasen los días y tu experiencia te haga un mejor kiosquero, vas a poder desbloquear otra variedad de mercadería.\r\nPrestá atención para no perder días sin poner productos nuevos.\r\n¡Empezá el día en la compu y buena suerte!\r\nHaceme click y me retiro",
            ""
            };

        guideText = guideInWorld;
    }

    private void Start()
    {
        NextTextToGuide();
    }

    public void NextTextToGuide()
    {
        if (CurrentIndexGuide >= 4 && CurrentIndexGuide < 12)
        {
            guideText = guideInPcText;
            guider.CanvasTuto.SetActive(false);
            ChangeMarkIcon();
        }
        else if (CurrentIndexGuide == 12)
        {
            guideInPcText.gameObject.SetActive(false);
            guideText = guideInWorld;
            guider.CanvasTuto.SetActive(true);
        }

        guideText.gameObject.SetActive(true);

        guideText.text = textsToGuide[currentIndexGuide];
        currentIndexGuide++;
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
            guider.ChangeTutoNode();
            NextTextToGuide();
        }
    }

    private void ChangeMarkIcon()
    {
        for (int i = 0; i < marks.Count; i++)
            if (markIndex == i)
            {
                Debug.Log("Activando marca en el índice: " + i);
                marks[i].SetActive(true);
            }
            else
                marks[i].SetActive(false);

        markIndex++;
    }
}
