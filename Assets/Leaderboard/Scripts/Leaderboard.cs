using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] int boardID = 140;
    public GameObject rowPrefab;
    public GameObject loadingText;
    public Transform rowsParent;

    public UnityEngine.UI.ScrollRect leaderboardScroll;

    private GameObject currentElement;

    private void OnEnable()
    {
        rowsParent.gameObject.SetActive(false);
        if (PlayerPrefs.GetString("Username", "Player") != "Player" && PlayerPrefs.GetInt("Kills",0) != 0)
        {
            //store score on the leaderboard
            LB_Controller.instance.StoreScore(PlayerPrefs.GetInt("Kills"), PlayerPrefs.GetString("Username"), boardID);
        }
        LB_Controller.OnUpdatedScores += OnLeaderboardUpdated;
        StartCoroutine(DownloadScores());
    }

    IEnumerator DownloadScores()
    {
        loadingText.SetActive(true);
        yield return new WaitForSeconds(2);
        loadingText.SetActive(false);
        rowsParent.gameObject.SetActive(true);
        LB_Controller.instance.ReloadLeaderboard(boardID);
    }

    private void DownloadScoreFunc()
    {
        System.TimeSpan ts = System.TimeSpan.FromMilliseconds(2000); //wait for 2 sec
        LB_Controller.instance.ReloadLeaderboard(boardID);
    }

    private void OnLeaderboardUpdated(LB_Entry[] entries)
    {
        if (entries != null && entries.Length > 0)
        {
            if(rowsParent != null)
            {
                foreach (Transform entry in rowsParent)
                {
                    Destroy(entry.gameObject);
                }
            }

            foreach (LB_Entry entry in entries)
            {
                GameObject newGo = Instantiate(rowPrefab, rowsParent);
                TMPro.TMP_Text[] texts = newGo.GetComponentsInChildren<TMPro.TMP_Text>();

                if (entry.rank == 1)
                {
                    texts[0].color = Color.yellow;
                }
                if (entry.rank == 2)
                {
                    texts[0].color = Color.grey;
                }
                if (entry.rank == 3)
                {
                    texts[0].color = new Color(1.0f, 0.64f, 0.0f);
                }
                texts[1].color = texts[0].color;
                texts[2].color = texts[0].color;

                texts[0].text = entry.rank.ToString();
                texts[1].text = entry.name;
                texts[2].text = entry.points.ToString();

                if (entry.name.Equals(PlayerPrefs.GetString("Username")))
                {
                    texts[1].text = entry.name + " (You)";
                    texts[1].color = Color.yellow;
                    currentElement = newGo;
                }
            }
            if (currentElement)
            {
                SnapTo(currentElement.GetComponent<RectTransform>());
            }
        }
        else if (entries == null)
        {
            Debug.Log("ups something went wrong");
        }
    }

    private void OnDestroy()
    {
        LB_Controller.OnUpdatedScores -= OnLeaderboardUpdated;
    }

    public void SnapTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        if (leaderboardScroll)
        {
            leaderboardScroll.content.anchoredPosition =
                (Vector2)leaderboardScroll.transform.InverseTransformPoint(leaderboardScroll.content.position)
                - (Vector2)leaderboardScroll.transform.InverseTransformPoint(target.position);
        }
    }
}
