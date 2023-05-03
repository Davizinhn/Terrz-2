﻿using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.Burst.CompilerServices;

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
    public GameObject RagDollLeonard;
    public GameObject RagDollMegan;
    public GameObject coiso;

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
                a.enabled=false;
            }
            // Get the rigidbody on this.
            rigidbody = GetComponent<Rigidbody>();
            camera = GetComponentInChildren<Camera>();
            if(Persona=="leonard")
            {
                anim.SetBool("isGrounded", true);
            }
            else if(Persona=="megan")
            {
                anim1.SetBool("isGrounded", true);
            }
        }
        else
        {
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
        if(view.IsMine)
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

            if(isLaying && Input.GetKeyDown(KeyCode.E))
                {
                    curBed.gameObject.GetComponent<PhotonView>().RPC("unLayHere", RpcTarget.AllBuffered, this.gameObject.GetComponent<PhotonView>().ViewID);
                }

            // Get targetMovingSpeed.
            float targetMovingSpeed = IsRunning ? runSpeed : speed;
            if (speedOverrides.Count > 0)
            {
                targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
            }

            // Get targetVelocity from input.
            if(GameObject.Find("CinematicCamera") == null || !GameObject.Find("CinematicCamera").GetComponent<CinematicManager>().inCinematic){
                    Vector2 targetVelocity = new Vector2(0f, 0f);
                    if (!isLaying) { targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed); }
            if(Persona=="leonard")
            {
                anim.SetFloat("Vertical", targetVelocity.y);
                anim.SetFloat("Horizontal", targetVelocity.x);
                anim.SetBool("isRunning", IsRunning);
                anim.SetBool("isGrounded", ground.isGrounded);
                        anim.SetBool("isLaying", isLaying);
            }
            else if(Persona=="megan")
            {
                anim1.SetFloat("Vertical", targetVelocity.y);
                anim1.SetFloat("Horizontal", targetVelocity.x);
                anim1.SetBool("isRunning", IsRunning);
                anim1.SetBool("isGrounded", ground.isGrounded);
                        anim1.SetBool("isLaying", isLaying);
                    }
            // Apply movement.
                        RaycastHit hit;
            Ray ray = new Ray(this.transform.position, Camera.main.transform.forward);

                    if (!isLaying)
                    {
                        rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y, targetVelocity.y);
                    }
            }

            // Apply headbobbing.
            if(ground.isGrounded && !isLaying)
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
        }
        }
        else
        {
            if(Persona=="leonard")
            {
                this.gameObject.transform.position = RagDollLeonard.transform.position;
            }
            else if(Persona=="megan")
            {
                this.gameObject.transform.position = RagDollMegan.transform.position;
            }
        }
    }

    public void Morrer(bool morreu = true)
    {
        if(view.IsMine){
        if(!isDead && morreu)
        {

            if(Persona=="megan")
            {
                    view.RPC("SpawnMegan", RpcTarget.AllBufferedViaServer);
                }
            else
            {
                    view.RPC("SpawnLeonard", RpcTarget.AllBufferedViaServer);
                }
            GameObject.Find("SpectatorManager").GetComponent<SpectatorManager>().Spectator=true;
            this.gameObject.tag="PlayerMorto";
            this.gameObject.GetComponent<Rigidbody>().freezeRotation = true;
            //this.gameObject.GetComponent<SpectatorMePls>().enabled = false;
            isDead=true;            
            view.RPC("MorrerRPC", RpcTarget.AllBufferedViaServer);
        }
        else if(!isDead && !morreu)
        {
            GameObject.Find("SpectatorManager").GetComponent<SpectatorManager>().Spectator=true;
            isDead=true;
            view.RPC("MorrerRPC", RpcTarget.AllBufferedViaServer);
            view.RPC("DestroyThis", RpcTarget.AllBufferedViaServer);
        }}
    }

    [PunRPC]
    public void MorrerRPC()
    {
        //morteSound.active=true;
        Destroy(gameObject.GetComponentInChildren<FirstPersonAudio>().gameObject);
        //Destroy(anim.gameObject);
        //Destroy(anim1.gameObject);
        foreach(SkinnedMeshRenderer i in personaMesh)
        {
            i.enabled = false; 
        }
        Destroy(coiso);
        GameObject.Find("SpectatorManager").GetComponent<SpectatorManager>().playersMortos++;
        this.gameObject.tag="PlayerMorto";
        this.isDead = true;
        //this.gameObject.GetComponent<SpectatorMePls>().enabled = false;
    }

    [PunRPC]
    public void SpawnMegan()
    {
        RagDollMegan.transform.parent = null;
        RagDollMegan.SetActive(true);
    }

    [PunRPC]
    public void SpawnLeonard()
    {
        RagDollLeonard.transform.parent = null;
        RagDollLeonard.SetActive(true);
    }

    [PunRPC]
    public void DestroyThis()
    {
        Destroy(this.gameObject);
    }
}
