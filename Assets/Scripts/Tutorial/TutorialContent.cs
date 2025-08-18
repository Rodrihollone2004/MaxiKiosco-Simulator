using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialContent : MonoBehaviour
{

    public static TutorialContent Instance;

    public bool isComplete { get; set; }

    private string[] textsToGuide;

    private int currentIndexGuide = 0;
    public int CurrentIndexGuide => currentIndexGuide;

    [SerializeField] private TMP_Text guideText;

    private Coroutine currentCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        textsToGuide = new string[]
            {
            "�Hola! �Te doy la bienvenida a tu primer trabajo! Quiz�s no sea mucho, pero antes que nada. �Todo suma!\r\nPor el momento necesito que cubras algunos d�as de la semana mientras termino de hacer unos tr�mites.\r\n�Vamos! Te explico lo que ten�s que hacer.",
            "Todos los d�as vas a empezar ac�.\r\nEse es el Kiosco. Hac� clic sobre la persiana para subirla.",
            "�Perfecto! Sub� la t�rmica para prender las luces con el clic izquierdo.",
            "Ahora hace clic izquierdo en el monitor para gestionar la compu",
            "En esta se hacen los pedidos. Ac� te van a aparecer todos los productos que tengas desbloqueados para comprar.",
            "Esta es la mas importante si no quer�s perder plata:\r\nAc� vas a tener todos los productos que tengas a la venta y vas a poder ponerles un precio. Trat� de no irte muy arriba porque los clientes pueden no comprar si est� muy caro. �No abuses!\r\n",
            "Cuando llegue un cliente ten�s que usar esta.\r\nAc� te dice qu� se van a llevar, cu�nto sale, c�mo paga y esas cosas.",
            "Por �ltimo, esta secci�n de ac� te va a permitir cambiar el fondo de la pantalla para que te sea m�s ameno a la vista",
            "Hagamos un pedido. Entr� en la aplicaci�n del Stock y compra una caja de todo lo que veas.",
            "�Perfecto! Tenemos con qu� empezar. Los pedidos los dejan en la puerta. And� a buscarlos y los vas poniendo en la estanter�a.",
            "�Ya casi terminamos! Ahora toca abrir y esperar que entre alguien.\r\nAnd� a la compu y toc� el bot�n verde que aparece arriba a la derecha. Autom�ticamente va a empezar a pasar el tiempo y hay que trabajar.\r\n�A las 22 termina el turno!",
            "Cuando te paguen en efectivo claramente vas a tener que darles vuelto la mayor�a de las veces. Abajo ten�s la caja y en la pantalla te dice cu�nto vuelto le ten�s que dar. Imposible equivocarse.\r\nAgarr� los billetes con el clic izquierdo y cuando tengas el vuelto apetras el Enter y listo. Si te pasas, le das con el clic derecho para sacar billetes.\r\n",
            "Cuando te paguen con QR, hacele clic al recuadro verde y escrib� el monto en pantalla. Cuando lo tengas apret� el Enter y listo.\r\n",
            "�Listo! �Ya est�s para quedarte hasta el cierre! �Mucha suerte y nos vemos ma�ana!"
            };
    }

    private void Start()
    {
        NextTextToGuide(false);
    }

    public void NextTextToGuide(bool isCompleteTutorial)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        guideText.gameObject.SetActive(true);

        guideText.text = textsToGuide[currentIndexGuide];
        currentIndexGuide++;

        isComplete = isCompleteTutorial;

        currentCoroutine = StartCoroutine(DesactivateText());
    }

    private IEnumerator DesactivateText()
    {
        yield return new WaitForSeconds(10f);
        guideText.gameObject.SetActive(false);

        currentCoroutine = null;

        if (currentIndexGuide == 1)
            NextTextToGuide(false);

    }

    public void CompleteStep(int stepIndex)
    {
        // Solo avanza si el paso actual coincide
        if (currentIndexGuide == stepIndex)
        {
            NextTextToGuide(false);
        }
    }
}
