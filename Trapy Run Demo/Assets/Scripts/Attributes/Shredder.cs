using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : MonoBehaviour
{
    //This script destroys objects that are behind the player.
    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
}
