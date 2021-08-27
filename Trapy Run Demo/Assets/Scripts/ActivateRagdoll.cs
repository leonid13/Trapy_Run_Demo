using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateRagdoll : MonoBehaviour
{
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
        GetComponent<BoxCollider>().enabled = true;
    }

    public void ActivateRagdolll(Animator animator)
    {
        animator.enabled = false;
        foreach (var r in GetComponentsInChildren<Rigidbody>())
        {
            r.detectCollisions = true;
            r.isKinematic = false;
            r.AddForce((transform.forward + -transform.up) * 500f, ForceMode.Force);
        }

        foreach (var c in GetComponentsInChildren<Collider>())
        {
            c.enabled = true;
        }
    }
}
