using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminarPartida : MonoBehaviour
{
    public void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag=="Player" && !col.gameObject.GetComponent<FirstPersonMovement>().isDead)
        {
            col.gameObject.GetComponent<FirstPersonMovement>().Morrer(false);
        }
    }
}
