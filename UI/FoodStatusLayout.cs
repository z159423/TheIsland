using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FoodStatusLayout : MonoBehaviour
{
    public Image hungerImage;
    public Text hungerValueText;

    public Image HpImage;
    public Text HpValueText;


    public void Init(int hungerValue = 0, int HpValue = 0)
    {
        if (hungerValue > 0)
        {
            hungerImage.gameObject.SetActive(true);
            hungerValueText.text = hungerValue.ToString();
        }
        else
        {
            hungerImage.gameObject.SetActive(false);
        }

        if (HpValue > 0)
        {
            HpImage.gameObject.SetActive(true);
            HpValueText.text = HpValue.ToString();
        }
        else
        {
            HpImage.gameObject.SetActive(false);
        }
    }
}
