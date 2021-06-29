using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Prewiew : MonoBehaviourPunCallbacks
{
    public GameObject prefab;
    private MeshRenderer myRend;
    public Material goodMat;
    public Material badMat;

    private BuildSystem buildSystem;

    private bool isSnapped = false;
    public bool isFoundation = false;

    public List<string> tagsISnapTo = new List<string>();

    private void Start()
    {
        buildSystem = GameObject.FindObjectOfType<BuildSystem>();
        myRend = GetComponent<MeshRenderer>();
        ChangeColor();
    }

    public void Place()
    {
        PhotonNetwork.Instantiate(prefab.name, transform.position, transform.rotation, 0);
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    private void ChangeColor()
    {
        if (isSnapped)
        {
            myRend.material = goodMat;
        }
        else
        {
            myRend.material = badMat;
        }

        if (isFoundation)
        {
            myRend.material = goodMat;
            isSnapped = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < tagsISnapTo.Count; i++)
        {
            string currentTag = tagsISnapTo[i];

            if(other.tag == currentTag)
            {
                buildSystem.PauseBuild(true); //this is how you snap
                transform.position = other.transform.position;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < tagsISnapTo.Count; i++)
        {
            string currentTag = tagsISnapTo[i];

            if(other.tag == currentTag)
            {
                isSnapped = false;
                ChangeColor();
            }
        }
    }

    public bool GetSnapped()
    {
        return isSnapped;
    }
}
