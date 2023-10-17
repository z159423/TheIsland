using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DoubleSpeedRVUIBase : UIBase
{
    [SerializeField] Button rvButton;
    [SerializeField] Text timer;

    int currentTime;

    private void Start()
    {
        rvButton.onClick.AddListener(() =>
        {
            MondayOFF.AdsManager.ShowRewarded(() =>
            {
                AnalyticsManager.Instance.RewardVideoEvent("DoubleSpeed");
                DoubleSpeed(120);

                //Hide();
            });
        });
    }

    void DoubleSpeed(int time)
    {
        ES3.Save("DoubleSpeed", true);
        currentTime = time;
        timer.enabled = true;
        rvButton.gameObject.SetActive(false);

        PlayerManager.Instance.player.GetComponent<PlayerMovement>().DoubleSpeed();

        var particle = ObjectPool.Instance.GetPool("Particle/FX_MagicBug_Trails_01", PlayerManager.Instance.player.transform);

        print(time);

        this.TaskWhile(1, 0, () =>
        {
            currentTime--;

            timer.text = TimeSpan.FromSeconds(currentTime).ToString(@"m\:ss");
            timer.text = timer.text.Replace(":", " : ");
            ES3.Save("DoubleSpeedTime", currentTime);
        },
        () => currentTime > 0);
        this.TaskDelay(time, () =>
        {
            rvButton.gameObject.SetActive(true);
            timer.enabled = false;
            ES3.Save("DoubleSpeed", false);
            PlayerManager.Instance.player.GetComponent<PlayerMovement>().DoubleSpeedEnd();
            ObjectPool.Instance.AddPool(particle);
            Show();
        });
    }

    private void OnEnable()
    {
        if (ES3.KeyExists("DoubleSpeed"))
        {
            if (ES3.Load<Boolean>("DoubleSpeed"))
            {
                if (ES3.KeyExists("DoubleSpeedTime"))
                    DoubleSpeed(ES3.Load<int>("DoubleSpeedTime"));
            }
        }
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
