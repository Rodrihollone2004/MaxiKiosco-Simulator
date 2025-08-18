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
            "¡Hola! ¡Te doy la bienvenida a tu primer trabajo! Quizás no sea mucho, pero antes que nada. ¡Todo suma!\r\nPor el momento necesito que cubras algunos días de la semana mientras termino de hacer unos trámites.\r\n¡Vamos! Te explico lo que tenés que hacer.",
            "Todos los días vas a empezar acá.\r\nEse es el Kiosco. Hacé clic sobre la persiana para subirla.",
            "¡Perfecto! Subí la térmica para prender las luces con el clic izquierdo.",
            "Ahora hace clic izquierdo en el monitor para gestionar la compu",
            "En esta se hacen los pedidos. Acá te van a aparecer todos los productos que tengas desbloqueados para comprar.",
            "Esta es la mas importante si no querés perder plata:\r\nAcá vas a tener todos los productos que tengas a la venta y vas a poder ponerles un precio. Tratá de no irte muy arriba porque los clientes pueden no comprar si está muy caro. ¡No abuses!\r\n",
            "Cuando llegue un cliente tenés que usar esta.\r\nAcá te dice qué se van a llevar, cuánto sale, cómo paga y esas cosas.",
            "Por último, esta sección de acá te va a permitir cambiar el fondo de la pantalla para que te sea más ameno a la vista",
            "Hagamos un pedido. Entrá en la aplicación del Stock y compra una caja de todo lo que veas.",
            "¡Perfecto! Tenemos con qué empezar. Los pedidos los dejan en la puerta. Andá a buscarlos y los vas poniendo en la estantería.",
            "¡Ya casi terminamos! Ahora toca abrir y esperar que entre alguien.\r\nAndá a la compu y tocá el botón verde que aparece arriba a la derecha. Automáticamente va a empezar a pasar el tiempo y hay que trabajar.\r\n¡A las 22 termina el turno!",
            "Cuando te paguen en efectivo claramente vas a tener que darles vuelto la mayoría de las veces. Abajo tenés la caja y en la pantalla te dice cuánto vuelto le tenés que dar. Imposible equivocarse.\r\nAgarrá los billetes con el clic izquierdo y cuando tengas el vuelto apetras el Enter y listo. Si te pasas, le das con el clic derecho para sacar billetes.\r\n",
            "Cuando te paguen con QR, hacele clic al recuadro verde y escribí el monto en pantalla. Cuando lo tengas apretá el Enter y listo.\r\n",
            "¡Listo! ¡Ya estás para quedarte hasta el cierre! ¡Mucha suerte y nos vemos mañana!"
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
