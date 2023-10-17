using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro_TouchToStart : MonoBehaviour
{
    public void Touch()
    {
        UIManager.Instance.ShowScreen("UI/TouchPreventionPanel");

        AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.UI, "Intro_TouchToStart");
        PlayerManager.Instance.player.IntroStart();

        Destroy(gameObject);
    }
}
