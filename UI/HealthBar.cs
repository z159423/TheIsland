using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public Text level;

    public void SetMaxHealth(int health, int level = -1)
    {
        if (level == -1)
        {
            this.level.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            this.level.text = level.ToString();
            this.level.transform.parent.gameObject.SetActive(true);
        }


        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        slider.value = health;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void SetHealth(float health)
    {
        slider.value = health;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
