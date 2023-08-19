using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.Burst.CompilerServices;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public GroundCheck ground;
    public KeyCode runningKey = KeyCode.LeftShift;

    [Header("Headbobbing")]
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.2f;
    public float midpoint = 2.0f;

    public Animator anim;
    public Animator anim1;
    public bool isLaying;
    public BedBehaviour curBed;
    public Transform LeonardHead;
    public Transform meganHead;
    public GameObject cameraHead;
    public TMP_Text uiNameText;

    Rigidbody rigidbody;
    public Camera camera;
    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    private float timer = 0.0f;

    public PhotonView view;
    public SkinnedMeshRenderer[] personaMesh;
    public GameObject[] others;
    public LayerMask collisionLayer;
    public float distance = 2f;
    public string Persona;
    string personaOther;
    public GameObject megan, outro;
    bool hi;
    public bool isDead;
    GameObject outroEu;
    public GameObject morteSound;
    public GameObject[] elementosUIDelete;
    public bool isEmoting;
    public bool canEmote;
    public GameObject emotePanel;
    public GameObject emoteCam;
    public GameObject userPanel;
    public ManageGame gameManager;

    [PunRPC]
    public void SetChar(string persona)
    {
        Persona = persona;
        personaOther = persona;
    }

    public void DestroyAObject()
    {
        Destroy(outroEu);
    }

    void Awake()
    {
        if(view.IsMine)
        {
            if(SceneManager.GetActiveScene().name=="Game")
            {
            gameManager = GameObject.FindObjectOfType<ManageGame>();
            }
            /*foreach(FirstPersonMovement player in GameObject.FindObjectsOfType<FirstPersonMovement>())
            {
                if(player.gameObject.GetComponent<PhotonView>().IsMine && player.gameObject != this.gameObject)
                {
                    outroEu = player.gameObject;
                    Destroy(outroEu);
                }
            }*/

            view.RPC("SetChar", RpcTarget.AllBuffered, PlayerPrefs.GetString("curPersona"));
            foreach(SkinnedMeshRenderer a in personaMesh)
            {
                a.gameObject.layer=9;
            }
            // Get the rigidbody on this.
            rigidbody = GetComponent<Rigidbody>();
            //camera = GetComponentInChildren<Camera>();            
            if(Persona=="leonard")
            {
                anim.SetBool("isGrounded", true);                
                foreach(SkinnedMeshRenderer a in anim1.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    a.enabled = false;
                }
                cameraHead.transform.parent = LeonardHead;

            }
            else if(Persona=="megan")
            {
                anim1.SetBool("isGrounded", true);
                foreach (SkinnedMeshRenderer a in anim.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    a.enabled = false;
                }
                cameraHead.transform.parent = meganHead;
            }
            uiNameText.text = PhotonNetwork.NickName;
        }
        else
        {
            Destroy(emoteCam);
            if(Persona=="megan")
            {
                outro.active=false;
                megan.active=true;
            }
            else
            {
                megan.active=false;
                outro.active=true;
            }
            Att();
            foreach(GameObject a in others)
            {
                Destroy(a);
            }
        }
        

        
    }
    int bibi;
    public void Att()
    {
        if(Persona=="megan")
        {
            Persona="leonard";
        }
        else
        {
            Persona="megan";
        }
        bibi++;
        if(bibi>2)
        {
            Persona =personaOther;
            hi=true;
        }
    }

    void FixedUpdate()
    {
        if(!isDead)
        {
             if(!view.IsMine)
        {
            if(!hi){Att();}
            if(Persona=="megan")
            {
                outro.active=false;
                megan.active=true;
            }
            else
            {
                megan.active=false;
                outro.active=true;
            }
        }
        if((gameManager!=null ? gameManager.isPaused : false))
        {
            rigidbody.velocity=new Vector3(0, 0, 0);
            if(isEmoting)
            {
                    camera.gameObject.GetComponent<FirstPersonLook>().enabled = true;
                    isEmoting = false;
                    camera.enabled = true;
                    emoteCam.active = false;
                    userPanel.active = true;
                    if (Persona == "leonard")
                    {
                        foreach (SkinnedMeshRenderer a in anim1.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            a.gameObject.layer = 9;
                            a.enabled = false;
                        }
                        foreach (SkinnedMeshRenderer a in anim.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            a.gameObject.layer = 9;
                            a.enabled = true;
                        }
 

                    }
                    else if (Persona == "megan")
                    {
                        foreach (SkinnedMeshRenderer a in anim1.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            a.gameObject.layer = 9;
                            a.enabled = true;
                        }
                        foreach (SkinnedMeshRenderer a in anim.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            a.gameObject.layer = 9;
                            a.enabled = false;
                        }
                    }
            }
        }
        if(view.IsMine && (gameManager!=null ? !gameManager.isPaused : true))
        {
                    if(IsRunning)
                    {
                        bobbingSpeed = 0.25f;
                    }
                    else
                    {
                        bobbingSpeed = 0.18f;
                    }
            // Update IsRunning from input.
            IsRunning = canRun && Input.GetKey(runningKey) && !isLaying;

            if(isLaying && Input.GetKeyDown(KeyCode.E) && GameObject.Find("Monster").GetComponent<EnemyAI>().curBed != curBed)
                {
                    curBed.gameObject.GetComponent<PhotonView>().RPC("unLayHere", RpcTarget.AllBuffered, this.gameObject.GetComponent<PhotonView>().ViewID);
                }

                if (canEmote && Input.GetMouseButton(2) && !isEmoting)
                {
                    camera.gameObject.GetComponent<FirstPersonLook>().enabled = false;
                    emotePanel.active = true;
                    UnityEngine.Cursor.lockState = CursorLockMode.None;
                }
                else if (canEmote && !Input.GetMouseButton(2))
                {
                    camera.gameObject.GetComponent<FirstPersonLook>().enabled = true;
                    emotePanel.active = false;
                    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                }

            // Get targetMovingSpeed.
            float targetMovingSpeed = IsRunning ? runSpeed : speed;
            if (speedOverrides.Count > 0)
            {
                targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
            }
            if(GameObject.Find("CinematicCamera") != null)
            {
                canEmote = !isLaying && !isEmoting && ground.isGrounded && this.gameObject.GetComponent<Interact>().cue.text=="" && !GameObject.Find("CinematicCamera").GetComponent<CinematicManager>().inCinematic;
            }
            else{
                canEmote = !isLaying && !isEmoting && ground.isGrounded && this.gameObject.GetComponent<Interact>().cue.text=="";
            }

            // Get targetVelocity from input.
            if(GameObject.Find("CinematicCamera") == null || !GameObject.Find("CinematicCamera").GetComponent<CinematicManager>().inCinematic){
                    Vector2 targetVelocity = new Vector2(0f, 0f);
                    if (!isLaying || !isEmoting) { targetVelocity = Vector2.ClampMagnitude(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), 1f) * targetMovingSpeed; }
            if(Persona=="leonard")
            {
                anim.SetFloat("Vertical", targetVelocity.y);
                anim.SetFloat("Horizontal", targetVelocity.x);
                anim.SetBool("isRunning", IsRunning);
                anim.SetBool("isGrounded", ground.isGrounded);
                        anim.SetBool("isLaying", isLaying);
                        anim.SetBool("isEmoting", isEmoting);

                    }
            else if(Persona=="megan")
            {
                anim1.SetFloat("Vertical", targetVelocity.y);
                anim1.SetFloat("Horizontal", targetVelocity.x);
                anim1.SetBool("isRunning", IsRunning);
                anim1.SetBool("isGrounded", ground.isGrounded);
                        anim1.SetBool("isLaying", isLaying);
                        anim1.SetBool("isEmoting", isEmoting);
                    }
            // Apply mov
            // ement.
                    if (!isLaying)
                    {
                        Vector3 movementDirection = new Vector3(targetVelocity.x, 0f, targetVelocity.y);
                        Vector3 adjustedVelocity = transform.rotation * movementDirection;
                        adjustedVelocity.y = rigidbody.velocity.y;

                        rigidbody.velocity = adjustedVelocity;

                    }
                }

            // Apply headbobbing.
            if(ground.isGrounded && !isLaying && !isEmoting)
            {
                float waveslice = 0.0f;
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");

                if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
                {
                    timer = 0.0f;
                }
                else
                {
                    waveslice = Mathf.Sin(timer);
                    timer += bobbingSpeed;
                    if (timer > Mathf.PI * 2)
                    {
                        timer = timer - (Mathf.PI * 2);
                    }
                }
                if (waveslice != 0)
                {
                    float translateChange = waveslice * bobbingAmount;
                    float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
                    totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
                    translateChange = totalAxes * translateChange;
                    Vector3 localPosition = camera.transform.localPosition;
                    localPosition.y = midpoint + translateChange;
                    camera.transform.localPosition = localPosition;
                }
                else
                {
                    Vector3 localPosition = camera.transform.localPosition;
                    localPosition.y = midpoint;
                    camera.transform.localPosition = localPosition;
                }
            }
            if(isEmoting && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
                {
                    camera.gameObject.GetComponent<FirstPersonLook>().enabled = true;
                    isEmoting = false;
                    camera.enabled = true;
                    emoteCam.active = false;
                    userPanel.active = true;
                    if (Persona == "leonard")
                    {
                        foreach (SkinnedMeshRenderer a in anim1.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            a.gameObject.layer = 9;
                            a.enabled = false;
                        }
                        foreach (SkinnedMeshRenderer a in anim.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            a.gameObject.layer = 9;
                            a.enabled = true;
                        }
 

                    }
                    else if (Persona == "megan")
                    {
                        foreach (SkinnedMeshRenderer a in anim1.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            a.gameObject.layer = 9;
                            a.enabled = true;
                        }
                        foreach (SkinnedMeshRenderer a in anim.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            a.gameObject.layer = 9;
                            a.enabled = false;
                        }
                    }
                }
            
        }
        }
        else
        {
            if(Persona=="leonard")
            {
                anim.SetBool("isDead", true);
                anim.SetBool("isGrounded", true);
                anim.SetBool("isEmoting", isEmoting);
            }
            else if(Persona=="megan")
            {
                anim1.SetBool("isDead", true);
                anim1.SetBool("isGrounded", true);
                anim1.SetBool("isEmoting", isEmoting);
            }
        }
    }

    public void FazerEsseEmote(int number)
    {            
        isEmoting=true;
        view.RPC("MudarAnimRPC", RpcTarget.AllBuffered, number);
            if (Persona == "leonard")
            {
                view.RPC("TriggerAnim", RpcTarget.AllBuffered, "emote_"+number.ToString(), 0);
            }
            else if (Persona == "megan")
            {
                view.RPC("TriggerAnim", RpcTarget.AllBuffered, "emote_"+number.ToString(), 1);
            }
        emotePanel.active = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        camera.enabled = false;
        emoteCam.active = true;
        userPanel.active = false;
                            if (Persona == "leonard")
                    {

                        foreach (SkinnedMeshRenderer a in anim.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            a.gameObject.layer = 3;
                            a.enabled = true;
                        }
 

                    }
                    else if (Persona == "megan")
                    {
                        foreach (SkinnedMeshRenderer a in anim1.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            a.gameObject.layer = 3;
                            a.enabled = true;
                        }
                    }
    }


    [PunRPC]
    public void MudarAnimRPC(int number)
    {
        isEmoting=true;
            if (Persona == "leonard")
            {
                view.RPC("TriggerAnim", RpcTarget.AllBuffered, "emote_"+number.ToString(), 0);
            }
            else if (Persona == "megan")
            {
                view.RPC("TriggerAnim", RpcTarget.AllBuffered, "emote_"+number.ToString(), 1);
            }
    }

    public void Morrer(bool morreu = true)
    {
        if(view.IsMine){
        if(!isDead && morreu)
        {
            morteSound.active=true;
                isEmoting = false;
            if(Persona=="megan")
            {
                outro.active=false;
                megan.active=true;
            }
            else
            {
                megan.active=false;
                outro.active=true;
            }
            foreach(SkinnedMeshRenderer a in personaMesh)
            {
                a.gameObject.layer=3;
                    a.enabled = true;
            }
            foreach(GameObject a in elementosUIDelete)
            {
                a.active=false;
            }
                emotePanel.active = false;
                GameObject.Find("SpectatorManager").GetComponent<SpectatorManager>().Spectator=true;
            this.gameObject.tag="PlayerMorto";
            this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                isDead =true;
                emoteCam.active = false;
                isEmoting = false;
                view.RPC("MorrerRPC", RpcTarget.AllBuffered);
            if(Persona=="leonard")
            {
                anim.SetBool("isDead", true);
            }
            else if(Persona=="megan")
            {
                anim1.SetBool("isDead", true);
            }
                isLaying = false;
                if (isLaying)
                {
                    curBed.gameObject.GetComponent<PhotonView>().RPC("unLayHere", RpcTarget.AllBuffered, this.gameObject.GetComponent<PhotonView>().ViewID);
                }
            }
        else if(!isDead && !morreu)
        {
            GameObject.Find("SpectatorManager").GetComponent<SpectatorManager>().Spectator=true;
            isDead=true;
            view.RPC("MorrerRPC", RpcTarget.AllBuffered);
            view.RPC("DestroyThis", RpcTarget.AllBuffered);
        }}
    }

    [PunRPC]
    public void MorrerRPC()
    {
        //this.gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
        morteSound.active=true;
        Destroy(gameObject.GetComponentInChildren<FirstPersonAudio>().gameObject);
        GameObject.Find("SpectatorManager").GetComponent<SpectatorManager>().playersMortos++;
        this.gameObject.tag="PlayerMorto";                
        this.gameObject.layer = 6;
    }

    [PunRPC]
    public void DestroyThis()
    {
        Destroy(this.gameObject);
    }

    [PunRPC]
    public void TriggerAnim(string which, int qual = 0)
    {
        if(qual==0)
        {
        anim.SetTrigger(which);
        }
        else
        {
        anim1.SetTrigger(which);
        }
    }
}
