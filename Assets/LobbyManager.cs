using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class LobbyManager : MonoBehaviour
{

    public TMP_Text text;
    public void InitGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen=false;
        StartCoroutine(startTimer());
    }

    IEnumerator startTimer()
    {
        yield return new WaitForSeconds(1f);
        text.text="3";
        yield return new WaitForSeconds(1f);
        text.text="2";
        yield return new WaitForSeconds(1f);
        text.text="1";
        yield return new WaitForSeconds(1f);
        PhotonNetwork.LoadLevel("Teste");
    }
}
