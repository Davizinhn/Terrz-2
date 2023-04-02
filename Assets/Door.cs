using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen;
    public bool canInteract;
    public AudioClip abrir, fechar;

    void Update()
    {
        this.gameObject.GetComponent<Animator>().SetBool("Opened", isOpen);
    }

    public void Mudar()
    {
        if(canInteract)
        {
            canInteract=false;
            StartCoroutine(InteractVoltar());
            isOpen=!isOpen;
            PlayAudio(isOpen);
        }
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
        canInteract=true;
        yield break;
    }
}
