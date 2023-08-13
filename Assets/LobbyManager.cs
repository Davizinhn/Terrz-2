using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{

    public TMP_Text text;
    public TMP_Text players;

    public void Start()
    {
        GameObject.FindObjectOfType<DiscordPresence>().ChangePresence("In Lobby", "");
    }
    public void InitGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen=false;
        StartCoroutine(startTimer());
    }

    public void Update()
    {
        players.text=PhotonNetwork.PlayerList.Length.ToString()+"/4";
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadScene("Menu");
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
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        PhotonNetwork.LoadLevel("Game");
    }
}
