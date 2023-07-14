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
    public GameObject punchCol;
    Vector3 nil=new Vector3(0,0,0);

    public void ChangeToState(AIStates state)
    {
        if(curState != state)
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
            case AIStates.Chasing:
                ChasingState();
                break;
            case AIStates.Punching:
                PunchingState();
                break;
        }
    }

    bool isLoopingIdle = false;
    public void IdleState()
    {
        agent.speed = 0;
        VerifyForPlayer();
        if(!isLoopingIdle)
        {
            StartCoroutine(idleLoop());
        }
    }

    public IEnumerator idleLoop()
    {
        isLoopingIdle=true;
        yield return new WaitForSeconds(Random.Range(3,5));
        if(curState == AIStates.Idle)
        {
            ChooseRandomLocation();
            ChangeToState(AIStates.Walking);
        }
        isLoopingIdle=false;
    }

    bool alreadyWalking = false;
    public void WalkingState()
    {
        agent.speed=walkSpeed;
        isLoopingIdle=false;
        VerifyForPlayer();
        if(!alreadyWalking && ultimaLocation != nil)
        {
            alreadyWalking=true;
            agent.SetDestination(ultimaLocation);
        }
        else if(!alreadyWalking && ultimaLocation==nil)
        {
            ChooseRandomLocation();
        }
        if(alreadyWalking && agent.destination == agent.gameObject.transform.position)
        {
            if(agent.destination==ultimaLocation)
            {
                ultimaLocation=nil;
            }
            ChangeToState(AIStates.Idle);
            alreadyWalking=false;
        }
    }

    public void ChooseRandomLocation()
    {
        agent.SetDestination(randLocations[Random.Range(0, randLocations.Length-1)].transform.position);        
        Debug.DrawLine(agent.transform.position, agent.destination, Color.blue, 10f);
        alreadyWalking=true;
    }

    public void VerifyForPlayer()
    {
        int rayCount = 75;
        float coiso=95f;
        float rayLength = 100f;
        float angleBetweenRays = 2.9f; 
        Vector3 objectPosition = transform.position;
        Quaternion objectRotation = transform.rotation;

        for (int i = 0; i < rayCount; i++)
        {
            Quaternion rotation = Quaternion.Euler(0f, i * angleBetweenRays + objectRotation.eulerAngles.y - coiso, 0f);
            Vector3 direction = rotation * Vector3.forward;

            RaycastHit hit;
            if (Physics.Raycast(objectPosition, direction, out hit, rayLength))
            {
                Debug.DrawRay(objectPosition, direction * hit.distance, Color.red);
                if(hit.transform.gameObject.CompareTag("Player"))
                {
                    if(curState == AIStates.Chasing)
                    {
                        ultimaLocation = hit.transform.position;
                        chasingPlayer = hit.transform;
                    }
                    else
                    {
                        ultimaLocation = hit.transform.position;
                        chasingPlayer = hit.transform;
                        ChangeToState(AIStates.Chasing);
                    }
                }
            }
            else
            {
                Debug.DrawRay(objectPosition, direction * rayLength, Color.green);
            }
        }
    }

    bool jaToPerdendo=false;
    public void ChasingState()
    {
        agent.speed=runSpeed;
        isLoopingIdle=false;
        //isPunching=false;
        alreadyWalking=false;
        VerifyForPlayer();
        if(chasingPlayer!=null && agent.remainingDistance < 1.5f && jaToPerdendo)
        {
            ChangeToState(AIStates.Punching);
        }
        if(chasingPlayer!=null && !jaToPerdendo)
        {
            StartCoroutine(perderPlayer());
        }
        if(chasingPlayer==null)
        {
            ChangeToState(AIStates.Walking);
        }
        else
        {
            agent.SetDestination(chasingPlayer.transform.position);
        }
    }

    public IEnumerator perderPlayer()
    {
        jaToPerdendo=true;
        yield return new WaitForSeconds(1.2f);
        chasingPlayer=null;
        jaToPerdendo=false;
    }

    bool isPunching = false;
    public void PunchingState()
    {
        agent.speed = 0;
        if(!isPunching)
        {
            isPunching=true;
            anim.SetTrigger("Punch");
            Invoke("BackToChasing", 1f);
        }
    }

    public void BackToChasing()
    {
        ChangeToState(AIStates.Chasing);
        isPunching=false;
    }

    public void PunchColActive(int zero = 1)
    {
        punchCol.SetActive(zero==1?true:false);
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
