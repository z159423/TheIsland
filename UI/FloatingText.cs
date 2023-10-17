using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;
using UnityEngine.UI;


public class FloatingText : MonoBehaviour
{

    [field: SerializeField] public Canvas canvas { get; private set; }

    [field: SerializeField] public Vector3 point { get; private set; }

    [SerializeField] RectTransform rectTransform;
    [SerializeField] Text outerText;
    [SerializeField] Image icon;

    [Header("Damage")]
    [SerializeField] Sprite sprDamagePostive;
    [SerializeField] Sprite sprDamageNegative;

    private void Update()
    {
        Vector2 screenPosition = CameraManager.Instance.GetMainCamera().WorldToScreenPoint(point);

        transform.position = screenPosition;
    }

    public void InitText(Vector3 point)
    {
        this.point = point;

        Vector2 screenPosition = CameraManager.Instance.GetMainCamera().WorldToScreenPoint(point);

        transform.position = screenPosition;

        icon.gameObject.SetActive(false);
    }

    public void SetDamageText(Color color, int value, bool postive)
    {
        icon.gameObject.SetActive(true);

        icon.sprite = postive ? sprDamagePostive : sprDamageNegative;

        outerText.color = color;
        outerText.text = (postive ? "+ " : "-") + $"{value}";

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public void SetPickupText(Color color, Item item, int count)
    {
        icon.gameObject.SetActive(true);

        icon.sprite = item.Icon;

        outerText.color = color;
        outerText.text = $"x {count}";

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public void SetEquipmentText(Color color, Equipment equipment)
    {
        icon.gameObject.SetActive(true);

        icon.sprite = equipment.Icon;

        outerText.color = color;
        outerText.text = $"{equipment.NameLocal.GetLocalizedString()}";

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public void SetPlainText(Color color, int size, string text)
    {

        outerText.text = text;
        outerText.color = color;
        outerText.fontSize = size;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
}
