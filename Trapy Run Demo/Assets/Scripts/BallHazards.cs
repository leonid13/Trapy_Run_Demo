using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHazards : MonoBehaviour
{
    ConstantForce constantForcee;

    void Awake()
    {
        constantForcee = GetComponent<ConstantForce>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y >= 10f)
        {
            constantForcee.enabled = false;
        }
        else if (transform.position.y <= 1f)
        {
            constantForcee.enabled = true;
        }
    }
}
