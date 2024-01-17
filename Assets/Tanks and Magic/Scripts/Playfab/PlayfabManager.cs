using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayfabManager : MonoBehaviour
{
    [SerializeField] private GameObject usernameScreen;
    [SerializeField] private TMP_InputField UserNameInput;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private TMP_Text UsernameDisplay;

    private void Start()
    {

        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            if (PlayerPrefs.GetString("Username", "") != "")
            {
                string currentPlayerName = PlayerPrefs.GetString("Username", "");
                Login(currentPlayerName);
            }
            else
            {
                usernameScreen.SetActive(true);
            }
        }
    }

    #region Login
    public void Login(string username)
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest
        {
            CustomId = username,
            CreateAccount = true, // Create a new account if it doesn't exist
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful!");
        if (PlayerPrefs.GetString("Username", "Guest") != "Guest")
        {
            // Set the display name after successful login
            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetUsernameResult, OnApiCallError);
        }
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login failed: " + error.ErrorMessage);
    }

    private void OnGetUsernameResult(GetAccountInfoResult result)
    {
        if (result.AccountInfo.TitleInfo.DisplayName == "")
            UpdateDisplayName(PlayerPrefs.GetString("Username", "Guest"));
        else
        {
            string displayName = PlayerPrefs.GetString("Username", "");
            UsernameDisplay.text = displayName;
        }
    }
    private void OnApiCallError(PlayFabError error)
    {
        // Handle API call errors here
        UnityEngine.Debug.LogError("PlayFab API call error: " + error.ErrorMessage);
    }

    private void UpdateDisplayName(string displayName)
    {
        UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        UserNameInput.text = displayName;

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateDisplayNameSuccess, OnUpdateDisplayNameFailure);
    }

    private void OnUpdateDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        PlayerPrefs.SetString("Username", UserNameInput.text);

        Debug.Log("Display name updated successfully!");

        usernameScreen.SetActive(false);
    }

    private void OnUpdateDisplayNameFailure(PlayFabError error)
    {
        Debug.LogError("Failed to update display name: " + error.ErrorMessage);

        errorText.text = error.ErrorMessage;
    }

    public void Register()
    {
        string username = UserNameInput.text;

        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            UpdateDisplayName(username);
        }
        else
        {
            LoginWithCustomIDRequest request = new LoginWithCustomIDRequest
            {
                CustomId = username,
                CreateAccount = true // Create a new account if it doesn't exist
            };

            PlayFabClientAPI.LoginWithCustomID(request, OnRegisterSuccess, OnRegisterFailure);
        }
    }

    void OnRegisterSuccess(LoginResult result)
    {
        print("registered");
        //messageText.text = ("Logged In!");

        UpdateDisplayName(UserNameInput.text);
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        UnityEngine.Debug.LogError("Login failed: " + error.ErrorMessage);
        errorText.text = error.ErrorMessage;
    }
    #endregion
}
