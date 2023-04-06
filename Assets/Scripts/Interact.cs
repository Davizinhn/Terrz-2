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

    public GameObject QuebradoStuff;
        public TMP_Text quebradoText;
    public CollisionCheckArrow arrow;

    PhotonView view;
    public TMP_Text cue;

    void Start()
    {
        view = this.gameObject.GetComponent<PhotonView>();
    }

    public void Update()
    {
        if(view.IsMine && !this.gameObject.GetComponent<FirstPersonMovement>().isDead)
        {
            RaycastHit hit;
            Ray ray = new Ray(thisCamera.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out hit, distance, collisionLayer) && !this.gameObject.GetComponent<FirstPersonMovement>().isDead)
            {
                if (hit.transform.gameObject.tag == "Door" && !hit.transform.gameObject.GetComponent<Door>().isMetal)
                {            QuebradoStuff.active=false;
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
                    if(Input.GetKeyDown(KeyCode.E) && !QuebradoStuff.active && !GameObject.Find("GameManager").GetComponent<ManageGame>().foge)
                    {
                            hit.transform.gameObject.GetComponent<Generator>().Mudar();
                    }
                    else if(Input.GetKeyDown(KeyCode.E) && QuebradoStuff.active && arrow.Pode)
                    {
                            hit.transform.gameObject.GetComponent<Generator>().Mudar();
                    }
                    if(QuebradoStuff.active)
                    {
                        quebradoText.text = hit.transform.gameObject.GetComponent<Generator>().quebradoPoints.ToString()+"/5";
                    }
                    QuebradoStuff.active=hit.transform.gameObject.GetComponent<Generator>().Quebrado;
                }
                else if (hit.transform.gameObject.tag == "Button" && PhotonNetwork.IsMasterClient)
                {            QuebradoStuff.active=false;
                    cue.text="Button";
                    if(Input.GetKeyDown(KeyCode.E) && hit.transform.gameObject.GetComponent<FirstPersonButton>().canPress)
                    {
                            hit.transform.gameObject.GetComponent<FirstPersonButton>().Pressing();
                    }
                }
                else if (hit.transform.gameObject.tag == "OpenButton")
                {            QuebradoStuff.active=false;
                    cue.text="Button";
                    if(Input.GetKeyDown(KeyCode.E) && hit.transform.gameObject.GetComponent<FirstPersonButton>().canPress)
                    {
                            hit.transform.gameObject.GetComponent<FirstPersonButton>().Pressing(true);
                    }
                }
                else
                {
                                QuebradoStuff.active=false;
                    cue.text="";
                }
            }
            else
            {
                            QuebradoStuff.active=false;
                cue.text="";
            }
        }
    }
}
