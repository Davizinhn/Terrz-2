using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public Vector3 max;
    public Vector3 min;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(Random.RandomRange(min.x, max.x), Random.RandomRange(min.y, max.y), Random.RandomRange(min.z, max.z)), Quaternion.identity);
    }
}
