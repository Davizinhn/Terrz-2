using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Door : MonoBehaviour
{
    public bool isOpen;
    public bool canInteract;
    public bool isMetal;
    public AudioClip abrir, fechar;
    public PhotonView view;
    public GameObject[] meshs;

    void Update()
    {
        this.gameObject.GetComponent<Animator>().SetBool("Opened", isOpen);
    }

    public void Mudar()
    {
        view.RPC("MudarRPC", RpcTarget.AllBuffered);
    }

    public void MudarEu()
    {
        if(canInteract)
        {
        canInteract=false;
        StartCoroutine(InteractVoltar());
        isOpen=!isOpen;
        PlayAudio(isOpen);
        }
    }

    [PunRPC]
    public void MudarRPC()
    {
            canInteract=false;
            StartCoroutine(InteractVoltar());
            isOpen=!isOpen;
            PlayAudio(isOpen);
    }

    public void PlayAudio(bool audio)
    {
        if(audio)
        {
            this.gameObject.GetComponent<AudioSource>().PlayOneShot(abrir);
        }
        else
        {
            this.gameObject.GetComponent<AudioSource>().PlayOneShot(fechar);
        }
    }

    IEnumerator InteractVoltar()
    {
        meshs[0].GetComponent<MeshCollider>().enabled=false;
        meshs[1].GetComponent<MeshCollider>().enabled=false;
        yield return new WaitForSeconds(1.015f);
        if(!canInteract){
                    meshs[0].GetComponent<MeshCollider>().enabled=true;
        meshs[1].GetComponent<MeshCollider>().enabled=true;
        canInteract=true;
        }
        yield break;
    }
}
