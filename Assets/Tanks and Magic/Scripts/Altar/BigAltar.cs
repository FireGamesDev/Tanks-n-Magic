using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigAltar : MonoBehaviour
{
    public GameObject altar;
    public Image barImage;
    public float coolDown = 3f; // cooldown to slow down OnTriggerStay update time
    private float waitCooldown;
    private float targetAmount;
    public float distance; // when the players are near to the crystal, the slider increases (kind of capture flag)

    private Animator anim;

    private void Start()
    {
        waitCooldown = coolDown;
        anim = altar.GetComponent<Animator>();
        barImage.fillAmount = 0f;
        targetAmount = 0f;
    }

    public void OnTriggerStay (Collider other)
    {
        targetAmount += 0.1f;
        
        if (targetAmount >= barImage.fillAmount)
        {
            waitCooldown -= Time.deltaTime;
            barImage.fillAmount = targetAmount * 0.04f * Time.deltaTime;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distance);
    }
}
