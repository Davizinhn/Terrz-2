using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public GameObject Loading;
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.LevelLoadingProgress != 0 && PhotonNetwork.LevelLoadingProgress != 1)
        {
            slider.value = PhotonNetwork.LevelLoadingProgress+0.2f;
            Loading.SetActive(true);
        }
        else{
            slider.value = 0f;
            Loading.SetActive(false);
        }
    }
}
