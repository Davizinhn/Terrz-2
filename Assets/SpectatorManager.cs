using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEditor;
using Cinemachine;
using Random = UnityEngine.Random;
using UnityEngine.Rendering.PostProcessing;

public class SpectatorManager : MonoBehaviour
{
    public bool Spectator = false;
    public GameObject SpectatorCam;
    public GameObject campadrao;
    public CinemachineFreeLook spectatorCine;
    [SerializeField] public SpectatorMePls[] players;
    public GameObject currentPlayer;
    public int playersMortos;
    int i;

    private void Start()
    {
        currentPlayer = players[0].gameObject;
                if(PlayerPrefs.GetInt("PostProcess") == 0)
        {
            campadrao.GetComponent<PostProcessVolume>().enabled=false;
        }
    }

    void Update()
    {
        if(playersMortos==players.Length-1 || players.Length==1){GameObject.Find("GameManager").GetComponent<ManageGame>().AcabarJogoo();}
        players = GameObject.FindObjectsOfType<SpectatorMePls>();

        if (Spectator)
        {
            SpectatorCam.SetActive(true);
            if(PlayerPrefs.GetInt("PostProcess") == 0 && campadrao.GetComponent<PostProcessVolume>().enabled==true)
            {
                campadrao.GetComponent<PostProcessVolume>().enabled=false;
            }
            Destroy(GameObject.FindWithTag("MainCamera"));
            Cursor.lockState = CursorLockMode.Locked;

            spectatorCine.Follow = currentPlayer.transform;
            spectatorCine.LookAt = currentPlayer.transform;
        }

        if (Spectator && Input.GetKeyDown(KeyCode.D))
        {
            i++;
            if (i > players.Length-1)
            {
                i = 0;
            }
            if (i < 0)
            {
                i = players.Length;
            }
            currentPlayer = players[i].gameObject;
            spectatorCine.Follow = currentPlayer.transform;
            spectatorCine.LookAt = currentPlayer.transform;
        }
        if (Spectator && Input.GetKeyDown(KeyCode.A))
        {
            i--;
            if (i > players.Length-1)
            {
                i = 0;
            }
            if (i < 0)
            {
                i = players.Length;
            }
            currentPlayer = players[i].gameObject;
            spectatorCine.Follow = currentPlayer.transform;
            spectatorCine.LookAt = currentPlayer.transform;
        }
    }
    
}
