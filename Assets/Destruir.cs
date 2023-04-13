using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruir : MonoBehaviour
{
    void Update()
    {
        this.gameObject.GetComponent<ParticleSystem>().loop = false;
        if(!this.gameObject.GetComponent<ParticleSystem>().isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}
