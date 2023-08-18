using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LampSound : MonoBehaviour
{
    public PhotonView view;
    public AudioSource audioS;
    public GameObject[] quais;
    bool jaTaIndo = false;

    public void Update()
    {
        if(SceneManager.GetActiveScene().name=="Game")
        {
            if(PhotonNetwork.IsMasterClient)
            {
                if(!jaTaIndo)
                {
                    jaTaIndo=true;
                    StartCoroutine(timerPraComecar());
                }
            }
        }
    }

    public IEnumerator timerPraComecar()
    {
        yield return new WaitForSeconds(Random.Range(10f, 25f));
        view.RPC("LigarOuDesligar", RpcTarget.AllBuffered, audioS.isPlaying?0:1);
        if(Random.Range(0,15) == 1)
        {
        view.RPC("Sparklings", RpcTarget.AllBuffered, Random.Range(0,2));
        }
        jaTaIndo=false;
        StopCoroutine(timerPraComecar());
    }

    [PunRPC]
    public void LigarOuDesligar(int a = 0)
    {
        if(a==0)
        {
            audioS.Stop();
        }
        else
        {
            audioS.Play();
        }
    }

    [PunRPC]
    public void Sparklings(int qual)
    {
        quais[qual].SetActive(true);
    }


}
