using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.Burst.CompilerServices;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

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

    public Cinemachine.CinemachineFreeLook emoteCamCine;

    Rigidbody rigidbody;
    public Camera camera;
    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    private float timer = 0.0f;

    public PhotonView view;
    public SkinnedMeshRenderer[] personaMesh;
    public GameObject[] others;
    public LayerMask collisionLayer;
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
    public bool hasFallen;
    int morrerInt = 0;
    public GameObject coisos;
    LobbyManager lobbyManager;
    public object maxLives;
    public AudioClip[] grunts;
    public AudioSource gruntSource;
    public AudioSource breath;
    public bool isCrouching = false;
    public float crouchY;
    public CapsuleCollider normalCollider;
    public CapsuleCollider crouchCollider;
    public float distance;
    public Transform[] coisosCrouch;

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
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Lives", out maxLives);
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            if (SceneManager.GetActiveScene().name=="Game")
            {
            gameManager = GameObject.FindObjectOfType<ManageGame>();
            }
            else
            {
                lobbyManager = GameObject.FindObjectOfType<LobbyManager>();
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
            if (lobbyManager != null)
            {
                lobbyManager.curCam = camera.gameObject.transform;
            }
            else
            {
                gameManager.curCam = camera.gameObject.transform;
            }
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
            coisos.transform.parent = LeonardHead;
        }
        else
        {
            Persona="megan";
            coisos.transform.parent = meganHead;
        }
        bibi++;
        if(bibi>2)
        {
            Persona =personaOther;
            hi=true;
        }
    }

    public void TirarAnimsLobby()
    {
        rigidbody.velocity = new Vector3(0, 0, 0);
        camera.gameObject.GetComponent<FirstPersonLook>().enabled = true;
        isEmoting = false;
        camera.enabled = true;
        emoteCam.active = false;
        userPanel.active = true;
        if (Persona == "leonard")
        {
            anim.SetFloat("Vertical", 0f);
            anim.SetFloat("Horizontal", 0f);
            anim.SetBool("isRunning", false);
            anim.SetBool("isEmoting", false);
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
            anim1.SetFloat("Vertical", 0f);
            anim1.SetFloat("Horizontal", 0f);
            anim1.SetBool("isRunning", false);
            anim1.SetBool("isEmoting", false);
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
        if (lobbyManager != null)
        {
            lobbyManager.curCam = camera.gameObject.transform;
        }
        else
        {
            gameManager.curCam = camera.gameObject.transform;
        }
    }

    public bool canUnCrouch()
    {
        RaycastHit hit;
        foreach(Transform a in coisosCrouch)
        {
            Ray ray = new Ray(a.position, Vector3.up);
            if (Physics.Raycast(ray, out hit, distance, collisionLayer))
            {
                Debug.DrawLine(a.position, hit.point, Color.red);
                Debug.Log("Hitted " + hit.transform.gameObject.name);
                return false;
            }
        }
        return true;
    }



    private void Update()
    {
        if (view.IsMine && (gameManager != null ? !gameManager.isPaused : !lobbyManager.rulesOpen))
        {
            CrouchBehaviour();
        }
        crouchCollider.enabled = isCrouching;
        normalCollider.enabled = !isCrouching;
    }

    [PunRPC]
    public void isCrouchingRPC(bool crouched)
    {
        isCrouching = crouched;
    }

    void FixedUpdate()
    {
        if (!isDead && !hasFallen)
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
        if(view.IsMine && (gameManager!=null ? !gameManager.isPaused : !lobbyManager.rulesOpen))
        {
                    if(IsRunning)
                    {
                        bobbingSpeed = 0.25f;
                    }
                    else
                    {
                        bobbingSpeed = 0.18f;
                    }
                //#if UNITY_EDITOR
                /*if(Input.GetKeyDown(KeyCode.M))
                {
                    MorrerEscolher();
                }*/
                //#endif
                // Update IsRunning from input.
                IsRunning = canRun && Input.GetKey(runningKey) && !isLaying && !isCrouching;

            if(isLaying && Input.GetKeyDown(KeyCode.E) && GameObject.Find("Monster").GetComponent<EnemyAI>().curBed != curBed)
                {
                    curBed.gameObject.GetComponent<PhotonView>().RPC("unLayHere", RpcTarget.AllBuffered, this.gameObject.GetComponent<PhotonView>().ViewID);
                }

                if (canEmote && Input.GetKey(KeyCode.Q) && !isEmoting)
                {
                    camera.gameObject.GetComponent<FirstPersonLook>().enabled = false;
                    emotePanel.active = true;
                    UnityEngine.Cursor.lockState = CursorLockMode.None;
                }
                else if (canEmote && !Input.GetKey(KeyCode.Q))
                {
                    camera.gameObject.GetComponent<FirstPersonLook>().enabled = true;
                    emotePanel.active = false;
                    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                }

            // Get targetMovingSpeed.
                float targetMovingSpeed;
                if(IsRunning)
                {
                    targetMovingSpeed = runSpeed;
                }
                else if(isCrouching)
                {
                    targetMovingSpeed = speed / 2;
                }
                else
                {
                    targetMovingSpeed = speed;
                }
            if (speedOverrides.Count > 0)
            {
                targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
            }
            if(GameObject.Find("CinematicCamera") != null)
            {
                canEmote = !isLaying && !isEmoting && ground.isGrounded && this.gameObject.GetComponent<Interact>().cue.text=="" && !GameObject.Find("CinematicCamera").GetComponent<CinematicManager>().inCinematic && !isCrouching;
            }
            else{
                canEmote = !isLaying && !isEmoting && ground.isGrounded && this.gameObject.GetComponent<Interact>().cue.text== "" && !isCrouching;
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
                        anim.SetBool("isCrouching", isCrouching);

                    }
            else if(Persona=="megan")
            {
                anim1.SetFloat("Vertical", targetVelocity.y);
                anim1.SetFloat("Horizontal", targetVelocity.x);
                anim1.SetBool("isRunning", IsRunning);
                anim1.SetBool("isGrounded", ground.isGrounded);
                        anim1.SetBool("isLaying", isLaying);
                        anim1.SetBool("isEmoting", isEmoting);
                        anim1.SetBool("isCrouching", isCrouching);
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
            if(ground.isGrounded && !isLaying && !isEmoting && !isCrouching)
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
            else
                {
                    Vector3 localPosition = camera.transform.localPosition;
                    localPosition.y = midpoint;
                    camera.transform.localPosition = localPosition;
                    if (isCrouching)
                    {
                        camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y - crouchY, camera.transform.position.z);
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
                    if (lobbyManager != null)
                    {
                        lobbyManager.curCam = camera.gameObject.transform;
                    }
                    else
                    {
                        gameManager.curCam = camera.gameObject.transform;
                    }
                }
            
        }
        }
        else
        {
            emoteCamCine.enabled = gameManager != null ? !gameManager.isPaused : true;
            if (Persona=="leonard")
            {
                anim.SetBool("isDead", isDead);
                anim.SetBool("isGrounded", true);
                anim.SetBool("isEmoting", isEmoting);
                //anim.SetBool("hasFallen", hasFallen);
            }
            else if(Persona=="megan")
            {
                anim1.SetBool("isDead", isDead);
                anim1.SetBool("isGrounded", true);
                anim1.SetBool("isEmoting", isEmoting);
                //anim1.SetBool("hasFallen", hasFallen);
            }
        }
    }

    public void CrouchBehaviour()
    {
        bool canCrouch;
        if (GameObject.Find("CinematicCamera") != null)
        {
            canCrouch = !isLaying && !isEmoting && ground.isGrounded && !GameObject.Find("CinematicCamera").GetComponent<CinematicManager>().inCinematic && !isCrouching;
        }
        else
        {
            canCrouch = !isLaying && !isEmoting && ground.isGrounded && !isCrouching;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if(!isCrouching)
            {
                if(canCrouch)
                {
                    Crouch();
                }
            }
            else
            {
                if(canUnCrouch())
                {
                    unCrouch();
                }
            }
        }
    }

    public void Crouch()
    {
        isCrouching = true;
        view.RPC("isCrouchingRPC", RpcTarget.OthersBuffered, true);
        camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y-crouchY, camera.transform.position.z);
    }

    public void unCrouch()
    {
        isCrouching = false;
        view.RPC("isCrouchingRPC", RpcTarget.OthersBuffered, false);
        camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y + crouchY, camera.transform.position.z);
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
        if (lobbyManager != null)
        {
            lobbyManager.curCam = emoteCam.transform.GetChild(0).gameObject.transform;
        }
        else
        {
            gameManager.curCam = emoteCam.transform.GetChild(0).gameObject.transform;
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

    public void MorrerEscolher()
    {
        if(!isDead && !hasFallen)
        {
            if (gameManager.playersPost > 1)
            {
                if (morrerInt > (int)maxLives-1)
                {
                    Morrer(true);
                }
                else
                {
                    QuaseMorrer();
                }
            }
            else
            {
                #if UNITY_EDITOR
                    QuaseMorrer();
                #else
                    Morrer(true); // MUDAR PRO SÓ MORRER DPS
                #endif
            }
        }


    }

    public void QuaseMorrer()
    {
        if(view.IsMine)
        {
            if(!hasFallen)
            {
                hasFallen = true;
                morrerInt++;
                isEmoting = false;
                if (Persona == "megan")
                {
                    outro.active = false;
                    megan.active = true;
                }
                else
                {
                    megan.active = false;
                    outro.active = true;
                }
                if (Persona == "leonard")
                {
                    anim.SetBool("hasFallen", true);
                }
                else if (Persona == "megan")
                {
                    anim1.SetBool("hasFallen", true);
                }
                foreach (GameObject a in elementosUIDelete)
                {
                    a.active = false;
                }
                emotePanel.active = false;
                emoteCam.active = true;
                camera.gameObject.GetComponent<FirstPersonLook>().enabled = false;
                camera.enabled = false;
                if (isLaying)
                {
                    curBed.gameObject.GetComponent<PhotonView>().RPC("unLayHere", RpcTarget.AllBuffered, this.gameObject.GetComponent<PhotonView>().ViewID);
                }
                this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                this.gameObject.tag = "PlayerCaido";
                GameObject.Find("SpectatorManager").GetComponent<SpectatorManager>().playersMortos++;
                this.gameObject.layer = 11;
                int gruntInt = Random.Range(0, grunts.Length);
                gruntSource.PlayOneShot(grunts[gruntInt]);
                breath.Play();
                view.RPC("QuaseMorrerOutros", RpcTarget.OthersBuffered, gruntInt);
                //StartCoroutine(morrerPorDemora());
                if(lobbyManager!=null)
                {
                    lobbyManager.curCam = emoteCam.transform.GetChild(0).gameObject.transform;
                }
                else
                {
                    gameManager.curCam = emoteCam.transform.GetChild(0).gameObject.transform;
                }
            }
        }
    }

    [PunRPC]
    public void QuaseMorrerOutros(int gruntInt)
    {
        this.gameObject.layer = 11;
        gruntSource.PlayOneShot(grunts[gruntInt]);
        breath.Play();
        GameObject.Find("SpectatorManager").GetComponent<SpectatorManager>().playersMortos++;
        this.gameObject.tag = "PlayerCaido";
        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        if (Persona == "megan")
        {
            megan.GetComponent<Outline>().enabled = true;
        }
        else
        {
            outro.GetComponent<Outline>().enabled = true;
        }
        isEmoting = false;
    }

    [PunRPC]
    public void LevantarPlayer()
    {
        //StopCoroutine(morrerPorDemora());

            hasFallen = false;
            GameObject.Find("SpectatorManager").GetComponent<SpectatorManager>().playersMortos--;
            this.gameObject.tag = "Player";

            if (Persona == "leonard")
            {
                anim.SetBool("hasFallen", false);
            outro.GetComponent<Outline>().enabled = false;


            }
            else if (Persona == "megan")
            {
                anim1.SetBool("hasFallen", false);
            megan.GetComponent<Outline>().enabled = false;

            }
        breath.Stop();
        Invoke("VoltarMovimentoAnim", 1.2f);


    }

    public void VoltarMovimentoAnim()
    {
        view.RPC("VoltarMovimentos", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void VoltarMovimentos()
    {
        this.gameObject.layer = 3;

        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        if (view.IsMine)
        {
            camera.gameObject.GetComponent<FirstPersonLook>().enabled = true;
            camera.enabled = true;
            emoteCam.active = false;
            foreach (GameObject a in elementosUIDelete)
            {
                a.active = true;
            }
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
            if (lobbyManager != null)
            {
                lobbyManager.curCam = camera.gameObject.transform;
            }
            else
            {
                gameManager.curCam = camera.gameObject.transform;
            }
        }
    }

    /*
    public IEnumerator morrerPorDemora()
    {
        yield return new WaitForSeconds(15f);
        if (!isDead && hasFallen)
        {
            Morrer(true);
            yield break;
        }
        yield break;
    }
    */

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
        this.gameObject.layer = 11;
        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        Destroy(gameObject.GetComponentInChildren<FirstPersonAudio>().gameObject);
        GameObject.Find("SpectatorManager").GetComponent<SpectatorManager>().playersMortos++;
        this.gameObject.tag="PlayerMorto";                
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
