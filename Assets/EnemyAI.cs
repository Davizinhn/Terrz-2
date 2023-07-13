using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Random = UnityEngine.Random;
using Workbench.Wolfsbane.Multiplayer;

public class EnemyAI : MonoBehaviour
{
    public enum AIStates{
        Idle,
        Walking,
        Chasing,
        LookingBed,
        Punching
    }
    public AIStates curState = AIStates.Idle;
    public NavMeshAgent agent;
    public Animator anim;
    Transform chasingPlayer;
    Vector3 ultimaLocation;
    public float walkSpeed;
    public float runSpeed;
    GameObject[] randLocations;

    public void ChangeToState(AIStates state)
    {
        curState = state;
    }

    public void Update()
    {
        Animations();
        switch(curState)
        {
            case AIStates.Idle:
                IdleState();
                break;
            case AIStates.Walking:
                WalkingState();
                break;
        }
    }

    bool isLoopingIdle = false;
    public void IdleState()
    {
        agent.speed = 0;
        if(!isLoopingIdle)
        {
            StartCoroutine(idleLoop());
        }
    }

    public IEnumerator idleLoop()
    {
        isLoopingIdle=true;
        yield return new WaitForSeconds(Random.Range(2,5));
        if(curState == AIStates.Idle)
        {
            ChangeToState(AIStates.Walking);
        }
        isLoopingIdle=false;
        StopCoroutine(idleLoop());
    }

    bool alreadyWalking = false;
    public void WalkingState()
    {
        agent.speed=walkSpeed;
        if(!alreadyWalking)
        {
            ChooseRandomLocation();
        }
        if(alreadyWalking && agent.remainingDistance==0f)
        {
            ChangeToState(AIStates.Idle);
            alreadyWalking=false;
        }
    }

    public void ChooseRandomLocation()
    {
        agent.SetDestination(randLocations[Random.Range(0, randLocations.Length-1)].transform.position);        
        alreadyWalking=true;
        Debug.DrawLine(agent.transform.position, agent.destination, Color.blue, 10f);

    }



    public void Start()
    {
        chasingPlayer = null;
        randLocations = GameObject.FindGameObjectsWithTag("RandPoints");
    }

    public void Animations()
    {
        anim.SetBool("isWalking", agent.remainingDistance>0 && curState == AIStates.Walking);
        anim.SetBool("isRunning", agent.remainingDistance>0 && curState == AIStates.Chasing);
    }

}
