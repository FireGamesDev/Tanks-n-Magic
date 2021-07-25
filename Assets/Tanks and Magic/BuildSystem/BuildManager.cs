using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public GameObject foundation; //preview

    public BuildSystem buildSystem;
    public TankScripts.TankShooting tankShootingScript;

    private UnityEngine.UI.Image buildCooldownIcon;
    private float buildCooldown;
    private float coolDown = 5f;

    private void Start()
    {
        buildCooldown = coolDown;
        if (GameObject.Find("BuildSystemDisplay"))
        {
            buildCooldownIcon = GameObject.Find("BuildSystemDisplay").GetComponent<UnityEngine.UI.Image>();
        }
    }

    private void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Aram"))
        {
            if (GameObject.FindGameObjectWithTag("GameHandler"))
            {
                if (GameObject.FindGameObjectWithTag("GameHandler").GetComponent<GameHandler>().stopped)
                {
                    return;
                }
            }
        }

        buildCooldown -= Time.deltaTime;

        if (buildCooldownIcon)
        {
            buildCooldownIcon.fillAmount = coolDown / coolDown - buildCooldown / coolDown;
        }
        else
        {
            if (GameObject.Find("BuildSystemDisplay"))
            {
                buildCooldownIcon = GameObject.Find("BuildSystemDisplay").GetComponent<UnityEngine.UI.Image>();
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && !buildSystem.isBuilding && buildCooldown <= 0 && !Chat.ChatInputField.GetComponent<UnityEngine.UI.Mask>().showMaskGraphic)
        {
            buildCooldown = coolDown;
            buildSystem.NewBuild(foundation);
            tankShootingScript.canShoot = false; //can't shoot if enters the build mode
        }
    }
}
