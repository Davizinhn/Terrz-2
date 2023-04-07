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

    public string curPersona;
    public SkinnedMeshRenderer[] leonard;
    public SkinnedMeshRenderer[] megan;
    // Start is called before the first frame update
    void Start()
    {
                curPersona =  PlayerPrefs.GetString("curPersona");
        if(curPersona=="megan")
        {
            foreach (SkinnedMeshRenderer a in megan)
            {
                a.enabled=true;
            }
            foreach (SkinnedMeshRenderer a in leonard)
            {
                a.enabled=false;
            }
        }
        else
        {
            foreach (SkinnedMeshRenderer a in megan)
            {
                a.enabled=false;
            }
            foreach (SkinnedMeshRenderer a in leonard)
            {
                a.enabled=true;
            }
        }
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

    public void Update()
    {
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
        curPersona =  PlayerPrefs.GetString("curPersona");
        if(curPersona=="megan")
        {
            foreach (SkinnedMeshRenderer a in megan)
            {
                a.enabled=true;
            }
            foreach (SkinnedMeshRenderer a in leonard)
            {
                a.enabled=false;
            }
        }
        else
        {
            foreach (SkinnedMeshRenderer a in megan)
            {
                a.enabled=false;
            }
            foreach (SkinnedMeshRenderer a in leonard)
            {
                a.enabled=true;
            }
        }
    }
}