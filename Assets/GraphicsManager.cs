using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.PostProcessing;

public class GraphicsManager : MonoBehaviour
{
    public TMP_Dropdown myDrop;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown fullDropdown;
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

    public void AtualizarResolution()
    {
        if(!pode)
            return;
        string[] res = resolutionDropdown.options[resolutionDropdown.value].text.Split("x");
        Screen.SetResolution(int.Parse(res[0]), int.Parse(res[0]), fullDropdown.value==0?FullScreenMode.FullScreenWindow:FullScreenMode.Windowed);
    }

    bool pode=false;
    void PopulateResolutionDropdown()
    {
        Resolution[] resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        foreach (Resolution resolution in resolutions)
        {
            string option = resolution.width + "x" + resolution.height;
            if(resolutionDropdown.options.Count>0)
            {
                if(resolutionDropdown.options[resolutionDropdown.options.Count-1].text!=option)
                {
                    resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(option));
                }
            }
            else
            {
                resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(option));
            }
        }

        resolutionDropdown.RefreshShownValue();

            for(int i = 0; i<resolutionDropdown.options.Count; i++)
            {
                if(resolutionDropdown.options[i].text == Screen.currentResolution.width+"x"+Screen.currentResolution.height)
                {
                    resolutionDropdown.value=i;
                    break;
                }
            }
        fullDropdown.value=Screen.fullScreenMode==FullScreenMode.FullScreenWindow?0:1;
        pode=true;
    }

    void Start()
    {
        PopulateResolutionDropdown();
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
