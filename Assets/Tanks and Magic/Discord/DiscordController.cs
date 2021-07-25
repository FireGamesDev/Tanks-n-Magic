
#if UNITY_EDITOR
using System;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using Discord;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Photon.Pun;

public class DiscordController : MonoBehaviour
{
    private long lastTimestamp = 0;

    public long applicationId;
    public Discord.Discord discord;
    public string details;
    public string state;

    private ActivityManager activityManager;

    public void Init()
    {
        discord = new Discord.Discord(applicationId, (long)CreateFlags.Default);
        activityManager = discord.GetActivityManager();

        activityManager.OnActivityJoin += secret => {
            Console.WriteLine("OnJoin {0}", secret);
            //Sends this off to a Activity callback named here as 'UpdateActivity' passing in the discord instance details and lobby details
            UpdateActivity(true);
        };

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Lobby")
        {
            UpdateActivity(true);
        }
        else UpdateActivity(false);

        EditorApplication.update += Update;
        EditorSceneManager.sceneOpened += SceneOpened;
    }

    private void SceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
    {
        lastTimestamp = GetTimestamp();
        if (scene.name == "Lobby")
        {
            UpdateActivity(true);
        }
        else UpdateActivity(false);

    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (discord != null)
            discord.RunCallbacks();
    }

    public void UpdateActivity(bool in_lobby)
    {
        if (discord == null)
            Init();

        activityManager = discord.GetActivityManager();
        Activity activity;


        string[] link = new string[2] {"",""};
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Menu")
        {
            link = state.Split('/');
        }

        lastTimestamp = GetTimestamp();

        if (in_lobby)
        {
            lastTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            activity = new Activity
            {
                State = state,
                Details = details,
                Timestamps =
                {
                    Start = lastTimestamp
                },
                Assets =
                {
                    LargeImage = "large_image", // Larger Image Asset Key
                },
                Party =
                {
                    Id = PhotonNetwork.CurrentRoom.Name,
                    Size = {
                        CurrentSize = PhotonNetwork.CurrentRoom.PlayerCount,
                        MaxSize = PhotonNetwork.CurrentRoom.MaxPlayers,
                    },
                },
                Secrets =
                {
                    Join = "joinSecret",
                },
                Instance = true,
            };
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Menu")
        {
            activity = new Discord.Activity
            {
                State = "\"https://" + link[0] + "\n/" + link[1],
                Details = details,
                Timestamps =
                {
                    Start = lastTimestamp
                },
                Assets =
                {
                    LargeImage = "large_image", // Larger Image Asset Key
                    LargeText = "https://levimakesgames.itch.io/tanksn-magic"
                },
                Instance = true,
            };
        }
        else
        {
            activity = new Discord.Activity
            {
                State = state,
                Details = details,
                Timestamps =
                {
                    Start = lastTimestamp
                },
                Assets =
                {
                    LargeImage = "large_image", // Larger Image Asset Key
                    LargeText = "https://levimakesgames.itch.io/tanksn-magic"
                },
                Instance = true,
            };
        }

        activityManager.UpdateActivity(activity, (result) =>
        {
            if (result == Discord.Result.Ok)
            {
                Debug.Log("Discord state updated!");
            }
            else
            {
                Debug.LogError("Discord state update failed!");
            }
        });
    }

    public long GetTimestamp()
    {
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(EditorAnalyticsSessionInfo.elapsedTime);
        long timestamp = DateTimeOffset.Now.Add(timeSpan).ToUnixTimeSeconds();
        return timestamp;
    }

    private bool DiscordRunning()
    {
        Process[] processes = Process.GetProcessesByName("Discord");

        if (processes.Length == 0)
        {
            processes = Process.GetProcessesByName("DiscordPTB");

            if (processes.Length == 0)
            {
                processes = Process.GetProcessesByName("DiscordCanary");
            }
        }
        return processes.Length != 0;
    }

    private void Join()
    {
        
    }
}
#endif
