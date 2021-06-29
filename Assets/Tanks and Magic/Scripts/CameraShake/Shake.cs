using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public Animator[] camAnim;

    public void CamShake()
    {
        camAnim[0].SetTrigger("shake");
        camAnim[1].SetTrigger("shake");
        camAnim[2].SetTrigger("shake");
        camAnim[3].SetTrigger("shake");
    }
}
