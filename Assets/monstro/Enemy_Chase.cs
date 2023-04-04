using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Random = UnityEngine.Random;

//using Random = System.Random;

public class Enemy_Chase : MonoBehaviour
{
    public Transform vision;
    public LayerMask collisionLayer;

    void Start(){

    }

    void Update () {   
        RaycastHit hit;
        Ray ray = new Ray(vision.transform.position, vision.transform.forward);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, collisionLayer))
        {
            Debug.DrawLine(vision.transform.position, vision.transform.forward, Color.green, 1f);
        }
    }

    
}
