using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Nominho : MonoBehaviour
{
    public float maxDistance;

    void Update()
    {
        this.gameObject.GetComponent<Animator>().SetBool("Ativado", Vector3.Distance(Camera.main.transform.position, this.gameObject.transform.position) < maxDistance);
    }
}
