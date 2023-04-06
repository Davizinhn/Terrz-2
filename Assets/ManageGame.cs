using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ManageGame : MonoBehaviour
{
    public GameObject acabouText;
    public Door terminarDoor;
    public bool isacabando;
    public bool foge;
        public Generator[] allGen;
        public bool ativados;

    public void Awake()
    {
                allGen = FindObjectsOfType<Generator>();
    }

    public void Update()
    {
        foreach(Generator a in allGen)
        {
            ativados=true;
            if (!a.Ativada)
            {
                ativados = false;
                break;
            }
        }
        if(ativados && !terminarDoor.isOpen)
        {
            terminarDoor.Mudar();
        }
        foge = terminarDoor.isOpen && ativados;
    }

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
