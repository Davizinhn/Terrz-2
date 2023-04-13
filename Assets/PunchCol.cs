using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchCol : MonoBehaviour
{
    public void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag=="Generator" && !col.gameObject.GetComponent<Generator>().Quebrado && !GameObject.Find("GameManager").GetComponent<ManageGame>().foge)
        {
            col.gameObject.GetComponent<Generator>().QuebrarNormie();
            col.gameObject.GetComponent<Generator>().TocarExplosion();
        }
        if(col.gameObject.tag=="Player" && !col.gameObject.GetComponent<FirstPersonMovement>().isDead)
        {
            col.gameObject.GetComponent<FirstPersonMovement>().Morrer();
        }
    }
}
