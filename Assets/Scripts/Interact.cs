using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Interact : MonoBehaviour
{
    public Camera thisCamera;
    public LayerMask collisionLayer;
    public float distance = 2f;

    public GameObject QuebradoStuff;
        public TMP_Text quebradoText;
    public CollisionCheckArrow arrow;
    public RectTransform Coiso;
    public GameObject sliderGenerator;
    public GameObject[] apertarGenerator;

    public GameObject ajudarUI;
    public Slider ajudarSlider;

    PhotonView view;
    public TMP_Text cue;
    public AudioSource tick;
        Vector3 hahi;
    public ManageGame gameManager;
    LobbyManager lobbyManager;


    void Start()
    {
        view = this.gameObject.GetComponent<PhotonView>();
        Coiso.anchoredPosition = new Vector3(Random.RandomRange(-339f, 340), -159f, 0);
            if(SceneManager.GetActiveScene().name=="Game")
            {
            gameManager = GameObject.FindObjectOfType<ManageGame>();
            }
            else
            {
            lobbyManager = GameObject.FindObjectOfType<LobbyManager>();
            }
    }

    public void Update()
    {
        if(view.IsMine && !this.gameObject.GetComponent<FirstPersonMovement>().isDead && !this.gameObject.GetComponent<FirstPersonMovement>().hasFallen && (gameManager!=null ? !gameManager.isPaused : !lobbyManager.rulesOpen) && !this.gameObject.GetComponent<FirstPersonMovement>().isEmoting)
        {
            RaycastHit hit;
            if (Camera.main != null) { Ray ray = new Ray(thisCamera.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out hit, distance, collisionLayer) && !this.gameObject.GetComponent<FirstPersonMovement>().isDead && !this.gameObject.GetComponent<FirstPersonMovement>().isEmoting)
            {
                if (hit.transform.gameObject.tag == "Door" && !hit.transform.gameObject.GetComponent<Door>().isMetal)
                {            QuebradoStuff.active=false;
                        ajudarSlider.value = 0;
                        sliderGenerator.GetComponent<Slider>().value = 0;

                        ajudarUI.SetActive(false);
                        if (hit.transform.gameObject.GetComponent<Door>().canInteract)
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
                        ajudarSlider.value = 0;
                        ajudarUI.SetActive(false);
                        cue.text="Generator";
                        if (QuebradoStuff.active)
                        {
                            if (hit.transform.gameObject.GetComponent<Generator>().modoSegurar)
                            {
                                if (Input.GetKey(KeyCode.E) && sliderGenerator.activeSelf == true)
                                {
                                    StartCoroutine(encher1());
                                }
                                else if (!Input.GetKey(KeyCode.E))
                                {
                                    StopAllCoroutines();
                                    sliderGenerator.GetComponent<Slider>().value = 0;
                                }
                                if (sliderGenerator.GetComponent<Slider>().value == 1)
                                {
                                    sliderGenerator.GetComponent<Slider>().value = 0;
                                    hit.transform.gameObject.GetComponent<PhotonView>().RPC("MudarModo", RpcTarget.AllBuffered, false);
                                }
                            }
                            else
                            {
                                if (Input.GetKeyDown(KeyCode.E) && !QuebradoStuff.active && !GameObject.Find("GameManager").GetComponent<ManageGame>().foge)
                                {
                                    hit.transform.gameObject.GetComponent<Generator>().Mudar();
                                }
                                else if (Input.GetKeyDown(KeyCode.E) && QuebradoStuff.active && arrow.Pode)
                                {
                                    tick.Play();
                                    hit.transform.gameObject.GetComponent<Generator>().Mudar();
                                    Coiso.anchoredPosition = new Vector3(Random.RandomRange(-339f, 340), -159f, 0);
                                    hit.transform.gameObject.GetComponent<PhotonView>().RPC("MudarModo", RpcTarget.AllBuffered, true);
                                }
                                else if (Input.GetKeyDown(KeyCode.E) && QuebradoStuff.active && !arrow.Pode)
                                {
                                    hit.transform.gameObject.GetComponent<Generator>().TocarExplosion();
                                    hahi = this.gameObject.transform.position;
                                    view.RPC("Atrair", RpcTarget.AllViaServer, hit.transform.gameObject.name);
                                    hit.transform.gameObject.GetComponent<PhotonView>().RPC("MudarModo", RpcTarget.AllBuffered, true);
                                }
                            }
                            quebradoText.text = hit.transform.gameObject.GetComponent<Generator>().quebradoPoints.ToString() + "/5";
                            foreach(GameObject a in apertarGenerator)
                            {
                                a.SetActive(!hit.transform.gameObject.GetComponent<Generator>().modoSegurar);
                            }
                            sliderGenerator.SetActive(hit.transform.gameObject.GetComponent<Generator>().modoSegurar);

                        }


                        QuebradoStuff.active=hit.transform.gameObject.GetComponent<Generator>().Quebrado && hit.transform.gameObject.GetComponent<Generator>().canInteract;
                }
                else if (hit.transform.gameObject.tag == "Button" && PhotonNetwork.IsMasterClient)
                {            QuebradoStuff.active=false;
                        ajudarSlider.value = 0;
                        sliderGenerator.GetComponent<Slider>().value = 0;
                        ajudarUI.SetActive(false);
                        cue.text="Button";
                    if(Input.GetKeyDown(KeyCode.E) && hit.transform.gameObject.GetComponent<FirstPersonButton>().canPress)
                    {
                            hit.transform.gameObject.GetComponent<FirstPersonButton>().Pressing();
                    }
                }
                else if (hit.transform.gameObject.tag == "OpenButton")
                {            QuebradoStuff.active=false;
                        ajudarSlider.value = 0;
                        sliderGenerator.GetComponent<Slider>().value = 0;

                        ajudarUI.SetActive(false);
                        cue.text="Button";
                    if(Input.GetKeyDown(KeyCode.E) && hit.transform.gameObject.GetComponent<FirstPersonButton>().canPress)
                    {
                            hit.transform.gameObject.GetComponent<FirstPersonButton>().Pressing(true);
                    }
                }
                else if (hit.transform.gameObject.tag == "Bed")
                {
                    cue.text="Hide";
                        ajudarSlider.value = 0;
                        sliderGenerator.GetComponent<Slider>().value = 0;

                        ajudarUI.SetActive(false);
                        QuebradoStuff.active = false;

                        if (Input.GetKeyDown(KeyCode.E))
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
                        sliderGenerator.GetComponent<Slider>().value = 0;

                        cue.text = "Help";
                        if (hit.transform.gameObject != this.gameObject)
                        {
                            ajudarUI.SetActive(true);
                            if (Input.GetKey(KeyCode.E) && ajudarUI.activeSelf == true)
                            {
                                StartCoroutine(encher());
                            }
                            else if(!Input.GetKey(KeyCode.E))
                            {
                                StopAllCoroutines();
                                ajudarSlider.value = 0;
                            }
                            if (ajudarSlider.value==1)
                            {
                                ajudarSlider.value = 0;
                                ajudarUI.SetActive(false);
                                hit.transform.gameObject.GetComponent<PhotonView>().RPC("LevantarPlayer", RpcTarget.AllBuffered);
                            }
                        }
                    }
                else
                {
                        ajudarSlider.value = 0;
                        ajudarUI.SetActive(false);
                        QuebradoStuff.active=false;
                        sliderGenerator.GetComponent<Slider>().value = 0;

                        cue.text="";
                }
            }
            else
            {
                    ajudarSlider.value = 0;
                    ajudarUI.SetActive(false);
                    QuebradoStuff.active=false;
                    sliderGenerator.GetComponent<Slider>().value = 0;

                    cue.text="";
            }
            }
        }
        else
        {
            ajudarSlider.value = 0;
            sliderGenerator.GetComponent<Slider>().value = 0;

            ajudarUI.SetActive(false);
            cue.text="";
            if(QuebradoStuff!=null)
            {
                QuebradoStuff.active = false;

            }
        }
    }

    public IEnumerator encher()
    {
        yield return new WaitForSeconds(0.00005f);
        if (ajudarUI.activeSelf)
        {
            ajudarSlider.value += 0.005f;
        }
        else
        {
            ajudarSlider.value = 0;
            StopAllCoroutines();
        }
        if (Input.GetKey(KeyCode.E) && ajudarUI.activeSelf == true)
        {
            StopAllCoroutines();
            StartCoroutine(encher());
        }
    }

    public IEnumerator encher1()
    {
        yield return new WaitForSeconds(0.0005f);
        if (sliderGenerator.activeSelf)
        {
            sliderGenerator.GetComponent<Slider>().value += 0.01f;
        }
        else
        {
            sliderGenerator.GetComponent<Slider>().value = 0;
            StopAllCoroutines();
        }
        if (Input.GetKey(KeyCode.E) && sliderGenerator.activeSelf == true)
        {
            StopAllCoroutines();
            StartCoroutine(encher1());
        }
    }

    [PunRPC]
    public void Atrair(string ha)
    {
        GameObject.FindObjectOfType<EnemyAI>().AvisarGenerator(GameObject.Find(ha).transform);
    }
}
