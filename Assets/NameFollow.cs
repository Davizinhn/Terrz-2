using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameFollow : MonoBehaviour
{
    public Transform camera1;

    void Start()
    {
        camera1 = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
            camera1 = Camera.main.transform;
            transform.LookAt(transform.position + camera1.rotation * Vector3.forward, camera1.rotation * Vector3.up);
    }
}