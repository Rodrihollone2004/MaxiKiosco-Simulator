using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Rigidbody[] rigidbodies;
    private Collider[] colliders;
    private Animator animator;

    private float ragdollTimer = 0f;
    private bool isRagdollActive = false;

    void Awake()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        animator = GetComponent<Animator>();

        SetEnabled(false); 
    }

    void Update()
    {
        if (isRagdollActive)
        {
            ragdollTimer -= Time.deltaTime;

            if (ragdollTimer <= 0f)
            {
                SetEnabled(false);
                isRagdollActive = false;

                ThiefController thief = GetComponent<ThiefController>();
                if (thief != null)
                {
                    thief.ResumeMovement();
                }
            }
        }
    }

    public void SetEnabled(bool enabled)
    {
        bool isKinematic = !enabled;

        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb != null)
                rb.isKinematic = isKinematic;
        }

        foreach (Collider col in colliders)
        {
            if (col != null && col.gameObject != this.gameObject)
                col.enabled = enabled;
        }

        if (animator != null)
            animator.enabled = !enabled;
    }

    public void ActivateTemporaryRagdoll(float duration)
    {
        SetEnabled(true);
        isRagdollActive = true;
        ragdollTimer = duration;
    }
}
