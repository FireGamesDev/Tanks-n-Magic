using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMinimapCamPos : MonoBehaviour
{
    public GameObject playerCam;

    private void Start()
    {
        if (PlayerPrefs.GetString("IsGreen", "false").Equals("false"))
        {
            transform.Rotate(0f, 0f, 180f, Space.Self);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, playerCam.transform.position.z);
    }
}
