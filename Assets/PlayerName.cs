using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerName : MonoBehaviour
{
    public TMP_InputField nameInpunField;
    public TMP_Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("curPersona"))
        {
            if(dropdown.value==0){
            PlayerPrefs.SetString("curPersona", "leonard");
            }else
            {
                        PlayerPrefs.SetString("curPersona", "megan");
            }
        }
        else
        {
            if(PlayerPrefs.GetString("curPersona") == "megan")
            {
                dropdown.value=1;
            }else
            {
                dropdown.value=0;
            }
        }
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

    public void PlaceSkin()
    {
        if(dropdown.value==0){
        PlayerPrefs.SetString("curPersona", "leonard");
        }else
        {
                    PlayerPrefs.SetString("curPersona", "megan");
        }
    }
}