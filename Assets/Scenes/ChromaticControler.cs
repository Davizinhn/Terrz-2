using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.AI;
using Photon.Pun;

public class ChromaticControler : MonoBehaviour
{
    public Transform player;
    public Transform enemy;
    public PostProcessVolume v;
    private ChromaticAberration ca;
    private Grain gra;
    public AudioSource chaseMusic;
    public AudioSource normalMusic;
    public float maxDistance = 5f;
    public float maxChromaticAberration = 1f;
    public float maxDistanceAud = 5f;
    public float maxAud = 1f;

    public void Awake()
    {
    }

    private void Update()
    {
        if(v != null){
            v.profile.TryGetSettings(out ca);
            v.profile.TryGetSettings(out gra);
            float distance = Vector3.Distance(enemy.position, player.position);
            float chromaticAberrationValue = maxChromaticAberration * (1 - (distance / maxDistance));
            float musicVol = 0;
            if(this.gameObject.GetComponent<Enemy_Chase>().Seguindo && distance <= maxDistanceAud)
            {
                musicVol = maxAud * (1 - (distance / maxDistanceAud));
            }
            else
            {
                musicVol=0.1f;
            }
            ca.intensity.value = chromaticAberrationValue;
            gra.intensity.value = chromaticAberrationValue;
            if(musicVol > 0.1f && !chaseMusic.isPlaying)
            {
                chaseMusic.volume = musicVol;
                chaseMusic.Play();
            }
            else if(distance > maxDistanceAud && chaseMusic.isPlaying)
            {
                musicVol = 0.1f;
                chaseMusic.Stop();
            }

            if(chaseMusic.isPlaying!=true)
            {
                normalMusic.volume = 0.1f;
            }
            else
            {
                normalMusic.volume = 0;
            }
        }
        else if(v==null && GameObject.Find("SpectatorManager").GetComponent<SpectatorManager>().Spectator!=true)
        {
            foreach(FirstPersonMovement a in GameObject.FindObjectsOfType<FirstPersonMovement>())
            {
                if(a.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    player = a.gameObject.transform;
                }
            }
            enemy = this.gameObject.transform;
            v = player.gameObject.GetComponentInChildren<PostProcessVolume>();
            chaseMusic.Stop();
        }
        else if(GameObject.Find("SpectatorManager").GetComponent<SpectatorManager>().Spectator==true)
        {
            chaseMusic.volume--;
        }
    }
}
