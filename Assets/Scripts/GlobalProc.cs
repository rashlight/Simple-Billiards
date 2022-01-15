using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GlobalProc : MonoBehaviour
{
    public static void MuteEvent(UnityEngine.Events.UnityEventBase ev)
    {
        int count = ev.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            ev.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
        }
    }
    public static void UnmuteEvent(UnityEngine.Events.UnityEventBase ev)
    {
        int count = ev.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            ev.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        }
    }
    
    public static bool CheckCueBallsThreshold(Snooker sn)
    {
        for (int i = 0; i < UnityEngine.EventSystems.EventSystem.current.GetComponent<GlobalVar>().poolBoxObject.transform.childCount; i++)
        {
            Rigidbody2D childRb = GlobalProc.GetGlobalVar().
                poolBoxObject.transform.GetChild(i).GetComponent<Rigidbody2D>();
            if (childRb == null || childRb.velocity.magnitude > sn.velocityThreshold) return false;
        }
        return true;
    }

    public static void AddScore(Text scoreText, int amount)
    {
        GlobalVar.score += amount;
        GlobalVar.deltaScore += amount;
        string positiveDeltaString = GlobalVar.deltaScore >= 0f ? "+" : "";
        scoreText.text = GlobalVar.score.ToString() + " (" + positiveDeltaString +  GlobalVar.deltaScore.ToString() + ")";
    }

    public static void ResetDeltaScore(Text scoreText)
    {
        GlobalVar.deltaScore = -1; // Since a shot is always take 1 point
        scoreText.text = GlobalVar.score.ToString();
    }

    public static void EditMessage(Text messageText, string text)
    {
        messageText.text = text;
    }
    
    public static void SetHighscore(int value)
    {
        PlayerPrefs.SetInt("highscore", value);
        PlayerPrefs.Save();
    }
    public static int LoadHighscore()
    {
        if (PlayerPrefs.HasKey("highscore"))
        {
            return PlayerPrefs.GetInt("highscore");
        }
        else return 0;
    }

    public static bool CheckPoolAvail()
    {
        return true;
    }

    public static UnityEngine.EventSystems.EventSystem GetCurrentEventSystem()
    {
        return UnityEngine.EventSystems.EventSystem.current;
    }

    public static GlobalVar GetGlobalVar()
    {
        return GetCurrentEventSystem().GetComponent<GlobalVar>();
    }

    private void ResetGameStates()
    {
        GlobalVar.isWin = false;
        if (PlayerPrefs.HasKey("highscore"))
        {
            EditMessage(GetGlobalVar().messageText, "Game has started! Highscore: " + PlayerPrefs.GetInt("highscore"));
        }
        if (PlayerPrefs.GetInt("Precision") > 7)
        {
            GetGlobalVar().messageText.fontSize -= 2;
        }
    }

    public void Start()
    {
        ResetGameStates();
    }
}
