using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;

public class DiscordPresence : MonoBehaviour
{
    public Discord.Discord discord;
    public long appid;
    Discord.ActivityManager activityManager;
    Discord.Activity activity;
    Discord.OverlayManager overlayManager;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        discord=new Discord.Discord(appid, (System.UInt64)Discord.CreateFlags.Default);
        activityManager = discord.GetActivityManager();
        activity = new Discord.Activity{
            Details = "Loading",
            State = "",
            Assets = {
                LargeImage = "logo"
            }
        };
        activityManager.UpdateActivity(activity, (res) =>{
            if(res==Discord.Result.Ok)
            {
                Debug.Log("Discord status set!");
            }
            else
            {
                Debug.LogError("Discord status failed!");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        discord.RunCallbacks();
    }

    public void ChangePresence(string details, string state)
    {
        activity.Details = details;
        activity.State = state;
        activityManager.UpdateActivity(activity, (res) =>{
            if(res==Discord.Result.Ok)
            {
                Debug.Log("Discord status set!");
            }
            else
            {
                Debug.LogError("Discord status failed!");
            }
        });
    }

    void OnApplicationQuit()
    {
        discord.Dispose();
    }
}
