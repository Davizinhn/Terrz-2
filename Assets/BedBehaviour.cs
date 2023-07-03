using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BedBehaviour : MonoBehaviour
{
    public bool isSomeoneHere = false;
    public Transform spot;
    public Transform unspot;
    public Transform CameraPos;
    public Transform spotMonstro;

    [PunRPC]
    public void LayHere(int playerID)
    {
        GameObject playerToLay = PhotonView.Find(playerID).gameObject;
        if(!isSomeoneHere)
        {
            isSomeoneHere = true;
            playerToLay.GetComponent<FirstPersonMovement>().isLaying = true;
            playerToLay.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            playerToLay.GetComponent<CapsuleCollider>().enabled = false;
            playerToLay.GetComponent<FirstPersonMovement>().curBed = this;
            //playerToLay.GetComponent<FirstPersonMovement>().camera.transform.position = CameraPos.position;
            //playerToLay.GetComponent<FirstPersonMovement>().camera.transform.position = new Vector3(playerToLay.GetComponent<FirstPersonMovement>().camera.transform.position.x, 0.26f, playerToLay.GetComponent<FirstPersonMovement>().camera.transform.position.z);
            playerToLay.transform.position = spot.position;

        }
    }

    [PunRPC]
    public void unLayHere(int playerID)
    {
        GameObject playerToLay = PhotonView.Find(playerID).gameObject;
        playerToLay.transform.position = unspot.position;
            isSomeoneHere = false;
            playerToLay.GetComponent<FirstPersonMovement>().isLaying = false;
            playerToLay.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            playerToLay.GetComponent<CapsuleCollider>().enabled = true;
            playerToLay.GetComponent<FirstPersonMovement>().curBed = null;
        //playerToLay.GetComponent<FirstPersonMovement>().camera.transform.position = new Vector3(playerToLay.GetComponent<FirstPersonMovement>().camera.transform.position.x, 2.126f, playerToLay.GetComponent<FirstPersonMovement>().camera.transform.position.z);
    }

    [PunRPC]
    public void beBackFromThingie()
    {
        if(isSomeoneHere){isSomeoneHere = false;}
    }
}
