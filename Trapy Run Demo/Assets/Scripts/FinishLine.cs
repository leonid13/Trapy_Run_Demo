using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    bool called = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && called == false)
        {
            called = true;
            gameManager.CrossFinishLine();
        }
    }
}
