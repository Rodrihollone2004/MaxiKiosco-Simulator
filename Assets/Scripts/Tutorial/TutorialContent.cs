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
            "�Hola! �Te doy la bienvenida a tu primer trabajo!\r\nQuiz�s no sea mucho, pero antes que nada. �Todo suma!\r\nPor el momento necesito que cubras algunos d�as de la semana mientras termino de hacer unos tr�mites.\r\n�Vamos! Te explico lo que ten�s que hacer.\r\n Hacme click para seguir",
            "Todos los d�as vas a empezar ac�.\r\nEse es el Kiosco.\r\nHac� clic sobre la persiana para subirla.",
            "�Perfecto!\r\nSub� la t�rmica para prender las luces con el clic izquierdo.",
            "Ahora hace clic izquierdo en el monitor para gestionar la compu",
            "Cuando llegue un cliente ten�s que usar esta.\r\nAc� te dice qu� se van a llevar, cu�nto sale, c�mo paga y esas cosas.",
            "En esta se hacen los pedidos y se controla el stock.\r\nTen�s categor�as y ten�s que elegir cu�ntos paquetes quer�s de cada cosa.\r\nSimple. Eleg�s y te llega al dep�sito.",
            "En esta te voy a dejar que gastes algo de la plata del kiosco en hacer que se vea un poco mejor.\r\nSi el kiosco se ve bien va a entrar mas gente.",
            "Esta es la m�s importante si no quer�s perder plata.\r\nAc� vas a tener que ir categor�a en categor�a viendo los productos que ten�s a la venta y poni�ndoles un precio. \r\nSi es muy caro, puede que algunos clientes no lo compren.",
            "Esta secci�n de ac� te va a permitir cambiar el fondo de la pantalla para que te sea m�s ameno a la vista",
            "Por �ltimo, esta te va a permitir customizar el nombre de la tienda a gusto",
            "Hagamos un pedido.\r\nEntr� en la aplicaci�n del Stock y compra una caja de todo lo que veas.",
            "�Perfecto! Tenemos con qu� empezar.\r\nAnd� a buscarlos y los vas poniendo en la estanter�a.",
            "�Como toda la vida!\r\nLas botellas a la heladera y el resto donde quede mas lindo!\r\nCuando las cajas esten vac�as dejalas arriba del pallet, alguien va a venir a buscarlas despu�s",
            "Cuando pasen los d�as y tu experiencia te haga un mejor kiosquero, vas a poder desbloquear otra variedad de mercader�a.\r\nPrest� atenci�n para no perder d�as sin poner productos nuevos.\r\n�Empez� el d�a en la compu y buena suerte!\r\nHaceme click y me retiro",
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
                Debug.Log("Activando marca en el �ndice: " + i);
                marks[i].SetActive(true);
            }
            else
                marks[i].SetActive(false);

        markIndex++;
    }
}
