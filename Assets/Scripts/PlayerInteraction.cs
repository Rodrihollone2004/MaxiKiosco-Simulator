using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode dropKey = KeyCode.G;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Transform holdPosition;

    [Header("Lanzamiento")]
    [SerializeField] private float throwForce = 10f;

    private IInteractable currentInteractable;
    private GameObject heldObject;
    private Rigidbody heldObjectRb;
    private Collider heldObjectCollider;

    private void Update()
    {
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

    private void PickUp(GameObject objToPickUp)
    {
        heldObject = objToPickUp;

        if (objToPickUp.TryGetComponent(out Rigidbody rb))
                heldObjectRb = rb;

        if (objToPickUp.TryGetComponent(out Collider col))
                heldObjectCollider = col;

        if (heldObjectRb != null)
                heldObjectRb.isKinematic = true;

        if (heldObjectCollider != null)
                heldObjectCollider.enabled = false;

        heldObject.transform.SetParent(holdPosition);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;
    }

    private void DropObject()
    {
        if (heldObjectRb != null)
                heldObjectRb.isKinematic = false;

        if (heldObjectCollider != null)
                heldObjectCollider.enabled = true;

        Vector3 forceDirection = cameraTransform.forward;
        heldObjectRb.AddForce(forceDirection * throwForce, ForceMode.Impulse);

        heldObject.transform.SetParent(null);
        heldObject = null;
        heldObjectRb = null;
        heldObjectCollider = null; 
    }
}

