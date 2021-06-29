using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class EmojiManager : MonoBehaviour, IPunObservable
{
    public PhotonView photonView;

    public Sprite[] emojis;
    public GameObject BubbleSpeechObject;
    public GameObject emojiSprite;

    public AudioSource sfx;
    private float volume = 0.2f; //if the players spams the volume goes down to not be annoying

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                BubbleSpeechObject.SetActive(true);
                photonView.RPC("SendNewMessage", RpcTarget.AllBuffered, 0);
                StopCoroutine("Remove");
                StartCoroutine("Remove");
                sfx.volume -= volume;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                BubbleSpeechObject.SetActive(true);
                photonView.RPC("SendNewMessage", RpcTarget.AllBuffered, 1);
                StopCoroutine("Remove");
                StartCoroutine("Remove");
                sfx.volume -= volume;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                BubbleSpeechObject.SetActive(true);
                photonView.RPC("SendNewMessage", RpcTarget.AllBuffered, 2);
                StopCoroutine("Remove");
                StartCoroutine("Remove");
                sfx.volume -= volume;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                BubbleSpeechObject.SetActive(true);
                photonView.RPC("SendNewMessage", RpcTarget.AllBuffered, 3);
                StopCoroutine("Remove");
                StartCoroutine("Remove");
                sfx.volume -= volume;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                BubbleSpeechObject.SetActive(true);
                photonView.RPC("SendNewMessage", RpcTarget.AllBuffered, 4);
                StopCoroutine("Remove");
                StartCoroutine("Remove");
                sfx.volume -= volume;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                BubbleSpeechObject.SetActive(true);
                photonView.RPC("SendNewMessage", RpcTarget.AllBuffered, 5);
                StopCoroutine("Remove");
                StartCoroutine("Remove");
                sfx.volume -= volume;
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                BubbleSpeechObject.SetActive(true);
                photonView.RPC("SendNewMessage", RpcTarget.AllBuffered, 6);
                StopCoroutine("Remove");
                StartCoroutine("Remove");
                sfx.volume -= volume;
            }
        }
    }

    [PunRPC]
    private void SendNewMessage(int emojiIndex)
    {
        sfx.pitch = Random.Range(0f, 3f);
        sfx.PlayOneShot(sfx.clip);
        emojiSprite.GetComponent<Image>().sprite = emojis[emojiIndex];
    }

    IEnumerator Remove()
    {
        yield return new WaitForSeconds(4f);
        sfx.volume = 1f;
        BubbleSpeechObject.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(BubbleSpeechObject.activeSelf);
        } 
        else if (stream.IsReading)
        {
            BubbleSpeechObject.SetActive((bool)stream.ReceiveNext());
        }
    }
}
