using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Properties;

public class PlayfabAccountDeletion : MonoBehaviour
{
    [SerializeField] private GameObject usernameScreen;
    [SerializeField] private string apiEndpoint; //"https://asd12.playfabapi.com";
    [SerializeField] private string secretKey; //"asdddddddddddddddddddddddddddddddddddddddas";
    [SerializeField] private string namePlayerPrefKey;

    public void DeleteUserAccount()
    {
        GetPlayFabIdAndDeleteAccount(apiEndpoint);
        PlayFabClientAPI.ForgetAllCredentials();
    }
    private void GetPlayFabIdAndDeleteAccount(string apiEndpoint)
    {
        // Check if the player is authenticated
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            // Get the current PlayFab authentication ticket
            var request = new GetAccountInfoRequest();

            PlayFabClientAPI.GetAccountInfo(request,
                result =>
                {
                    string playFabId = null;
                    if (result != null && result.AccountInfo != null && result.AccountInfo.PlayFabId != null)
                    {
                        playFabId = result.AccountInfo.PlayFabId;
                        Debug.Log("PlayFab ID of the current user: " + playFabId);
                        // Use this PlayFab ID for further operations

                        // Call the routine to delete the account
                        if (!string.IsNullOrEmpty(playFabId))
                        {
                            StartCoroutine(DeletePlayerCoroutine(apiEndpoint, playFabId));
                        }
                        else
                        {
                            Debug.LogError("Player ID is empty or not set.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to get PlayFab ID for the current user.");
                    }
                },
                error =>
                {
                    Debug.LogError("Error getting PlayFab ID: " + error.ErrorMessage);
                });
        }
        else
        {
            Debug.LogError("Player is not logged in to PlayFab.");
        }
    }


    private IEnumerator DeletePlayerCoroutine(string apiEndpoint, string playerID)
    {
        if (string.IsNullOrEmpty(playerID))
        {
            Debug.LogError("Player ID is empty or not set.");
            yield break;
        }

        // Convert the Dictionary to a JSON string
        string jsonBody = "{\"PlayFabId\":\"" + playerID + "\"}";

        using (UnityWebRequest request = new UnityWebRequest(apiEndpoint + "/Admin/DeleteMasterPlayerAccount", "POST"))
        {
            // Set headers
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-SecretKey", secretKey);

            // Convert body to bytes and set it to the request
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // Send the request
            yield return request.SendWebRequest();

            // Check for errors
            if (request.isNetworkError)
            {
                string errorResponse = request.downloadHandler.text;
                Debug.LogError("Network Error: " + errorResponse);
            }
            else if (request.isHttpError)
            {
                string errorResponse = request.downloadHandler.text;
                Debug.LogError("HTTP Error: " + errorResponse);
            }
            else
            {
                // Request successful, log the response
                Debug.Log("Response: " + request.downloadHandler.text);
                PlayerPrefs.DeleteKey(namePlayerPrefKey);
                usernameScreen.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}
