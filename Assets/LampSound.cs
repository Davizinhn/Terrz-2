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
    int maxRange = 15;
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
        else if(SceneManager.GetActiveScene().name == "Menu")
        {
            if (!jaTaIndo)
            {
                maxRange = 2;
                jaTaIndo = true;
                StartCoroutine(timerPraComecar());
            }
        }
    }

    public IEnumerator timerPraComecar()
    {
        yield return new WaitForSeconds(Random.Range(10f, 25f));
        if(view!=null)
        {
            view.RPC("LigarOuDesligar", RpcTarget.AllBuffered, audioS.isPlaying ? 0 : 1);
        }
        else
        {
            LigarOuDesligar(audioS.isPlaying ? 0 : 1);
        }
        if (Random.Range(0,maxRange) == 1)
        {
            if (view != null)
            {
                view.RPC("Sparklings", RpcTarget.AllBuffered, Random.Range(0, 2));

            }
            else
            {
                Sparklings(Random.Range(0, 2));
            }

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
