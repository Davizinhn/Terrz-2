using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class FirstPersonButton : MonoBehaviour
{
    [Header("Event")]
    public UnityEngine.UI.Button.ButtonClickedEvent eventToExecute = new UnityEngine.UI.Button.ButtonClickedEvent();

    [Header("Settings")]
    public bool canPress;
    bool isPressing;
    bool isHitting;
    public enum Type
    {
        CanBePressedMultipleTimes,
        CanBePressedOneTime,
    }
    public Type type;
    public float distance = 2f;
    public float timeOut = 2f;
    public bool isLocked;

    public void Update()
    {
        canPress=!isLocked&&!isPressing;
    }

    public void ForeverLock()
    {
        isLocked = true;
    }

    public void Pressing()
    {
        this.gameObject.GetComponent<PhotonView>().RPC("PressRPC", RpcTarget.AllBuffered);
    }


    [PunRPC]
    public void PressRPC()
    {
        if(canPress)
        {
            isPressing=true;
            this.gameObject.GetComponent<AudioSource>().Play();
            this.gameObject.GetComponent<Animator>().SetTrigger("Press");
            eventToExecute.Invoke();
            StartCoroutine(Pressing1());
        }
    }

    public IEnumerator Pressing1()
    {
        yield return new WaitForSeconds(timeOut);
        if(type==Type.CanBePressedMultipleTimes && isPressing==true)
        {
        isPressing=false;
        }else{isPressing=false; isLocked=true;}
        StopAllCoroutines();
    }

}