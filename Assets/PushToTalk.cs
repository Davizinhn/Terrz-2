using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using Photon.Voice.PUN;

public class PushToTalk : MonoBehaviourPun, IPunObservable
{
    public KeyCode PushButton = KeyCode.V;
    public Recorder VoiceRecorder;
    private PhotonView view;
    public GameObject Mic;

    public GameObject MicPlayer;

    // Start is called before the first frame update
    void Start()
    {
        Mic.SetActive(false);
        view = photonView;
        VoiceRecorder.TransmitEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(PushButton))
        {
            if (view.IsMine)
            {
                photonView.RPC("ActiveMic", RpcTarget.All);
                VoiceRecorder.TransmitEnabled = true;
            }
        }
        else if (Input.GetKeyUp(PushButton))
        {
            if (view.IsMine)
            {
                photonView.RPC("DeactiveMic", RpcTarget.All);
                VoiceRecorder.TransmitEnabled = false;
            }
        }
        
        if(view.IsMine)
        {
            Mic.GetComponent<SpriteRenderer>().enabled=false;
            MicPlayer.active = VoiceRecorder.TransmitEnabled;
        }
    }

    [PunRPC]
    void ActiveMic()
    {
        Mic.active=true;
    }

    [PunRPC]
    void DeactiveMic()
    {
        Mic.active=false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(Mic.activeSelf);
        }
        else
        {
            Mic.SetActive((bool) stream.ReceiveNext());
        }
    }

    
}

