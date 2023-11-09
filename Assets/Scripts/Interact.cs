using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;


public class Interact : MonoBehaviour
{
    public Camera thisCamera;
    public LayerMask collisionLayer;
    public float distance = 2f;

    public GameObject QuebradoStuff;
        public TMP_Text quebradoText;
    public CollisionCheckArrow arrow;
    public RectTransform Coiso;

    PhotonView view;
    public TMP_Text cue;
    public AudioSource tick;
        Vector3 hahi;
    public ManageGame gameManager;


    void Start()
    {
        view = this.gameObject.GetComponent<PhotonView>();
        Coiso.anchoredPosition = new Vector3(Random.RandomRange(-339f, 340), -159f, 0);
            if(SceneManager.GetActiveScene().name=="Game")
            {
            gameManager = GameObject.FindObjectOfType<ManageGame>();
            }
    }

    public void Update()
    {
        if(view.IsMine && !this.gameObject.GetComponent<FirstPersonMovement>().isDead && !this.gameObject.GetComponent<FirstPersonMovement>().hasFallen && (gameManager!=null ? !gameManager.isPaused : true) && !this.gameObject.GetComponent<FirstPersonMovement>().isEmoting)
        {
            RaycastHit hit;
            if (Camera.main != null) { Ray ray = new Ray(thisCamera.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out hit, distance, collisionLayer) && !this.gameObject.GetComponent<FirstPersonMovement>().isDead && !this.gameObject.GetComponent<FirstPersonMovement>().isEmoting)
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
                            tick.Play();
                            hit.transform.gameObject.GetComponent<Generator>().Mudar();
                            Coiso.anchoredPosition = new Vector3(Random.RandomRange(-339f, 340), -159f, 0);
                    }
                    else if(Input.GetKeyDown(KeyCode.E) && QuebradoStuff.active && !arrow.Pode)
                    {
                        hit.transform.gameObject.GetComponent<Generator>().TocarExplosion();
                        hahi = this.gameObject.transform.position;
                        view.RPC("Atrair", RpcTarget.AllViaServer, hit.transform.gameObject.name);
                    }
                    if(QuebradoStuff.active)
                    {
                        quebradoText.text = hit.transform.gameObject.GetComponent<Generator>().quebradoPoints.ToString()+"/10";
                    }
                    QuebradoStuff.active=hit.transform.gameObject.GetComponent<Generator>().Quebrado && hit.transform.gameObject.GetComponent<Generator>().canInteract;
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
                else if (hit.transform.gameObject.tag == "Bed")
                {
                    cue.text="Hide";
                    if(Input.GetKeyDown(KeyCode.E))
                    {
                        if (!this.GetComponent<FirstPersonMovement>().isLaying)
                        { 
                            hit.transform.gameObject.GetComponent<PhotonView>().RPC("LayHere", RpcTarget.AllBuffered, this.gameObject.GetComponent<PhotonView>().ViewID);
                        }
                    }
                }
                else if(hit.transform.gameObject.tag == "PlayerCaido")
                    {
                        QuebradoStuff.active = false;
                        cue.text = "Help";
                        if (hit.transform.gameObject != this.gameObject)
                        {
                            if (Input.GetKeyDown(KeyCode.E))
                            {
                                hit.transform.gameObject.GetComponent<PhotonView>().RPC("LevantarPlayer", RpcTarget.AllBuffered);
                            }
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
        else
        {
            cue.text="";
            if(QuebradoStuff!=null)
            {
                QuebradoStuff.active = false;

            }
        }
    }

    [PunRPC]
    public void Atrair(string ha)
    {
        GameObject.FindObjectOfType<EnemyAI>().AvisarGenerator(GameObject.Find(ha).transform);
    }
}
