using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphicsManager : MonoBehaviour
{
    public TMP_Dropdown myDrop;
    
    void Update()
    {
        PlayerPrefs.SetInt("PlayerQuality", myDrop.value);
    }

    public void OnAtualiazar()
    {
        if (myDrop.value == 0)
        {
            QualitySettings.SetQualityLevel(1, true);
        }
        if (myDrop.value == 1)
        {
            QualitySettings.SetQualityLevel(2, true);
        }
        if (myDrop.value == 2)
        {
            QualitySettings.SetQualityLevel(3, true);
        }
        if (myDrop.value == 3)
        {
            QualitySettings.SetQualityLevel(5, true);
        }    
    }

    void Start()
    {
        if(!PlayerPrefs.HasKey("PlayerQuality"))
        {
            myDrop.value = 3;
        }
        else{
            myDrop.value = PlayerPrefs.GetInt("PlayerQuality");
        }
    }
}
