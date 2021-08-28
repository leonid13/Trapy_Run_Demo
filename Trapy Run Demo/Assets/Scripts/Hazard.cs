using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hazard : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().Die();
            other.enabled = false;
        }
        else
        {
            other.GetComponent<ActivateRagdoll>().ActivateRagdolll(other.GetComponent<Animator>());
            other.GetComponent<NavMeshAgent>().enabled = false;
        }
    }
}
