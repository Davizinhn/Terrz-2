using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudarNPC : MonoBehaviour
{
    public Door door;

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag=="Monster" && !door.isOpen)
        {
            door.Mudar();
        }
    }
}
