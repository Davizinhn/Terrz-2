using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Random = UnityEngine.Random;

//using Random = System.Random;

public class Enemy_Chase : MonoBehaviour
{
    public GameObject[] visions;
    public LayerMask collisionLayer;
    public bool Seguindo;
    public Transform seguindoEsse;
    public Animator anim;
    public NavMeshAgent agent;
    public GameObject[] randomPoints;
    public Vector3 ultimaLocalizacao;
    public bool andando;
    bool esperando=false;
    bool punching = false;
    public GameObject PunchCol;
    public AudioSource audioC;
    public AudioClip roarSound;
    public AudioClip punchSound;
    int randGen;
    public float minWalkV = 3f;
    public float minRunV = 5.25f;

    void Start(){
                if(PhotonNetwork.IsMasterClient){
        randomPoints = GameObject.FindGameObjectsWithTag("RandPoints");
        visions = GameObject.FindGameObjectsWithTag("MonsterVision");
        ChooseRandomPath();
                }
    }

    public void OnTriggerEnter(Collider collision)
    {
        randGen = Random.RandomRange(0,1);
    }

    public void OnTriggerStay(Collider collision)
    {
        if(collision.gameObject.tag=="Generator" && !collision.gameObject.GetComponent<Generator>().Quebrado && !punching && !Seguindo && collision.gameObject.GetComponent<Generator>().Ativada && randGen==0 && !andando && !GameObject.Find("GameManager").GetComponent<ManageGame>().foge)
        {
            Punch();
            this.gameObject.transform.LookAt(collision.gameObject.transform);
        }
    }

    public void Punch(bool parar=false)
    {
        if(!punching && !andando && !parar)
        {
            anim.SetTrigger("Punch");
            punching = true;
        }
        else if(!punching && parar)
        {
            anim.SetTrigger("Punch");
            Seguindo=false;
            andando=false;
        }
    }

    public void GeradorOuvir(Vector3 posicao)
    {
        if(!Seguindo)
        {
            agent.SetDestination(posicao);
        }
    }

    public void PunchColActive()
    {
        audioC.PlayOneShot(punchSound);
        PunchCol.active=true;
    }
    
    public void DeactivePunch()
    {
        PunchCol.active=false;
    }

    public void NoPunch()
    {
        PunchCol.active=false;
        punching=false;
    }

    void Update () { 
        if(PhotonNetwork.IsMasterClient)
        {

        foreach (GameObject vision in visions)
        {
            Quaternion rotation = vision.transform.rotation;
            Quaternion yRotation = Quaternion.AngleAxis(vision.transform.rotation.eulerAngles.y, Vector3.up);
            Vector3 direction = yRotation * Vector3.forward;
            RaycastHit hit;
            Ray ray = new Ray(vision.transform.position, direction);
            andando = agent.remainingDistance>0;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, collisionLayer))
            {
                Debug.DrawLine(vision.transform.position, hit.point, Color.green, 0.005f);
                if(hit.transform.gameObject.tag=="Player" && !Seguindo && !hit.transform.gameObject.GetComponent<FirstPersonMovement>().isLaying)
                {
                    StopCoroutine(Verify());
                    StartCoroutine(Verify());
                    Seguindo=true;
                    seguindoEsse = hit.transform;
                }
            }
        }

        if(Seguindo && agent.remainingDistance<2)
        {
            Punch(true);
            this.gameObject.transform.LookAt(seguindoEsse);
        }

        if(seguindoEsse!=null && !punching)
        {
            agent.SetDestination(seguindoEsse.position);
            ultimaLocalizacao = seguindoEsse.transform.position;
        }
        else if(seguindoEsse==null && !punching)
        {
            if(ultimaLocalizacao!=new Vector3(0, 0, 0) && agent.destination!=ultimaLocalizacao && agent.remainingDistance==0)
            {
                agent.SetDestination(ultimaLocalizacao);
            }
            if(agent.destination==ultimaLocalizacao && agent.remainingDistance==0 && !esperando)
            {
                StartCoroutine(Esperar());
            }
        }


        if(!Seguindo && !andando && !esperando)
        {
            StartCoroutine(Esperar());
        }

        if(Seguindo)
        {

                switch(PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    case 1:
                        agent.speed = minRunV;
                        break;
                    case 2:
                        agent.speed = minRunV + 0.25f;
                        break;
                    case 3:
                        agent.speed = minRunV + 0.5f;
                        break;
                    case 4:
                        agent.speed = minRunV + 0.75f;
                        break;
                    case >4:
                        agent.speed = minRunV + 1f;
                        break;
                }
        }
        else
        {
            agent.speed=minWalkV;
        }
        anim.SetBool("isWalking", agent.remainingDistance>0 && !Seguindo);
        anim.SetBool("isRunning", agent.remainingDistance>0 && Seguindo);
                this.gameObject.GetComponent<PhotonView>().RPC("SettingBool", RpcTarget.Others, Seguindo);
        }
    }

    public void ChooseRandomPath()
    {
        agent.SetDestination(randomPoints[Random.RandomRange(0, randomPoints.Length-1)].transform.position);
        Debug.Log("Escolhendo outro caminho");
    }

    IEnumerator Esperar()
    {
        int a = Random.RandomRange(0,6);
        if(a==0)
        {
            if(!punching && !Seguindo && !andando)
            {
            Roar();
            }
        }
        esperando=true;
        if(a==0)
        {
            yield return new WaitForSeconds(Random.RandomRange(6f, 8f));
            if(!Seguindo && !punching)
            {
                ultimaLocalizacao = new Vector3(0, 0, 0);
                ChooseRandomPath();
            }
            esperando=false;
        }
        else
        {
            yield return new WaitForSeconds(Random.RandomRange(3.5f, 6.5f));
            if(!Seguindo && !punching)
            {
                ultimaLocalizacao = new Vector3(0, 0, 0);
                ChooseRandomPath();
            }
            esperando=false;
        }
        yield break;
    }

    IEnumerator Verify()
    {
        yield return new WaitForSeconds(0.5f);
        seguindoEsse=null;
        this.gameObject.GetComponent<PhotonView>().RPC("SettingPlayer", RpcTarget.All, null);
        Seguindo = false;
        yield break;
    }

    public void Roar()
    {
        if(!punching && !andando && !Seguindo)
        {
            punching=true;
            anim.SetTrigger("Roar");
            this.gameObject.GetComponent<PhotonView>().RPC("RoarOthers", RpcTarget.OthersBuffered);
        }
    }

    [PunRPC]
    public void RoarOthers()
    {
            punching=true;
            anim.SetTrigger("Roar");
    }

    public void StopRoar()
    {
        punching=false;
    }

    public void PlaySoundRoar()
    {
                    audioC.PlayOneShot(roarSound);
    }

    [PunRPC]
    public void SettingBool (bool someValue)
    {
        Seguindo = someValue;
    }

    
}
