using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameFollow : MonoBehaviour
{
    public Nominho nomim;

    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + nomim.curCam.rotation * Vector3.forward, nomim.curCam.rotation * Vector3.up);
    }
}