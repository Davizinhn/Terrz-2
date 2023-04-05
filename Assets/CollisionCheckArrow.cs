using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheckArrow : MonoBehaviour
{
    public bool Pode;

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if(collider2D.gameObject.name=="Certo")
        {
            Pode=true;
        }
    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        if(collider2D.gameObject.name=="Certo")
        {
            Pode=false;
        }
    }
}
