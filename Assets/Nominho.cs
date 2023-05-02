using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Nominho : MonoBehaviour
{
    public float maxDistance;

    void Update()
    {
        if(Camera.main != null)
        {
            if (GameObject.FindObjectOfType<SpectatorManager>()!=null)
            {
                if(GameObject.FindObjectOfType<SpectatorManager>().Spectator)
                {
                    this.gameObject.GetComponent<Animator>().SetBool("Ativado", true);
                }
                else
                {
                    this.gameObject.GetComponent<Animator>().SetBool("Ativado", Vector3.Distance(Camera.main.transform.position, this.gameObject.transform.position) < maxDistance);
                }
                
            }
            else
            {
                this.gameObject.GetComponent<Animator>().SetBool("Ativado", Vector3.Distance(Camera.main.transform.position, this.gameObject.transform.position) < maxDistance);
            }
            
        }
        else
        {
            this.gameObject.GetComponent<Animator>().SetBool("Ativado", true);
        }

    }
}
