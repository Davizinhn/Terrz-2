using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ManageGame : MonoBehaviour
{
    public GameObject acabouText;
    bool isacabando;

    public void AcabarJogoo()
    {
        if(!isacabando)
        {
            isacabando=true;
        this.gameObject.GetComponent<PhotonView>().RPC("AcabarJogo", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void AcabarJogo()
    {
        StartCoroutine(acabarGame());
        acabouText.active=true;
    }

    IEnumerator acabarGame()
    {
        yield return new WaitForSeconds(3f);
        PhotonNetwork.LoadLevel("Lobby");
        PhotonNetwork.CurrentRoom.IsOpen=true;
        SceneManager.UnloadSceneAsync("Teste");
        yield break;
    }
}
