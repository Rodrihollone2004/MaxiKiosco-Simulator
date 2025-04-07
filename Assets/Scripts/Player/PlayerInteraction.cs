using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
     [Header("Config")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode dropKey = KeyCode.G;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Transform holdPosition;

    [Header("Throw")]
    [SerializeField] private float throwForce = 10f;

    [Header("Feedback")]
    [SerializeField] private Color outlineColor = Color.magenta;
    [SerializeField] private float outlineWidth = 7.0f;
    [SerializeField] private GameObject dropHintUI;
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Material defaultMaterial;

    //[Header("Efects")]
    //[SerializeField] private AudioClip pickupSound;
    //[SerializeField] private AudioClip dropSound;
    //private AudioSource audioSource;

    private IInteractable currentInteractable;
    private GameObject heldObject;
    private Rigidbody heldObjectRb;
    private Collider heldObjectCollider;

    private Renderer currentRenderer;
    //private MaterialPropertyBlock propBlock;

    //private void Awake()
    //{
    //    propBlock = new MaterialPropertyBlock();
    //}

    private void Update()
    {
        HandleHighlight();

        if (Input.GetKeyDown(interactKey))
        {
            if (heldObject == null)
            {
                TryPickUp();
            }
        }

        if (Input.GetKeyDown(dropKey) && heldObject != null)
        {
            DropObject();
        }
    }

    // detecta los objetos con raycast y aplica el outline
    private void HandleHighlight()
    {
        if (heldObject != null)
        {
            if (currentRenderer != null)
            {
                RemoveHighlight(currentRenderer);
                currentRenderer = null;
            }
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactRange, interactLayer))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();

            if (renderer == null) return;

            if (renderer != currentRenderer)
            {
                if (currentRenderer != null)
                {
                    RemoveHighlight(currentRenderer);
                }
                currentRenderer = renderer;
                ApplyHighlight(currentRenderer);
            }
        }
        else
        {
            if (currentRenderer != null)
            {
                RemoveHighlight(currentRenderer);
                currentRenderer = null;
            }
        }
    }

    private void ApplyHighlight(Renderer renderer)
    {
        //renderer.GetPropertyBlock(propBlock);
        //propBlock.SetColor("_Color", outlineColor);
        //propBlock.SetFloat("_Scale", outlineWidth);
        //renderer.SetPropertyBlock(propBlock);

        Material[] materials = renderer.sharedMaterials;

        if (materials.Length == 1 || materials[materials.Length - 1] != outlineMaterial)
        {
            Material[] newMaterials = new Material[materials.Length + 1];
            materials.CopyTo(newMaterials, 0);
            newMaterials[materials.Length] = outlineMaterial;
            renderer.materials = newMaterials;
        }
    }

    private void RemoveHighlight(Renderer renderer)
    {
        //renderer.GetPropertyBlock(propBlock);
        //propBlock.SetFloat("_Scale", 0f);
        //renderer.SetPropertyBlock(propBlock);

        Material[] materials = renderer.sharedMaterials;

        if (materials.Length > 1 && materials[materials.Length - 1] == outlineMaterial)
        {
            Material[] newMaterials = new Material[materials.Length - 1];
            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = materials[i];
            }
            renderer.materials = newMaterials;
        }
    }

    // intenta recoger un objeto con el raycast
    private void TryPickUp()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactRange, interactLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact();
                PickUp(hit.collider.gameObject);
            }
        }
    }

    // hace el objeto hijo del holdposition
    private void PickUp(GameObject objToPickUp)
    {
        if (currentRenderer != null)
        {
            RemoveHighlight(currentRenderer);
        }

        heldObject = objToPickUp;

        if (objToPickUp.TryGetComponent(out Rigidbody rb))
             heldObjectRb = rb;

        if (objToPickUp.TryGetComponent(out Collider col))
             heldObjectCollider = col;

        if (heldObjectRb != null)
            heldObjectRb.isKinematic = true;

        if (heldObjectCollider != null)
            heldObjectCollider.isTrigger = true;

        heldObject.transform.SetParent(holdPosition);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;

        if (dropHintUI != null)
        {
            dropHintUI.SetActive(true);
        }

        //if (audioSource != null && pickupSound != null)
        //{
        //    audioSource.PlayOneShot(pickupSound);
        //}
    }

    // devuelve todas las propiedades al objeto y aplica fuerza de lanzamiento
    private void DropObject()
    {

        if (heldObject != null)
        {
            heldObject.transform.position += Vector3.up * 0.2f;

            if (heldObjectRb != null)
            {
                heldObjectRb.isKinematic = false;
                heldObjectRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                heldObjectRb.AddForce(cameraTransform.forward * throwForce, ForceMode.Impulse);
            }

            if (heldObjectCollider != null)
                heldObjectCollider.isTrigger = false;

            if (dropHintUI != null)
            {
                dropHintUI.SetActive(false);
            }

            heldObject.transform.SetParent(null);
            heldObject = null;
            heldObjectRb = null;
            heldObjectCollider = null;

            //if (audioSource != null && dropSound != null)
            //{
            //    audioSource.PlayOneShot(dropSound);
            //}
        }
    }
}

