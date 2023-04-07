using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOutline : MonoBehaviour
{
    public Outline a;
    public Door b;

    void Update()
    {
        a.enabled = b.isOpen;
    }
}
