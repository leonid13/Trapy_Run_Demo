using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateRagdoll : MonoBehaviour
{
    [SerializeField] BoxCollider[] colliderToBeEnabled;
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

    //START

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
