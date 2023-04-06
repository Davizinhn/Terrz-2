using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Generator : MonoBehaviour
{
    public bool Ativada;
    public bool Quebrado;
    public int quebradoPoints = 0;
    public GameObject quebradoStuff;

    public void Update()
    {
        quebradoStuff.SetActive(Quebrado);
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

    public void QuebrarNormie()
    {
        if(!GameObject.Find("GameManager").GetComponent<ManageGame>().foge)
        {
            this.gameObject.GetComponent<PhotonView>().RPC("Quebrar", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void Quebrar()
    {
        Quebrado=true;
        Ativada=false;
    }

    public void Mudar()
    {
        if(!Quebrado)
        {
            this.gameObject.GetComponent<PhotonView>().RPC("Trocar", RpcTarget.AllBuffered);
        }
        else
        {
            this.gameObject.GetComponent<PhotonView>().RPC("Consertar", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void Trocar()
    {
        Ativada = !Ativada;
    }

    [PunRPC]
    public void Consertar()
    {
        if(Quebrado)
        {
            quebradoPoints++;
            if(quebradoPoints>=5)
            {
                quebradoPoints=0;
                Quebrado=false;
            }
        }
    }
}
