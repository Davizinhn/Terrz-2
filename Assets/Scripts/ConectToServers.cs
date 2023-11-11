using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.Networking;

public class ConectToServers : MonoBehaviourPunCallbacks
{
    public string thisVersion;
    public string gitHubVersion;
    public bool debug = false;
    public GameObject loader;
    public GameObject textinho;
    public GameObject Erro;
    private string url = "https://raw.githubusercontent.com/Davizinhn/Terrz-2/sem-ragdoll/gameVersion.txt";

    // Start is called before the first frame update
    void Start()
    {
        thisVersion = Application.version;
        if(!debug)
            StartCoroutine(GetTextFromGitHub());
        else
            PhotonNetwork.ConnectUsingSettings();
    }

    IEnumerator GetTextFromGitHub()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string text = request.downloadHandler.text;
                gitHubVersion = text;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Erro.active=true;
                loader.active=false;
            }
        }
    }

    public void VoltarErro()
    {
        Erro.active=false;
        loader.active=true;
        StartCoroutine(GetTextFromGitHub());
    }

    public void Quitar()
    {
        Application.Quit();
    }

    public override void OnConnectedToMaster()
    {
        if(!debug)
            if(thisVersion == gitHubVersion)
            {
                PhotonNetwork.JoinLobby();
            }
            else{
                textinho.active=true;
                loader.active=false;
            }
        else
            PhotonNetwork.JoinLobby();

    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Menu");
    }
}
