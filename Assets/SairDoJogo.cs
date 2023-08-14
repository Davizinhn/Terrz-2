using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SairDoJogo : MonoBehaviour
{
    public GameObject Normal;
    public GameObject Configs;
    public GameObject Jogar1;
    public GameObject ConfigsGraphics;
    public GameObject ConfigsSound;

    public void SairJogo()
    {
        Application.Quit();
    }

    public void Jogar()
    {
        PlayAudioA();
        Normal.active=false;
        Jogar1.active=true;
    }

    public void Configurar()
    {
        PlayAudioA();
        Normal.active=false;
        Configs.active=true;
    }

    public void VoltarNormal()
    {
        PlayAudioA();
        Jogar1.active=false;
        Normal.active=true;
        Configs.active=false;
    }

    public void VoltarConfigs()
    {
        PlayAudioA();
        ConfigsGraphics.active=false;
        ConfigsSound.active=false;
        Configs.active=true;
    }
    

    public void PlayAudioA()
    {
        this.gameObject.GetComponent<AudioSource>().Play();
    }
}
