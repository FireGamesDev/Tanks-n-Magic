using System.Collections;
using System.Collections.Generic;
using TankScripts;
using UnityEngine;
using Settings;
using Photon.Pun;

public class BuildSystem : MonoBehaviour
{
    public LayerMask layer;

    public TankShooting tankShootingScript;

    private GameObject previewGameObject = null;
    private Prewiew previewScript = null;
    private float mouseWheelRotation;
    private float mouseX;
    private float mouseY;

    public float stickTolerance = 1f;

    public bool isBuilding = false;
    private bool pauseBuilding = false;
    private bool canShootAfterBuild = false; // in the pause menu if we press left click we still dont shoot, thanks to this  variable

    [Header("CursorAnim")]
    public Animator reticleAnim;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && isBuilding) //cancel
        {
            CancelBuild();
            if (!ResumeMenu.GameIsPaused) // when the game is unpaused
            {
                tankShootingScript.canShoot = true; //can shoot if exit the build mode
            } 
        }

        if (Input.GetMouseButtonDown(0) && isBuilding) //build
        {
            if (previewScript.GetSnapped())
            {
                BuildIt();
                canShootAfterBuild = true;
            }
            return; // must return, if don't after build the tank immediatly shoots, because we press left click (but we press left click to build)
        }

        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Q)) && canShootAfterBuild)
        {
            tankShootingScript.canShoot = true; //can shoot if exit the build mode
            canShootAfterBuild = false;
        }

        if (isBuilding)
        {
            if (pauseBuilding)
            {
                mouseX = Input.GetAxis("Mouse X"); // if you want less stickness to the snap point,
                mouseY = Input.GetAxis("Mouse Y"); // in the project settings/ Input Manager then decrease the sensitivity (Mouse X, Mouse Y)
                                                   // or you can decrease the stcikTolerance value

                if (Mathf.Abs(mouseX) >= stickTolerance || Mathf.Abs(mouseY) >= stickTolerance)
                {
                    pauseBuilding = false;
                }
            }
            else
            {
                BuildRay();
            }
        }

        if (previewGameObject != null)
        {
            RotateFromMouseWheel();
        }
    }

    private void RotateFromMouseWheel()
    {
        mouseWheelRotation = Input.mouseScrollDelta.y;
        previewGameObject.transform.Rotate(Vector3.up, mouseWheelRotation * 90f);
    }

    public void NewBuild(GameObject _go)
    {
        previewGameObject = PhotonNetwork.Instantiate(_go.name, Input.mousePosition, Quaternion.identity, 0);
        previewScript = previewGameObject.GetComponent<Prewiew>();
        isBuilding = true;
    }

    public void CancelBuild()
    {
        PhotonNetwork.Destroy(previewGameObject);
        previewGameObject = null;
        previewScript = null;
        isBuilding = false;
    }

    public void BuildIt()
    {
        if (reticleAnim)
        {
            reticleAnim.SetTrigger("DoubleShoot");
        }
        previewScript.Place();
        previewGameObject = null;
        previewScript = null;
        isBuilding = false;
    }

    public void PauseBuild(bool _value)
    {
        pauseBuilding = _value;
    }

    private void BuildRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100f, layer))
        {
            Vector3 pos = new Vector3(hit.point.x, previewGameObject.transform.localScale.y / 2f, hit.point.z);
            previewGameObject.transform.position = pos;
        }
    }

    public void DestroyPreview()
    {
        if (isBuilding)
        {
            PhotonNetwork.Destroy(previewGameObject);
            previewGameObject = null;
            previewScript = null;
            isBuilding = false;
        }
    }
}
