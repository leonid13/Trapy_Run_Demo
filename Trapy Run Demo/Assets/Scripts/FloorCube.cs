using UnityEngine;

public class FloorCube : MonoBehaviour
{
    public void TriggerFall(GameObject cubeToTurnRed)
    {
        Destroy(Instantiate(cubeToTurnRed, transform.position, Quaternion.identity), 3f);
        Destroy(gameObject);
    }
}
