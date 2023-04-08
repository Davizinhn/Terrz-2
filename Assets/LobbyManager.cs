using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{

    public TMP_Text text;
    public void InitGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen=false;
        StartCoroutine(startTimer());
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Menu");
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
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
        this.gameObject.GetComponent<PhotonView>().RPC("Teleportar", RpcTarget.All);
    }

    [PunRPC]
    public void Teleportar()
    {
        PhotonNetwork.LoadLevel("Teste");
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }
}
