using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfabManager : MonoBehaviour
{
    private void Start()
    {
        if (PlayerPrefs.GetString("Nickname", "") != "")
        {
            string currentPlayerName = PlayerPrefs.GetString("Username", "");
            Login(currentPlayerName);
        }
    }

    #region Login
    public void Login(string username)
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest
        {
            CustomId = username,
            CreateAccount = true // Create a new account if it doesn't exist
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful!");

        // Set the display name after successful login
        string displayName = PlayerPrefs.GetString("Nickname", "");
        UpdateDisplayName(displayName);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login failed: " + error.ErrorMessage);
    }

    private void UpdateDisplayName(string displayName)
    {
        UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateDisplayNameSuccess, OnUpdateDisplayNameFailure);
    }

    private void OnUpdateDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Display name updated successfully!");
    }

    private void OnUpdateDisplayNameFailure(PlayFabError error)
    {
        Debug.LogError("Failed to update display name: " + error.ErrorMessage);
    }
    #endregion
}
