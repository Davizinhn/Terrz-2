using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Name : MonoBehaviourPun
{
    public TMP_Text nametext;
    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            nametext.text = photonView.Owner.NickName;
        }
        
        
        if (photonView.IsMine)
        {
            Destroy(nametext.gameObject);
        }
        
    }
}