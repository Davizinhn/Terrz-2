using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;
using DG.Tweening.Core;
using DG;
using DG.Tweening.Plugins;

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
        monstro = GameObject.Find("Monster");
        music.Play();
        monstro.GetComponent<ChromaticControler>().normalMusic.gameObject.active=false;
        StartCoroutine(GoToCamera());
    }

    void LateUpdate() {
        targetPosition = Camera.main.gameObject.transform.position;
        targetRotation = Camera.main.gameObject.transform.rotation;

        if(this.gameObject.transform.position == targetPosition)
        {
            CinematicBack();
            if(!adicionei)
            {
                adicionei = true;
                Invoke("AdicionaPlayer", 0.5f);
            }
        }

        allPlayersReady = GameObject.Find("GameManager").GetComponent<ManageGame>().playersPost == Photon.Pun.PhotonNetwork.PlayerList.Length;
    }

    void AdicionaPlayer()
    {
        GameObject.Find("GameManager").GetComponent<PhotonView>().RPC("addPlayers", RpcTarget.AllBuffered);
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
        transform.DOMove(targetPosition, 1.15f).SetEase(Ease.OutSine);
        transform.DORotateQuaternion(targetRotation, 1f).SetEase(Ease.InOutSine);
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

    public void VoltarTudo()
    {

    }
}
