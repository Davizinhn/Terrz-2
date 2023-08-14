using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Random = UnityEngine.Random;
using Workbench.Wolfsbane.Multiplayer;
using DG.Tweening;
using DG.Tweening.Core;
using DG;
using DG.Tweening.Plugins;
using Workbench.Wolfsbane.Multiplayer;

public class EnemyAI : MonoBehaviour, IPunObservable
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
    public AudioSource PassosSound;


    public void ChangeToState(AIStates state)
    {
        if(curState != state)
            curState = state;
            GetComponent<PhotonView>().RPC("ChaseForOthers", RpcTarget.OthersBuffered, state==AIStates.Chasing);
    }

    [PunRPC]
    public void ChaseForOthers(bool a = false)
    {
        curState=a?AIStates.Chasing:AIStates.Idle;
    }




    public void Update()
    {
        if(PhotonNetwork.IsMasterClient)
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
                case AIStates.LookingBed:
                    LookingBedState();
                    break;
            }
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
        if(col.CompareTag("Bed") && curState==AIStates.Idle && !isLookingBed)
        {
            curBed = col.gameObject.GetComponent<BedBehaviour>();
            ChangeToState(AIStates.LookingBed);
        }
    }

    bool isLoopingIdle = false;
    public void IdleState()
    {
        if(PassosSound.isPlaying)
        {
            gameObject.GetComponent<PhotonView>().RPC("PassosTirarOuColocar", RpcTarget.AllBuffered, 0);
        }
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
            int roar = Random.Range(0,1);
            if(roar==1&&!jaRoarou)
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
        if(!PassosSound.isPlaying || PassosSound.pitch != 0.95f)
        {
            gameObject.GetComponent<PhotonView>().RPC("PassosTirarOuColocar", RpcTarget.AllBuffered, 1);
        }
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
        int i = Random.Range(0, randLocations.Length-1);
        agent.SetDestination(randLocations[i].transform.position);        
        Debug.DrawLine(agent.transform.position, agent.destination, Color.blue, 10f);
        alreadyWalking=true;
    }


    public void AvisarGenerator(Transform thisGen)
    {
        if(curState == AIStates.Idle || curState == AIStates.Walking)
        {
            agent.SetDestination(EncontrarTransformMaisProximo(thisGen).position);
            ChangeToState(AIStates.Walking);
        }
    }

    private Transform EncontrarTransformMaisProximo(Transform referencia)
    {
        float distanciaMinima = Mathf.Infinity;
        Transform transformMaisProximo = null;

        foreach (GameObject transformAtual in randLocations)
        {
            float distancia = Vector3.Distance(transformAtual.transform.position, referencia.transform.position);

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                transformMaisProximo = transformAtual.transform;
            }
        }

        return transformMaisProximo;
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
        if(!PassosSound.isPlaying || PassosSound.pitch != 1.6f)
        {
            gameObject.GetComponent<PhotonView>().RPC("PassosTirarOuColocar", RpcTarget.AllBuffered, 2);
        }
        float multiplier = PhotonNetwork.PlayerList.Length<3 ? 1f : 1.15f;
        agent.speed=runSpeed * multiplier;
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
        if(PassosSound.isPlaying)
        {
            gameObject.GetComponent<PhotonView>().RPC("PassosTirarOuColocar", RpcTarget.AllBuffered, 0);
        }
        agent.speed = 0;
        if(!isPunching)
        {
            isPunching=true;
            switch(punchType)
            {
                case 0:
                    anim.SetBool("Punch", true);
                    Invoke("BackToChasing", 1f);
                    break;
                case 1:
                    anim.SetBool("Punch", true);
                    Invoke("BackToIdle", 1f);
                    break;
                case 2:
                    anim.SetBool("isSomeoneHere", true);
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

    public void BackToWalking()
    {
        curBed=null;
        ChooseRandomLocation();
        ChangeToState(AIStates.Walking);
        isPunching=false;
        isRoaring=false;
        isLookingBed=false;
    }

    bool isRoaring = false;
    bool jaRoarou = false;
    public void RoarState()
    {
        if(PassosSound.isPlaying)
        {
            gameObject.GetComponent<PhotonView>().RPC("PassosTirarOuColocar", RpcTarget.AllBuffered, 0);
        }
        agent.speed=0;
        if(!isRoaring)
        {
            isRoaring=true;
            jaRoarou=true;
            Invoke("VoltarRoar", 10f);
            anim.SetBool("Roar", true);
            Invoke("BackToIdle", 4.25f);
        }
    }

    public void SoundRoar()
    {
        audioSource.PlayOneShot(roarSound);
    }

    public void VoltarRoar()
    {
        jaRoarou=false;
        isRoaring=false;
    }

    public void PunchColActive(int zero = 1)
    {
        punchCol.SetActive(zero==1?true:false);
        if(zero==0)
            return;
        audioSource.PlayOneShot(punchSound);
        if(isLookingBed)
        {
            Debug.Log("chegei na 359");
            foreach(FirstPersonMovement a in GameObject.FindObjectsOfType<FirstPersonMovement>())
            {
                if(a.curBed.gameObject==curBed.gameObject)
                {
                    Debug.Log("chegei na 364");
                    Debug.Log(a.gameObject.GetComponent<PhotonView>().ViewID);
                    gameObject.GetComponent<PhotonView>().RPC("SimulatePunch", RpcTarget.AllBuffered, a.gameObject.GetComponent<PhotonView>().ViewID);
                }
            }
        }
    }

    [PunRPC]
    public void SimulatePunch(int playerToPunch1)
    {
        Debug.Log(playerToPunch1);
        PhotonView a = PhotonView.Find(playerToPunch1);
        GameObject playerToPunch = a.gameObject;
        playerToPunch.gameObject.GetComponent<FirstPersonMovement>().Morrer();
    }

    bool isLookingBed = false;
    public BedBehaviour curBed = null;
    public void LookingBedState()
    {
        if(PassosSound.isPlaying)
        {
            gameObject.GetComponent<PhotonView>().RPC("PassosTirarOuColocar", RpcTarget.AllBuffered, 0);
        }
        agent.speed=0;
        if(!isLookingBed)
        {
            transform.DOMove(curBed.spotMonstro.position, 0.25f).SetEase(Ease.InCubic);        
            transform.LookAt(curBed.transform.position);
            isLookingBed=true;
            anim.SetBool("LookHere", true);
            gameObject.GetComponent<PhotonView>().RPC("LookToThat", RpcTarget.OthersBuffered, curBed.transform.position.x, curBed.transform.position.y, curBed.transform.position.z);
            if(curBed.isSomeoneHere)
            {
                punchType = 2;
                ChangeToState(AIStates.Punching);
            }
        }
    }

    public void TirarHere(int a = 1)
    {
        anim.SetBool("LookHere", false);
        if(a==1)
            anim.SetBool("isSomeoneHere", false);
    }

    [PunRPC]
    public void LookToThat(float vectorX, float vectorY, float vectorZ)
    {
        transform.LookAt(new Vector3(vectorX, vectorY, vectorZ));
    }

    public void Start()
    {
        //PhotonNetwork.AutomaticallySyncScene = true;
        chasingPlayer = null;
        randLocations = GameObject.FindGameObjectsWithTag("RandPoints");
        audioSource=this.gameObject.GetComponent<AudioSource>();
    }

    public void Animations()
    {
        anim.SetBool("isWalking", agent.remainingDistance>0 && curState == AIStates.Walking);
        anim.SetBool("isRunning", agent.remainingDistance>0 && curState == AIStates.Chasing);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We are the Master Client, send data to others
            stream.SendNext(curBed != null ? curBed.gameObject.GetPhotonView().ViewID : -1);
        }
        else
        {
            // We are not the Master Client, receive data from Master Client
            int receivedViewID = (int)stream.ReceiveNext();
            curBed = receivedViewID == -1 ? null : PhotonView.Find(receivedViewID).GetComponent<BedBehaviour>();
        }
    }

    /*[PunRPC]
    public void TriggerAnim1(string which1)
    {
        anim.SetTrigger(which1);
    }*/

    public void TirarPunchERoar(int qual)
    {
        if(qual==1)
        {
            anim.SetBool("Punch", false);
        }
        else
        {
            anim.SetBool("Roar", false);
        }
    }

    [PunRPC]
    public void PassosTirarOuColocar(int simoun)
    {
        switch(simoun)
        {
            case 0:
                PassosSound.Stop();
                break;
            case 1:
                if(!PassosSound.isPlaying)
                    {PassosSound.Play();}
                PassosSound.pitch=0.95f;
                break;
            case 2:
                if(!PassosSound.isPlaying)
                    {PassosSound.Play();}
                PassosSound.pitch=1.6f;
                break;
        }
    }


}
