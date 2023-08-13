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
    public GameObject loader;
    public GameObject textinho;
    public GameObject Erro;
    private string url = "https://raw.githubusercontent.com/yourusername/yourrepository/main/yourfile.txt";

    // Start is called before the first frame update
    void Start()
    {
        thisVersion = Application.version;
        StartCoroutine(GetTextFromGitHub());
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
        if(thisVersion == gitHubVersion)
        {
            PhotonNetwork.JoinLobby();
        }
        else{
            textinho.active=true;
            loader.active=false;
        }
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Menu");
    }
}
