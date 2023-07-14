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
        Punching,
        Roar
    }
    [Header("Main Variables")]
    public AIStates curState = AIStates.Idle;
    public NavMeshAgent agent;
    public Animator anim;    
    public float walkSpeed;
    public float runSpeed;    
    public LayerMask layerMask;
    Transform chasingPlayer;
    Vector3 ultimaLocation;    
    Vector3 nil=new Vector3(0,0,0);
    AudioSource audioSource;
    GameObject[] randLocations;
    int randGen;
    [Header("Punch and Roar")]
    public GameObject punchCol;
    public AudioClip punchSound;
    public AudioClip roarSound;


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
            case AIStates.Roar:
                RoarState();
                break;
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Generator"))
        {
            Random.Range(0,1);
        }
    }

    public void OnTriggerStay(Collider col)
    {
        if(col.CompareTag("Generator"))
        {
            if(randGen==1 && !isPunching && (curState == AIStates.Idle || curState == AIStates.Walking) && !GameObject.Find("GameManager").GetComponent<ManageGame>().foge && !col.gameObject.GetComponent<Generator>().Quebrado)
            {
                this.gameObject.transform.LookAt(col.gameObject.transform);
                punchType=1;
                ChangeToState(AIStates.Punching);
                randGen=0;
            }
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
            int roar = Random.Range(0,2);
            if(roar==2&&!jaRoarou)
            {
                ChangeToState(AIStates.Roar);
            }
            else
            {
                ChooseRandomLocation();
                ChangeToState(AIStates.Walking);
            }
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
            if (Physics.Raycast(objectPosition, direction, out hit, rayLength, layerMask))
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
            punchType = 0;
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
        yield return new WaitForSeconds(0.5f);
        chasingPlayer=null;
        jaToPerdendo=false;
    }

    bool isPunching = false;
    int punchType = 0;
    public void PunchingState()
    {
        agent.speed = 0;
        if(!isPunching)
        {
            isPunching=true;
            switch(punchType)
            {
                case 0:
                    anim.SetTrigger("Punch");
                    Invoke("BackToChasing", 1f);
                    break;
                case 1:
                    anim.SetTrigger("Punch");
                    Invoke("BackToIdle", 1f);
                    break;
            }

        }
    }

    public void BackToChasing()
    {
        ChangeToState(AIStates.Chasing);
        isPunching=false;
    }

    public void BackToIdle()
    {
        ChangeToState(AIStates.Idle);
        isPunching=false;
        isRoaring=false;
    }

    bool isRoaring = false;
    bool jaRoarou = false;
    public void RoarState()
    {
        agent.speed=0;
        if(!isRoaring)
        {
            isRoaring=true;
            jaRoarou=true;
            Invoke("VoltarRoar", 10f);
            anim.SetTrigger("Roar");
            Invoke("BackToIdle", 4.25f);
        }
    }

    public void SoundRoar()
    {
        audioSource.clip=roarSound;
        audioSource.Play();
    }

    public void VoltarRoar()
    {
        jaRoarou=false;
    }

    public void PunchColActive(int zero = 1)
    {
        punchCol.SetActive(zero==1?true:false);
        if(zero==0)
            return;
        audioSource.PlayOneShot(punchSound);
    }



    public void Start()
    {
        chasingPlayer = null;
        randLocations = GameObject.FindGameObjectsWithTag("RandPoints");
        audioSource=this.gameObject.GetComponent<AudioSource>();
    }

    public void Animations()
    {
        anim.SetBool("isWalking", agent.remainingDistance>0 && curState == AIStates.Walking);
        anim.SetBool("isRunning", agent.remainingDistance>0 && curState == AIStates.Chasing);
    }

}
