using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform moveHere;
    [SerializeField] GameObject playerVCam;
    [SerializeField] GameObject winVcam;

    bool called = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && called == false)
        {
            other.GetComponent<NavMeshAgent>().SetDestination(moveHere.position);
            StartCoroutine(RotatePlayer(other.transform));
            other.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Dance");
            // when cameras are switched the TargetGroup1 game object
            // makes a smooth transition between them
            winVcam.SetActive(true);
            playerVCam.SetActive(false);
            called = true;
            gameManager.CrossFinishLine();
        }
    }

    private IEnumerator RotatePlayer(Transform player)
    {
        while (player.rotation.y < 180)
        {
            player.Rotate(new Vector3(0, 20, 0) * Time.deltaTime * 3, Space.Self);
            yield return null;
        }
        yield return null;
    }
}
