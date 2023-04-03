using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public Transform Spawn;

    public Generator[] allGen;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, Spawn.position, Quaternion.identity);
        allGen = FindObjectsOfType<Generator>();
    }

    void Update()
    {

    }
}
