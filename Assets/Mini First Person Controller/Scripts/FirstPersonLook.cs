using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField]
    Transform character;
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    Vector2 velocity;
    Vector2 frameVelocity;
    public ManageGame gameManager;



    void Reset()
    {
        // Get the character from the FirstPersonMovement in parents.
        character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Start()
    {
        // Lock the mouse cursor to the game screen.
        if(PlayerPrefs.GetInt("PostProcess") == 0)
        {
            this.gameObject.GetComponent<PostProcessVolume>().enabled=false;
        }
            if(SceneManager.GetActiveScene().name=="Game")
            {
            gameManager = GameObject.FindObjectOfType<ManageGame>();
            }
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if((GameObject.Find("CinematicCamera") == null ||!GameObject.Find("CinematicCamera").GetComponent<CinematicManager>().inCinematic) && (gameManager!=null ? !gameManager.isPaused : true))
        {
            // Get smooth velocity.
            Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
            frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
            velocity += frameVelocity;
            velocity.y = Mathf.Clamp(velocity.y, -90, 90);

            // Rotate camera up-down and controller left-right from velocity.
            transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
            character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
        }
    }
}
