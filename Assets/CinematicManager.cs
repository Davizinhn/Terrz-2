using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CinematicManager : MonoBehaviour
{
    public bool inCinematic = true;
    bool seguir = false;
    bool allPlayersReady=false;
    public GameObject waitingForOthers;
    public AudioSource music;
    Vector3 targetPosition;
    Quaternion targetRotation;
    GameObject monstro;
    bool adicionei;


    public void Awake()
    {
        monstro = GameObject.Find("Monstro");
        music.Play();
        monstro.GetComponent<ChromaticControler>().normalMusic.gameObject.active=false;
        StartCoroutine(GoToCamera());
    }

    void LateUpdate() {
        targetPosition = Camera.main.gameObject.transform.position;
        targetRotation = Camera.main.gameObject.transform.rotation;
        if(seguir)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5);
        }

        if(this.gameObject.transform.position == targetPosition)
        {
            CinematicBack();
            if(!adicionei)
            {
                adicionei = true;
                GameObject.Find("GameManager").GetComponent<PhotonView>().RPC("addPlayers", RpcTarget.AllBuffered);
            }
        }

        allPlayersReady = GameObject.Find("GameManager").GetComponent<ManageGame>().playersPost == Photon.Pun.PhotonNetwork.PlayerList.Length;
    }

    IEnumerator GoToCamera()
    {
        yield return new WaitForSeconds(3f);
        monstro.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled=false;
        IrParaCamera();
        yield break;
    }

    IEnumerator PessoasEsperandoNao()
    {
        yield return null;
        if(GameObject.Find("GameManager").GetComponent<ManageGame>().playersPost == Photon.Pun.PhotonNetwork.PlayerList.Length)
        {
            monstro.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled=true;
            monstro.GetComponent<ChromaticControler>().normalMusic.gameObject.active=true;
            inCinematic=false;
            waitingForOthers.active=false;
            this.gameObject.active=false;
        }
        else
        {
            StartCoroutine(PessoasEsperandoNao());
        }
        yield break;
    }


    public void IrParaCamera()
    {
        seguir = true;
    }

    public void CinematicBack()
    {
        if(allPlayersReady)
        {
            if(Photon.Pun.PhotonNetwork.PlayerList.Length==1)
            {
                monstro.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled=true;
                monstro.GetComponent<ChromaticControler>().normalMusic.gameObject.active=true;
                inCinematic=false;
                waitingForOthers.active=false;
                this.gameObject.active=false;
            }
            else{
            waitingForOthers.active=true;
            monstro.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled=false;
                waitingForOthers.GetComponent<TMPro.TMP_Text>().text = "Waiting for other players: \n" + GameObject.Find("GameManager").GetComponent<ManageGame>().playersPost +"/"+Photon.Pun.PhotonNetwork.PlayerList.Length.ToString();
            StartCoroutine(PessoasEsperandoNao());
            }
        }
        else
        {
            waitingForOthers.active=true;
            monstro.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled=false;
            waitingForOthers.GetComponent<TMPro.TMP_Text>().text = "Waiting for other players: \n"+ GameObject.Find("GameManager").GetComponent<ManageGame>().playersPost + "/"+Photon.Pun.PhotonNetwork.PlayerList.Length.ToString();
        }
    }
}
