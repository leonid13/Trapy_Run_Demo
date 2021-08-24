using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform moveTarget;
    [SerializeField] float jumpStrenght;// or jump time
    [SerializeField] float shoutDistance = 2f;

    private RaycastHit hit;
    private bool jumping = false;
    private Player player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Start()
    {
        GetComponent<NavMeshAgent>().SetDestination(moveTarget.position);
    }

    void Update()
    {
        if (!jumping && !Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            // StartCoroutine(JumpOverCubes());
        }
    }

    protected virtual IEnumerator JumpOverCubes()// this is diffrent for level 3 enemy
    {
        jumping = true;
        //jump animation
        //increase Y poition based on jump strenght
        //when Y pos is again 0, check for ground, if no ground is found disable navmeshaganet

        jumping = false;
        yield return null;
    }

    protected virtual void Movement()// this is diffrent for level 3 enemy
    {

    }

    private void Acceleration()// for when enemies are far behind
    {

    }

    // PUBLIC METHODS               // this also means first to jump on player
    public IEnumerator JumpOnPlayer(bool shouldAgrevateOthers)
    {
        Debug.Log("JumpOnPlayer");
        if (shouldAgrevateOthers)
        {
            AgrevateNearbyEnemies();
        }
        // stop navMesh
        // set speed to 0
        // jump animation
        // transform.translate to Player Coroutine
        // look at Player

        //only the first enemy that is near the Player, trigger his death
        // Just when he lands on him
        if (shouldAgrevateOthers)
        {
            player.Die();
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

    // public void AggrevateNearbyEnemiesOnDamaged()
    // {
    //     RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0, LayerMask.GetMask("Enemy"));
    //     int hitsLenght = hits.Length;
    //     if (hitsLenght == 0 || currentTarget == null) return;

    //     for (int i = 0; i < hitsLenght; i++)
    //     {
    //         AIController ai = hits[i].collider.GetComponent<AIController>();
    //         if (ai.fighter.CanAttack(currentTarget, true))
    //         {
    //             ai.AggrevateOnDamaged(currentTarget);
    //         }
    //     }
    // }
}
