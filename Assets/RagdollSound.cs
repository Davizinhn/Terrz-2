using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollSound : MonoBehaviour
{
    public Rigidbody rb;
    void Awake()
    {
        rb.AddForce(Vector3.back*2, ForceMode.Impulse);
    }
}
