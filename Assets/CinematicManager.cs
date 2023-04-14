using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }

        allPlayersReady = GameObject.FindGameObjectsWithTag("Player").Length == Photon.Pun.PhotonNetwork.PlayerList.Length;
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
        yield return new WaitForSeconds(5f);
            monstro.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled=true;
            monstro.GetComponent<ChromaticControler>().normalMusic.gameObject.active=true;
            inCinematic=false;
            waitingForOthers.active=false;
            this.gameObject.active=false;
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
            waitingForOthers.GetComponent<TMPro.TMP_Text>().text = "Waiting for other players: \n"+GameObject.FindGameObjectsWithTag("Player").Length.ToString()+"/"+Photon.Pun.PhotonNetwork.PlayerList.Length.ToString();
            StartCoroutine(PessoasEsperandoNao());
            }
        }
        else
        {
            waitingForOthers.active=true;
            monstro.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled=false;
            waitingForOthers.GetComponent<TMPro.TMP_Text>().text = "Waiting for other players: \n"+GameObject.FindGameObjectsWithTag("Player").Length.ToString()+"/"+Photon.Pun.PhotonNetwork.PlayerList.Length.ToString();
        }
    }
}
