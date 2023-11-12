using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Nominho : MonoBehaviour
{
    public float maxDistance;
    public Transform curCam;

    void Update()
    {
        curCam = GameObject.FindObjectOfType<LobbyManager>() != null ? GameObject.FindObjectOfType<LobbyManager>().curCam : GameObject.FindObjectOfType<ManageGame>().curCam;
        if (curCam != null)
        {
            this.gameObject.GetComponent<Animator>().SetBool("Ativado", Vector3.Distance(curCam.position, this.gameObject.transform.position) < maxDistance);
        }
        else
        {
            this.gameObject.GetComponent<Animator>().SetBool("Ativado", true);
        }

    }
}
