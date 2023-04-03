using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Interact : MonoBehaviour
{
    public Camera thisCamera;
    public LayerMask collisionLayer;
    public float distance = 2f;

    PhotonView view;
    public TMP_Text cue;

    void Start()
    {
        view = this.gameObject.GetComponent<PhotonView>();
    }

    public void Update()
    {
        if(view.IsMine)
        {
            RaycastHit hit;
            Ray ray = new Ray(thisCamera.transform.position, Camera.main.transform.forward);

            if (Physics.Raycast(ray, out hit, distance, collisionLayer))
            {
                if (hit.transform.gameObject.tag == "Door")
                {
                    if(hit.transform.gameObject.GetComponent<Door>().canInteract)
                    {
                        cue.text="Door";
                        if(Input.GetKeyDown(KeyCode.E))
                        {
                            hit.transform.gameObject.GetComponent<Door>().Mudar();
                        }
                    }
                    else
                    {
                        cue.text="";
                    }
                }
                else if (hit.transform.gameObject.tag == "Generator")
                {
                    cue.text="Generator";
                    if(Input.GetKeyDown(KeyCode.E))
                    {
                            hit.transform.gameObject.GetComponent<Generator>().Mudar();
                    }
                }
                else
                {
                    cue.text="";
                }
            }
            else
            {
                cue.text="";
            }
        }
    }
}
