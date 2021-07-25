using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : MonoBehaviour
{
    public TMPro.TMP_Text skinText;
    public TMPro.TMP_Text skinTextColor;

    [Header("TankParts")]
    public Renderer turret;
    public Renderer left_track;
    public Renderer right_track;

    private int nextSkin;
    //limits
    private int pink = 10;
    private int blue = 50;
    private int orange = 100;
    private int yellow = 200;
    private int white = 1000;

    //materials
    [Header("Materials")]
    public Material pink_mat;
    public Material blue_mat;
    public Material orange_mat;
    public Material yellow_mat;
    public Material white_mat;
    public Material default_mat_green;
    public Material default_mat_purple;

    private int kills;
    public bool isMenu = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Menu"))
        {
            if (GetComponent<Photon.Pun.PhotonView>().IsMine)
            {
                kills = PlayerPrefs.GetInt("Kills", 0);
                GetComponent<Photon.Pun.PhotonView>().RPC("SetSkinRPC", Photon.Pun.RpcTarget.AllBuffered, kills);
            }
        }

        if (isMenu)
        {
            kills = PlayerPrefs.GetInt("Kills", 0);
            SetSkin(PlayerPrefs.GetInt("Kills", 0));
        }
    }

    
    private void SetSkin(int kills)
    {
        if (kills >= white)
        {
            if (skinText)
            {
                skinText.text = "Kills: " + kills.ToString();
                skinTextColor.text = "";
            }
            SetMaterial(white_mat);
            return;
        }

        if (kills >= yellow)
        {
            nextSkin = white;
            if (skinTextColor)
            {
                skinTextColor.text = "Next: White";
            }
            SetMaterial(yellow_mat);
            return;
        }

        if (kills >= orange)
        {
            nextSkin = yellow;
            if (skinTextColor)
            {
                skinTextColor.text = "Next: Yellow";
            }
            SetMaterial(orange_mat);
            return;
        }

        if (kills >= blue)
        {
            nextSkin = orange;
            if (skinTextColor)
            {
                skinTextColor.text = "Next: Orange";
            }
            SetMaterial(blue_mat);
            return;
        }

        if (kills >= pink)
        {
            nextSkin = blue;
            if (skinTextColor)
            {
                skinTextColor.text = "Next: Blue";
            }
            SetMaterial(pink_mat);
            return;
        }

        if (kills >= 0)
        {
            nextSkin = pink;
            if (skinTextColor)
            {
                skinTextColor.text = "Next: Pink";
            }

            if (CompareTag("PurpleTank"))
            {
                SetMaterial(default_mat_purple);
            }
            else if (CompareTag("GreenTank"))
            {
                SetMaterial(default_mat_green);
            }
            else
            {
                SetMaterialDefault(default_mat_purple, default_mat_green);
            }

        }
    }
    
    [Photon.Pun.PunRPC]
    private void SetSkinRPC(int kills)
    {
        if (kills >= white)
        {
            if (skinText)
            {
                skinText.text = "Kills: " + kills.ToString();
                skinTextColor.text = "";
            }
            SetMaterial(white_mat);
            return;
        }

        if (kills >= yellow)
        {
            nextSkin = white;
            if (skinTextColor)
            {
                skinTextColor.text = "Next: White";
            }
            SetMaterial(yellow_mat);
            return;
        }

        if (kills >= orange)
        {
            nextSkin = yellow;
            if (skinTextColor)
            {
                skinTextColor.text = "Next: Yellow";
            }
            SetMaterial(orange_mat);
            return;
        }

        if (kills >= blue)
        {
            nextSkin = orange;
            if (skinTextColor)
            {
                skinTextColor.text = "Next: Orange";
            }
            SetMaterial(blue_mat);
            return;
        }

        if (kills >= pink)
        {
            nextSkin = blue;
            if (skinTextColor)
            {
                skinTextColor.text = "Next: Blue";
            }
            SetMaterial(pink_mat);
            return;
        }

        if (kills >= 0)
        {
            nextSkin = pink;
            if (skinTextColor)
            {
                skinTextColor.text = "Next: Pink";
            }

            if (CompareTag("PurpleTank"))
            {
                SetMaterial(default_mat_purple);
            }
            else if (CompareTag("GreenTank"))
            {
                SetMaterial(default_mat_green);
            }
            else
            {
                SetMaterialDefault(default_mat_purple, default_mat_green);
            }

        }
    }

    private void SetMaterial(Material mat)
    {
        if (skinText)
        {
            skinText.text = "Kills: " + kills.ToString() + "/" + nextSkin.ToString();
        }

        turret.material = mat;

        Material[] matArray = left_track.materials;
        matArray[1] = mat;
        left_track.materials = matArray;

        matArray = right_track.materials;
        matArray[1] = mat;
        right_track.materials = matArray;
    }

    private void SetMaterialDefault(Material mat, Material mat2)
    {
        if (skinText)
        {
            skinText.text = "Kills: " + kills.ToString() + "/" + nextSkin.ToString();
        }

        if (Random.Range(1, 3) == 2)
        {
            Material swap = mat;
            mat = mat2;
            mat2 = swap;
        }

        turret.material = mat;

        Material[] matArray = left_track.materials;
        matArray[1] = mat;
        left_track.materials = matArray;

        matArray = right_track.materials;
        matArray[1] = mat2;
        right_track.materials = matArray;
    }
}
