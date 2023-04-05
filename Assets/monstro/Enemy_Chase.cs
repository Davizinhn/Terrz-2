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

    void Start(){
        randomPoints = GameObject.FindGameObjectsWithTag("RandPoints");
        visions = GameObject.FindGameObjectsWithTag("MonsterVision");
        ChooseRandomPath();
    }

    void Update () { 
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
                if(hit.transform.gameObject.tag=="Player")
                {
                    StopCoroutine(Verify());
                    StartCoroutine(Verify());
                    Seguindo=true;
                    seguindoEsse = hit.transform;
                }
            }
        }

        if(seguindoEsse!=null)
        {
            agent.SetDestination(seguindoEsse.position);
            ultimaLocalizacao = seguindoEsse.transform.position;
        }
        else
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
            agent.speed=6f;
        }
        else
        {
            agent.speed=3.5f;
        }
        anim.SetBool("isWalking", agent.remainingDistance>0 && !Seguindo);
        anim.SetBool("isRunning", agent.remainingDistance>0 && Seguindo);
    }

    public void ChooseRandomPath()
    {
        agent.SetDestination(randomPoints[Random.RandomRange(0, randomPoints.Length-1)].transform.position);
        Debug.Log("Escolhendo outro caminho");
    }

    IEnumerator Esperar()
    {
        esperando=true;
        yield return new WaitForSeconds(Random.RandomRange(1.5f, 4.5f));
        if(!Seguindo)
        {
            ultimaLocalizacao = new Vector3(0, 0, 0);
            ChooseRandomPath();
        }
        esperando=false;
        yield break;
    }

    IEnumerator Verify()
    {
        yield return new WaitForSeconds(2.5f);
        seguindoEsse=null;
        Seguindo = false;
        yield break;
    }

    
}
