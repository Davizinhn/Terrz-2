using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;

public class Generator : MonoBehaviour
{
    public bool Ativada;
    public bool Quebrado;
    public bool canInteract = true;
    public int quebradoPoints = 0;
    public bool modoSegurar = true;
    public GameObject quebradoStuff;
    public ParticleSystem explosion;
    public LayerMask lm;

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

    public void TocarExplosion()
    {
        this.gameObject.GetComponent<PhotonView>().RPC("Explosao", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void MudarModo(bool segurar = false)
    {
        modoSegurar = segurar;
    }

    [PunRPC]
    public void Explosao()
    {
        modoSegurar = true;
        canInteract = false;
        StartCoroutine(CanInteractBack());
        ParticleSystem exp = Instantiate(explosion, this.gameObject.transform.position, Quaternion.identity);
        GameObject.FindWithTag("MainCamera").transform.DOShakePosition(1.8f, Vector3.Distance(this.transform.position, Camera.main.transform.position)<=15?0.8f:0f, 3, 29, false, true, ShakeRandomnessMode.Harmonic);
    }

    IEnumerator CanInteractBack()
    {
        yield return new WaitForSeconds(3f);
        canInteract=true;
        yield break;
    }

    [PunRPC]
    public void Quebrar()
    {
        modoSegurar = true;
        Quebrado = true;
        Ativada=false;
    }

    public void Mudar()
    {
        if (!canInteract)
            return;
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
        if(Quebrado && !modoSegurar)
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
