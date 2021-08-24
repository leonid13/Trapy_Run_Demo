using UnityEngine;

public class FloorCube : MonoBehaviour
{
    public void TriggerFall()
    {
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Animator>().SetTrigger("Change");
    }
}
