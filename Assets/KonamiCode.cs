using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KonamiCode : MonoBehaviour
{
    private List<KeyCode> konamiCodeSequence = new List<KeyCode>
    {
        KeyCode.UpArrow,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.B,
        KeyCode.A
    };

    public int konamiCodeIndex = 0;

    public Animator targetAnimator;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(konamiCodeSequence[konamiCodeIndex]))
            {
                konamiCodeIndex++;

                if (konamiCodeIndex == konamiCodeSequence.Count)
                {
                    GetComponent<AudioSource>().Play();
                    targetAnimator.Play("Rodando");
                    konamiCodeIndex = 0;
                }
            }
            else
            {
                konamiCodeIndex = 0;
            }
        }
    }
}
