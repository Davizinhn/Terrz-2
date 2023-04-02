using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public Camera thisCamera;
    public LayerMask collisionLayer;
    public float distance = 2f;

    public void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(thisCamera.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out hit, distance, collisionLayer))
        {
            if (hit.transform.gameObject.tag == "Door")
            {
                if(hit.transform.gameObject.GetComponent<Door>().canInteract)
                {
                    if(Input.GetKeyDown(KeyCode.E))
                    {
                        hit.transform.gameObject.GetComponent<Door>().Mudar();
                    }
                }
            }
        }
    }
}
