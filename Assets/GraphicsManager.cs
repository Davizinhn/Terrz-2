using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.PostProcessing;

public class GraphicsManager : MonoBehaviour
{
    public TMP_Dropdown myDrop;
    public GameObject campadrao;
    
    void Update()
    {
        PlayerPrefs.SetInt("PlayerQuality", myDrop.value);
    }

    public void OnAtualiazar()
    {
        if (myDrop.value == 0)
        {
            PlayerPrefs.SetInt("PostProcess", 0);
            QualitySettings.SetQualityLevel(1, true);
        }
        if (myDrop.value == 1)
        {
            PlayerPrefs.SetInt("PostProcess", 0);
            QualitySettings.SetQualityLevel(2, true);
        }
        if (myDrop.value == 2)
        {
                        PlayerPrefs.SetInt("PostProcess", 1);
            QualitySettings.SetQualityLevel(3, true);
        }
        if (myDrop.value == 3)
        {
                        PlayerPrefs.SetInt("PostProcess", 1);
            QualitySettings.SetQualityLevel(5, true);
        }    
        if(PlayerPrefs.GetInt("PostProcess") == 0)
        {
            campadrao.GetComponent<PostProcessVolume>().enabled=false;
        }
        else
        {
            campadrao.GetComponent<PostProcessVolume>().enabled=true;
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

        if(!PlayerPrefs.HasKey("PostProcess"))
        {
            PlayerPrefs.SetInt("PostProcess", 1);
        }
    }
}
