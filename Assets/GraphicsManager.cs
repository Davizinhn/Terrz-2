using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GraphicsManager : MonoBehaviour
{
    public TMP_Dropdown myDrop;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown fullDropdown;
    public GameObject campadrao;

    public Slider musicSlider;
    public Slider soundSlider;
    public Slider masterSlider;
    public Slider voiceSlider;
    public AudioMixer musicMixer;
    
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
        Screen.SetResolution(int.Parse(res[0]), int.Parse(res[1]), fullDropdown.value==0?FullScreenMode.FullScreenWindow:FullScreenMode.Windowed);
        Debug.Log(int.Parse(res[0]));
        Debug.Log(int.Parse(res[1])); 
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
        for (int i = 0; i<resolutionDropdown.options.Count; i++)
        {
            string resAtual = Screen.width.ToString()+"x"+Screen.height.ToString();
            if(resolutionDropdown.options[i].text==resAtual)
            {
                resolutionDropdown.value=i;
                break;
            }
        }
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

        if(!PlayerPrefs.HasKey("MusicVol"))
        {
            musicSlider.value=0;
            PlayerPrefs.SetFloat("MusicVol", 0);
            soundSlider.value=0;
            PlayerPrefs.SetFloat("volSound", 0);
            UpdateMusicAudio();
        }
        else
        {
            musicSlider.value=PlayerPrefs.GetFloat("MusicVol");
            soundSlider.value=PlayerPrefs.GetFloat("volSound");


            Debug.Log(PlayerPrefs.GetFloat("volSound"));

            UpdateMusicAudio();
        }

        if(!PlayerPrefs.HasKey("VoiceVol"))
        {
            voiceSlider.value=0;
            PlayerPrefs.SetFloat("VoiceVol", 0);
            UpdateMusicAudio();
        }
        else
        {
            voiceSlider.value=PlayerPrefs.GetFloat("VoiceVol");
            UpdateMusicAudio();
        }

        if(!PlayerPrefs.HasKey("MasterVol"))
        {
            masterSlider.value=0;
            PlayerPrefs.SetFloat("MasterVol", 0);
            UpdateMusicAudio();
        }
        else
        {
            masterSlider.value=PlayerPrefs.GetFloat("MasterVol");
            UpdateMusicAudio();
        }



    }

    public void UpdateMusicAudio()
    {
        musicMixer.SetFloat("musicVol", musicSlider.value);
        musicMixer.SetFloat("soundVol", soundSlider.value);
        musicMixer.SetFloat("masterVol", masterSlider.value);
        musicMixer.SetFloat("voiceVol", voiceSlider.value);
    }

    public void Depois()
    {
        PlayerPrefs.SetFloat("volSound", soundSlider.value);
    }

    public void Depois1()
    {
        PlayerPrefs.SetFloat("MusicVol", musicSlider.value);

    }

    public void Depois2()
    {
        PlayerPrefs.SetFloat("MasterVol", masterSlider.value);

    }

    public void Depois3()
    {
        PlayerPrefs.SetFloat("VoiceVol", voiceSlider.value);

    }
}
