using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    public TMP_Text text;
    public TMP_Text players;
    public TMP_Text roomName;
    public GameObject rulesTab;
    public Transform rulesTransform;
    public List<RoomRule> roomRules = new List<RoomRule>();
    public GameObject optionPrefab;
    public bool isMatchStarting = false;
    public GameObject masterClientTxt;
    public bool rulesOpen = false;
    public Transform curCam;
    bool isMasterClient;

    public void Start()
    {
        isMasterClient = PhotonNetwork.IsMasterClient;
        roomName.text = "Room Name: \n" + PhotonNetwork.CurrentRoom.Name;
        masterClientTxt.SetActive(isMasterClient);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;
        GameObject.FindObjectOfType<DiscordPresence>().ChangePresence("In Lobby", "");
        if(isMasterClient)
        {
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(roomRules[0].Name, out object ha);
            if (ha == null)
            {
                UpdatePhotonProperties();
            }
            else
            {
                foreach(RoomRule rule in roomRules)
                {
                    PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(rule.Name, out object valor);
                    rule.value = (int)valor;
                }
            }
            foreach(RoomRule rule in roomRules)
            {
                GameObject ruleButton = Instantiate(optionPrefab, rulesTransform);
                RuleButton ruleB = ruleButton.GetComponent<RuleButton>();
                ruleB.name.text = rule.Name;
                ruleB.dropdown.ClearOptions();
                ruleB.dropdown.AddOptions(rule.Options);
                ruleB.dropdown.onValueChanged.AddListener(delegate { UpdateRoomRules(ruleB.name.text, int.Parse(ruleB.dropdown.options[ruleB.dropdown.value].text)); });
                for (int i = 0; i < ruleB.dropdown.options.Count; i++)
                {
                    if (ruleB.dropdown.options[i].text == rule.value.ToString())
                    {
                        ruleB.dropdown.value = i;
                    }
                }
            }
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        Start();
    }

    public void UpdatePhotonProperties()
    {
        ExitGames.Client.Photon.Hashtable custom = new ExitGames.Client.Photon.Hashtable();
        foreach (RoomRule rule in roomRules)
        {
            if(!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(rule.Name))
            {
                PhotonNetwork.CurrentRoom.CustomProperties.Add(rule.Name, rule.value);
            }
            else
            {
                custom.Add(rule.Name, rule.value);
            }
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(custom);
    }

    public void InitGame()
    {
        isMatchStarting = true;
        PhotonNetwork.CurrentRoom.IsOpen=false;
        StartCoroutine(startTimer());
    }

    public void Update()
    {
        players.text=PhotonNetwork.PlayerList.Length.ToString()+"/4";
        if(!rulesOpen && isMasterClient && !isMatchStarting)
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                Cursor.lockState = CursorLockMode.None;
                rulesOpen = true;
                rulesTab.SetActive(true);
                foreach(FirstPersonMovement fpc in GameObject.FindObjectsOfType<FirstPersonMovement>())
                {
                    if (fpc.gameObject.GetComponent<PhotonView>().IsMine)
                    {
                        fpc.TirarAnimsLobby();
                        break;
                    }
                }
            }
        }
        else if (rulesOpen && isMasterClient && !isMatchStarting)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Cursor.lockState = CursorLockMode.Locked;
                rulesOpen = false;
                rulesTab.SetActive(false);
            }
        }
    }

    public void UpdateRoomRules(string Name, int newValue)
    {
        foreach(RoomRule a in roomRules)
        {
            if(a.Name == Name)
            {
                a.value = newValue;
                break;
            }
        }
        UpdatePhotonProperties();
    }

    public void LeaveRoom()
    {
        if(!isMatchStarting)
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            SceneManager.LoadScene("Menu");
        }
    }

    IEnumerator startTimer()
    {
        yield return new WaitForSeconds(1f);
        text.text="3";
        yield return new WaitForSeconds(1f);
        text.text="2";
        yield return new WaitForSeconds(1f);
        text.text="1";
        yield return new WaitForSeconds(1f);
        this.gameObject.GetComponent<PhotonView>().RPC("Teleportar", RpcTarget.All);
    }

    [PunRPC]
    public void Teleportar()
    {        
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        PhotonNetwork.LoadLevel("Game");
    }
}

[System.Serializable]
public class RoomRule
{
    public string Name;
    public int value;
    public List<TMP_Dropdown.OptionData> Options;
}