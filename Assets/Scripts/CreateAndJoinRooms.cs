using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createField;
    public TMP_InputField joinField;
    private void Start(){
        Cursor.lockState = CursorLockMode.None;
        GameObject.FindObjectOfType<DiscordPresence>().ChangePresence("In Menus", "");
    }

    public void CreateRoom()
    {
        if(GetComponent<PlayerName>().nameInpunField.text=="" || createField.text=="")
            return;
        PhotonNetwork.CreateRoom(createField.text);
    }

    public void JoinRoom()
    {
        if(GetComponent<PlayerName>().nameInpunField.text=="" || joinField.text=="")
            return;
        PhotonNetwork.JoinRoom(joinField.text);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {

    }

    public override void OnJoinedRoom()
    {        
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        PhotonNetwork.LoadLevel("Lobby");

        /*
        MenuLobby.SetActive(true);
        MenuNormal.SetActive(false);
        EstáEmSala = true;3*/
    }

    public void Ready()
    {
        this.gameObject.GetComponent<PhotonView>().RPC("AumentarOValorRPC", RpcTarget.All);
    }

    public void UnReady()
    {
        this.gameObject.GetComponent<PhotonView>().RPC("DiminuirOValorRPC", RpcTarget.All);
    }

    /*
    public void Update()
    {
        if (EstáEmSala == true)
        {
            playersName.text = PlayersConectados.ToString();
        
        
            
            if (PlayersProntos == PlayersConectados)
            {
                PhotonNetwork.LoadLevel("SampleScene");
            }

            playersProntosText.text = PlayersProntos.ToString()+" / "+PlayersConectados.ToString();
        }
        PlayersConectados = PhotonNetwork.CountOfPlayers;
    }
    
    [PunRPC]
    void AumentarOValorRPC()
    {
        PlayersProntos++;
    }
    
    void DiminuirOValorRPC()
    {
        PlayersProntos--;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(PlayersConectados);
            stream.SendNext(PlayersProntos);
        }
        else if (stream.IsReading)
        {
            PlayersProntos = (int)stream.ReceiveNext();
            PlayersConectados = (int)stream.ReceiveNext();
        }
    }
    */
}
