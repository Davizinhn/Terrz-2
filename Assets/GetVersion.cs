using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetVersion : MonoBehaviour
{
    void Start()
    {
        this.GetComponent<TMP_Text>().text="v"+Application.version.ToString();
    }

}
