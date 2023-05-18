using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameFollow : MonoBehaviour
{
    public Transform camera1;
    public Transform camera2;

    void Start()
    {
        camera1 = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(camera1==null)
        {
            camera2 = GameObject.FindWithTag("SpectatorCam").GetComponent<Camera>().transform;
            transform.LookAt(transform.position + camera2.rotation * Vector3.forward, camera2.rotation * Vector3.up);
        }
        else
        {
            if (Camera.main.transform != null) { camera1 = Camera.main.transform;       transform.LookAt(transform.position + camera1.rotation * Vector3.forward, camera1.rotation * Vector3.up);}

        }
    }
}