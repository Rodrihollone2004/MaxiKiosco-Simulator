using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Rigidbody[] rigidbodies;
    private Collider[] colliders;
    private Animator animator;

    void Awake()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        animator = GetComponent<Animator>();

        SetEnabled(false); // arranca desactivado
    }

    public void SetEnabled(bool enabled)
    {
        bool isKinematic = !enabled;

        // Activo/desactivo físicas
        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb != null)
                rb.isKinematic = isKinematic;
        }

        foreach (Collider col in colliders)
        {
            if (col != null && col.gameObject != this.gameObject) // evito el collider del root
                col.enabled = enabled;
        }

        // Desactivo el Animator cuando paso a ragdoll
        if (animator != null)
            animator.enabled = !enabled;
    }
}
