using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Generator : MonoBehaviour
{
    public bool Ativada;

    public void Update()
    {
        this.gameObject.GetComponent<Animator>().SetBool("Ativado", Ativada);
        if(Ativada)
        {
            this.gameObject.GetComponent<AudioSource>().volume=0.15f;
        }
        else
        {
            this.gameObject.GetComponent<AudioSource>().volume=0f;
        }
    }

    public void Mudar()
    {
        this.gameObject.GetComponent<PhotonView>().RPC("Trocar", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void Trocar()
    {
        Ativada = !Ativada;
    }
}
