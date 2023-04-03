using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerName : MonoBehaviour
{
    public TMP_InputField nameInpunField;
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("PlayerName"))
        {
            return;
        }
        else
        {
            string PlayerName = PlayerPrefs.GetString("PlayerName");
            nameInpunField.text = PlayerName;
        }
    }
    public void PlacePlayerName()
    {
        string PlayerNickname = nameInpunField.text;
        PhotonNetwork.NickName = PlayerNickname;
        PlayerPrefs.SetString("PlayerName", PlayerNickname);
    }
}