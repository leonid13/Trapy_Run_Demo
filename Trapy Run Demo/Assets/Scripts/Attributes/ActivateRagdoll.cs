using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateRagdoll : MonoBehaviour
{
    [SerializeField] BoxCollider[] colliderToBeEnabled;

    // Here we disable all colliders and Rigidbodies and enable jest the base ones.
    private void Awake()
    {
        foreach (var r in GetComponentsInChildren<Rigidbody>())
        {
            r.detectCollisions = false;
            r.isKinematic = true;
        }
        foreach (var c in GetComponentsInChildren<Collider>())
        {
            c.enabled = false;
        }
        foreach (var col in colliderToBeEnabled)
        {
            col.enabled = true;
        }

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.detectCollisions = true;
        rigidbody.isKinematic = false;
    }

    // When the death of the object is near the Ragdoll is activated
    public void ActivateRagdolll(Animator animator)
    {
        animator.enabled = false;
        Destroy(GetComponent<Rigidbody>());
        foreach (var r in GetComponentsInChildren<Rigidbody>())
        {
            r.detectCollisions = true;
            r.isKinematic = false;
            r.AddForce((transform.forward + -transform.up) * 300f, ForceMode.Force);
        }

        foreach (var c in GetComponentsInChildren<Collider>())
        {
            c.enabled = true;
        }

    }
}
