using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioSource sfx;

    void Start()
    {
        sfx.Play();
    }
}
