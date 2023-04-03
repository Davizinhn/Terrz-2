using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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

    Rigidbody rigidbody;
    Camera camera;
    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    private float timer = 0.0f;

    public PhotonView view;
    public SkinnedMeshRenderer[] personaMesh;
    public GameObject[] others;
    public LayerMask collisionLayer;
    public float distance = 2f;
    public string Persona;
    public GameObject megan, outro;

    [PunRPC]
    public void SetChar(string persona)
    {
        Persona = persona;
    }

    void Awake()
    {
        if(view.IsMine)
        {
            view.RPC("SetChar", RpcTarget.AllBuffered, PlayerPrefs.GetString("curPersona"));
            foreach(SkinnedMeshRenderer a in personaMesh)
            {
                a.enabled=false;
            }
            if(Persona=="megan")
            {
                anim = megan.GetComponent<Animator>();
            }
            else
            {
                anim = outro.GetComponent<Animator>();
            }
            // Get the rigidbody on this.
            rigidbody = GetComponent<Rigidbody>();
            camera = GetComponentInChildren<Camera>();
        }
        else
        {
            if(Persona=="megan")
            {
                outro.active=false;
                megan.active=true;
                anim = megan.GetComponent<Animator>();
            }
            else
            {
                megan.active=false;
                outro.active=true;
                anim = outro.GetComponent<Animator>();
            }
            foreach(GameObject a in others)
            {
                Destroy(a);
            }
        }

        
    }

    void FixedUpdate()
    {
        if(view.IsMine)
        {
            if(Persona=="megan")
            {
                anim = megan.GetComponent<Animator>();
            }
            else
            {
                anim = outro.GetComponent<Animator>();
            }
            // Get the rigidbody on this.
            rigidbody = GetComponent<Rigidbody>();
            camera = GetComponentInChildren<Camera>();
        }
        else
        {
            if(Persona=="megan")
            {
                outro.active=false;
                megan.active=true;
                anim = megan.GetComponent<Animator>();
            }
            else
            {
                megan.active=false;
                outro.active=true;
                anim = outro.GetComponent<Animator>();
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
            IsRunning = canRun && Input.GetKey(runningKey);

            // Get targetMovingSpeed.
            float targetMovingSpeed = IsRunning ? runSpeed : speed;
            if (speedOverrides.Count > 0)
            {
                targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
            }

            // Get targetVelocity from input.
            Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);
            anim.SetFloat("Vertical", targetVelocity.y);
            anim.SetFloat("Horizontal", targetVelocity.x);
            anim.SetBool("isRunning", IsRunning);
            anim.SetBool("isGrounded", ground.isGrounded);
            // Apply movement.
                        RaycastHit hit;
            Ray ray = new Ray(this.transform.position, Camera.main.transform.forward);

                rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y, targetVelocity.y);

            // Apply headbobbing.
            if(ground.isGrounded)
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
}
