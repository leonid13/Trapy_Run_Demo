using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform moveTarget;
    [SerializeField] private float speed = 4.3f;
    [SerializeField] private float boostSpeedOnTUrnning = 1f;
    [SerializeField] private float turnningTolerance = 0.25f;
    [SerializeField] private float minXMovement = -5f;
    [SerializeField] private float maxXMovement = 5f;

    private Camera mainCamera;
    private NavMeshAgent navMeshAgent;
    private RaycastHit hit;
    private RaycastHit[] nearbyEnemyHits;
    private bool callForEnemies = true;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        ProcessInput();
        AccelerateOnTurnning();
        moveTarget.position = new Vector3(moveTarget.position.x, 0, transform.position.z + 6f);
        if (navMeshAgent.enabled) navMeshAgent.SetDestination(moveTarget.position);

        if (!Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            navMeshAgent.enabled = false;
        }

        if (callForEnemies) CheckForEnemies();
    }

    // for some reason when Player was making a steep turn it slowed him down,
    // so i made this method to speed him up
    private void AccelerateOnTurnning()
    {
        if (Mathf.Abs(moveTarget.position.x - transform.position.x) > turnningTolerance)
        {
            navMeshAgent.speed = speed + boostSpeedOnTUrnning;
        }
        else
        {
            navMeshAgent.speed = speed;
        }
    }

    private void CheckForEnemies()
    {
        nearbyEnemyHits = Physics.SphereCastAll(transform.position, 1f, Vector3.up, 0, LayerMask.GetMask("Enemy"));
        int hitsLenght = nearbyEnemyHits.Length;
        if (hitsLenght == 0) return; // someone is near, Player got jumped on

        callForEnemies = false;
        GetComponent<NavMeshAgent>().isStopped = true;
        GetComponent<NavMeshAgent>().speed = 0;
        for (int i = 0; i < hitsLenght; i++)
        {
            StartCoroutine(nearbyEnemyHits[i].collider.GetComponent<Enemy>().JumpOnPlayer(true));
        }
    }

    private void ProcessInput()
    {
        if (Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            float cameraOffestZ = Vector3.Dot(transform.position - mainCamera.transform.position,
            mainCamera.transform.forward);
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3
            (touchPosition.x, touchPosition.y, cameraOffestZ));

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

    // PUBLIC METHODS
    public void Die()
    {
        Debug.Log("Die");
        //menu open try again
        //die animation
        //stop navmehs
        // navspeed to 0
    }
}
