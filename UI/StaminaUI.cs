using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StaminaUI : SingletonInstance<StaminaUI>
{
    [SerializeField] private Slider slider;

    public void SetMaxStamina(int value)
    {
        slider.maxValue = value;
    }

    public void SetCurrentStamina(int value)
    {
        slider.value = value;
    }
}
