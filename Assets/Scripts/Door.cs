using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Door : MonoBehaviour
{
    public bool isOpen;
    public bool canInteract;
    public AudioClip abrir, fechar;
    public PhotonView view;

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
        yield return new WaitForSeconds(2f);
        if(!canInteract){
        canInteract=true;
        }
        yield break;
    }
}
