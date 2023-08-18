using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameFollow : MonoBehaviour
{
    public Transform camera1;
    public Transform camera2;
    public Transform camera3;

    void Start()
    {
        camera1 = Camera.main.transform;
        camera3 = camera1.parent.GetChild(8).GetChild(0);
    }


    // Update is called once per frame
    void Update()
    {
        if(camera1.parent.GetComponent<FirstPersonMovement>().isDead || camera1.parent.GetComponent<FirstPersonMovement>().isEmoting)
        {
            if(camera3.parent.gameObject.active==false)
            {
                camera2 = GameObject.FindWithTag("SpectatorCam").GetComponent<Camera>().transform;
                transform.LookAt(transform.position + camera2.rotation * Vector3.forward, camera2.rotation * Vector3.up);
            }
            else
            {
                transform.LookAt(transform.position + camera3.rotation * Vector3.forward, camera3.rotation * Vector3.up);
            }
        }
        else
        {
            camera1 = Camera.main.transform;       
            transform.LookAt(transform.position + camera1.rotation * Vector3.forward, camera1.rotation * Vector3.up);

        }
    }
}