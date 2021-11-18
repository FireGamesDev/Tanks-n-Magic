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
        buildCooldownIcon = GameObject.Find("BuildSystem").GetComponent<UnityEngine.UI.Image>();
    }

    private void Update()
    {
        buildCooldown -= Time.deltaTime;

        if (buildCooldownIcon)
        {
            buildCooldownIcon.fillAmount = coolDown / coolDown - buildCooldown / coolDown;
        }
        else buildCooldownIcon = GameObject.Find("BuildSystem").GetComponent<UnityEngine.UI.Image>();

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && !buildSystem.isBuilding && buildCooldown <= 0 && !Chat.ChatInputField.GetComponent<UnityEngine.UI.Mask>().showMaskGraphic)
        {
            buildCooldown = coolDown;
            buildSystem.NewBuild(foundation);
            tankShootingScript.canShoot = false; //can't shoot if enters the build mode
        }
    }
}
