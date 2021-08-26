using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform moveTarget;
    // jumpTime goes in hand with the value for jumpAnimSpeed and jumps over X cubes, this is for different enemy Levels
    //                                        3 cubes    2      1     0
    [SerializeField] float jumpTime = 0.75f;// 0.75f | 0.50f | 0.3f | 0.07f
    [SerializeField] float jumpAnimSpeed = 1f;// 1f  | 1.25f | 1.5f | 1f no jump
    [SerializeField] float shoutDistance = 2f;
    [SerializeField] float maxSpeedBoost = 10f;
    [SerializeField] float minSpeed = 5f;
    [SerializeField] Animator animator;

    private RaycastHit hit;
    private bool jumping = false;
    private Transform playerTrans;
    private int collisions = 0;
    private NavMeshAgent navMeshAgent;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTrans = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Start()
    {
        navMeshAgent.SetDestination(moveTarget.position);
    }

    private void Update()
    {
        Acceleration();
        if (collisions == 0 && jumping == false)
        {
            StartCoroutine(JumpOverCubes());
        }
    }

    protected virtual IEnumerator JumpOverCubes()// this is diffrent for level 3 enemy
    {
        jumping = true;
        animator.SetBool("Jump", true);

        animator.speed = jumpAnimSpeed;
        yield return new WaitForSeconds(jumpTime);

        if (collisions == 0)
        {
            GetComponent<ActivateRagdoll>().ActivateRagdolll(animator);
            navMeshAgent.enabled = false;
        }

        animator.speed = 1f;
        jumping = false;
        animator.SetBool("Jump", false);
    }

    protected virtual void Movement()// this is diffrent for level 3 enemy
    {

    }

    private void Acceleration()// for when enemies are far behind
    {
        navMeshAgent.speed = (1 - transform.position.z / playerTrans.position.z) * maxSpeedBoost + minSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        collisions++;
    }

    private void OnTriggerExit(Collider other)
    {
        collisions--;
    }

    // PUBLIC METHODS               // this also means first to jump on player
    public IEnumerator JumpOnPlayer(bool shouldAgrevateOthers)
    {
        if (shouldAgrevateOthers)
        {
            AgrevateNearbyEnemies();
        }
        navMeshAgent.SetDestination(playerTrans.position);
        animator.SetTrigger("JumpOnPlayer");
        yield return new WaitForSeconds(0.4f);// wait for peak in jump animation
        GetComponent<ActivateRagdoll>().ActivateRagdolll(animator);
        if (shouldAgrevateOthers)
        {
            playerTrans.GetComponent<Player>().Die();
        }
        yield return null;
    }

    public void AgrevateNearbyEnemies()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0, LayerMask.GetMask("Enemy"));
        int hitsLenght = hits.Length;
        if (hitsLenght == 0) return;

        for (int i = 0; i < hitsLenght; i++)
        {
            StartCoroutine(hits[i].collider.GetComponent<Enemy>().JumpOnPlayer(false));
        }
    }
}
