using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform moveTarget;
    //    ratios below coresponding to cube jumps  3      2       1        0
    [SerializeField] float jumpTime = 0.75f;//   0.50f | 0.25f | 0.13f |  0.05f
    [SerializeField] float jumpAnimSpeed = 1f;// 1.25f | 1.6f  |  2f   |  1f no jump
    [SerializeField] float shoutDistance = 2f;
    [SerializeField] float maxSpeedBoost = 10f;
    [SerializeField] float minSpeed = 5f;
    [SerializeField] bool isSmart = false;
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
        if (moveTarget == null && !isSmart) Destroy(gameObject);
        else if (!isSmart) navMeshAgent.SetDestination(moveTarget.position);

        if (isSmart) InvokeRepeating(nameof(SetDestitantion), 0, 0.2f);
    }

    private void Update()
    {
        Acceleration();
        if (collisions == 0 && jumping == false)
        {
            StartCoroutine(JumpOverCubes());
        }
    }

    private void SetDestitantion()
    {
        NavMeshHit hit;
        if (navMeshAgent.SamplePathPosition(NavMesh.AllAreas, 30.0F, out hit))
        {
            navMeshAgent.SetDestination(playerTrans.position);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator JumpOverCubes()
    {
        //Start the jumping based on the values jumpTime and jumpAnimSpeed
        jumping = true;
        animator.SetBool("Jump", true);

        animator.speed = jumpAnimSpeed;
        //wait for jumping over N cubes
        yield return new WaitForSeconds(jumpTime);

        //then check if there is a cube below, if yes >>> continue running
        if (collisions == 0)
        {
            Destroy(gameObject, 5f);
            GetComponent<ActivateRagdoll>().ActivateRagdolll(animator);
            navMeshAgent.enabled = false;
        }

        animator.speed = 1f;
        jumping = false;
        animator.SetBool("Jump", false);
    }

    // for when enemies are far behind the Player
    private void Acceleration()
    {
        if (playerTrans.position.z > transform.position.z)
        {
            navMeshAgent.speed = (1 - transform.position.z / playerTrans.position.z) * maxSpeedBoost + minSpeed;
        }
    }

    // Keeping track of how many cube the enemy is touching, when 0 >>> jump
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
        //This method is called in the Player script, when the enemy gets too close
        //Here the enemy is throwing its self on the Player
        if (shouldAgrevateOthers)
        {
            AgrevateNearbyEnemies();
        }
        navMeshAgent.SetDestination(playerTrans.position);
        animator.SetTrigger("JumpOnPlayer");
        // wait for peak in jump animation
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject, 5f);
        GetComponent<ActivateRagdoll>().ActivateRagdolll(animator);

        //only the first enemy that gets called will kill the player
        if (shouldAgrevateOthers)
        {
            playerTrans.GetComponent<Player>().Die();
        }
        navMeshAgent.speed = 0;
        navMeshAgent.isStopped = true;
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
