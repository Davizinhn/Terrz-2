using UnityEngine;
using Photon.Pun;

public class Jump : MonoBehaviour
{
    Rigidbody rigidbody;
    public float jumpStrength = 2;
    public event System.Action Jumped;

    [SerializeField, Tooltip("Prevents jumping when the transform is in mid-air.")]
    GroundCheck groundCheck;

    public PhotonView view;


    void Reset()
    {
        if(view.IsMine)
        {
                    // Try to get groundCheck.
        groundCheck = GetComponentInChildren<GroundCheck>();
        }
    }

    void Awake()
    {
                if(view.IsMine)
        {
        // Get rigidbody.
        rigidbody = GetComponent<Rigidbody>();
        }
    }

    void LateUpdate()
    {
                if(view.IsMine && !this.gameObject.GetComponent<FirstPersonMovement>().isDead && GameObject.Find("CinematicCamera") == null || !GameObject.Find("CinematicCamera").GetComponent<CinematicManager>().inCinematic)
        {
        // Jump when the Jump button is pressed and we are on the ground.
        if (Input.GetButtonDown("Jump") && (!groundCheck || groundCheck.isGrounded))
        {
            rigidbody.AddForce(Vector3.up * 100 * jumpStrength);
            Jumped?.Invoke();
        }}
    }
}
