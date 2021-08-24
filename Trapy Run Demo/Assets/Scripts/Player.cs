using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] Transform moveTarget;
    [SerializeField] float minXMovement = -5f;
    [SerializeField] float maxXMovement = 5f;

    private Camera mainCamera;
    private NavMeshAgent navMeshAgent;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        ProcessInput();
        moveTarget.position = new Vector3(moveTarget.position.x, 0, transform.position.z + 6f);
        navMeshAgent.SetDestination(moveTarget.position);

    }

    void ProcessInput()
    {
        if (Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            float cameraOffestZ = Vector3.Dot(transform.position - mainCamera.transform.position,
            mainCamera.transform.forward);
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3
            (touchPosition.x, touchPosition.y, cameraOffestZ));
            Debug.Log(worldPosition);

            if (worldPosition.x < minXMovement) { worldPosition.x = minXMovement; }
            else if (worldPosition.x > maxXMovement) worldPosition.x = maxXMovement;

            moveTarget.position = new Vector3(worldPosition.x, 0, moveTarget.position.z);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            other.GetComponent<FloorCube>().TriggerFall();
        }
    }
}
