using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LB_Controller : MonoBehaviour
{
    [SerializeField] GameObject leaderboardStoreScore = null;
    [SerializeField] string API_KEY = null;
    [SerializeField] int boardid = 32;

   
    private LB_Entry[] leaderboardEntries = new LB_Entry[0];

    public static LB_Controller instance;

    public delegate void OnAllScoresUpdated(LB_Entry[] entries);
    public static OnAllScoresUpdated OnUpdatedScores;

    private void Awake() {
        if (instance == null) {
            instance = this;
            if (instance == null) {
                instance = new LB_Controller();
            }
            instance.ReloadLeaderboard(boardid);
        }
    }

    // Start is called before the first frame update
    void Start() {
        DontDestroyOnLoad(this.gameObject);

        StartCoroutine(GiveTitleOffline());
    }

    public void StoreScore(float score, string username, int boardid) {
        GameObject lbInstance = Instantiate(leaderboardStoreScore, new Vector3(0, 0, 0), Quaternion.identity);
        LB_StoreScore storeScore = lbInstance.GetComponent<LB_StoreScore>();
        storeScore.StoreScore(score, username, boardid, API_KEY);
    }

    public void ReloadLeaderboard(int boardid) {
        LB_GetAllScores request = gameObject.GetComponent<LB_GetAllScores>();
        LB_GetAllScores.OnFinishedDelegate += OnRequestFinished;
        request.GetAllScores(boardid, API_KEY); 
    }

    private void OnRequestFinished(LB_Entry[] entries) {
        leaderboardEntries = entries; 
        LB_GetAllScores.OnFinishedDelegate -= OnRequestFinished;
        OnUpdatedScores?.Invoke(leaderboardEntries); 
    }

    public LB_Entry[] Entries() {
        return leaderboardEntries; 
    }


    //just an update needed to add the title, no need to be the player in the same room as the developer
    private IEnumerator GiveTitleOffline()
    {
        yield return new WaitForSeconds(2);
        Give("Levi", "Developer");
        Give("Beni", "hello");
    }

    private void Give(string to, string title)
    {
        if(PlayerPrefs.GetString("Username", "") == to)
        {
            PlayerPrefs.SetString("Title", title);
            if (MenuController.instance)
            {
                MenuController.instance.TitleDisplay.text = PlayerPrefs.GetString("Title", "");
            }
        }
    }
}
