using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FloatingTextManager : SingletonStatic<FloatingTextManager>
{
    ///<summary>
    /// 아이템 픽업 텍스트 생성
    ///</summary>
    public void GenerateFloatingText_ItemPickUp(Item item, int count, Vector3 point)
    {
        var text = Generate(point);

        if (text != null)
            text.GetComponentInParent<FloatingText>().SetPickupText(Color.white, item, count);
    }

    ///<summary>
    /// 데미지 텍스트 생성
    ///</summary>
    public void GenearteFloatingText_Damage(int value, Color color, Vector3 point, bool postive)
    {
        var text = Generate(point);

        if (text != null)
            text.GetComponentInParent<FloatingText>().SetDamageText(color, value, postive);
    }

    ///<summary>
    /// 장비 텍스트 생성
    ///</summary>
    public void GenerateFloatingText_Equipment(Equipment equipment, Vector3 point)
    {
        var text = Generate(point);

        if (text != null)
            text.GetComponentInParent<FloatingText>().SetEquipmentText(Color.white, equipment);
    }

    public void GenerateCustomText(string value, Color color, Vector3 point, int size)
    {
        var text = Generate(point);

        if (text != null)
            text.GetComponentInParent<FloatingText>().SetPlainText(color, size, value);
    }

    /// <summary>
    /// 커스텀 알람
    /// </summary>
    public void GenerateCustomAlert(Vector3 pos, string path)
    {
        var ui = ObjectPool.Instance.GetPool(path, UIManager.Instance.transform);
        ui.transform.position = pos;
        ui.transform.localScale = Vector3.zero;

        var seq = DOTween.Sequence();
        seq.Join(ui.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutBack));
        seq.Join(ui.transform.DOMove(ui.transform.position.AddY(50f), 0.5f));
        seq.Append(ui.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutBack));
        seq.AppendCallback(() => ObjectPool.Instance.AddPool(ui));
    }

    /// <summary>
    /// 재료 부족
    /// </summary>
    public void GenerateMaterialAlert(Vector3 pos)
    {
        var ui = ObjectPool.Instance.GetPool("UI/Material Alert", UIManager.Instance.transform);
        ui.transform.position = pos;
        ui.transform.localScale = Vector3.zero;

        var seq = DOTween.Sequence();
        seq.Join(ui.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutBack));
        seq.Join(ui.transform.DOMove(ui.transform.position.AddY(50f), 0.5f));
        seq.Append(ui.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutBack));
        seq.AppendCallback(() => ObjectPool.Instance.AddPool(ui));
    }

    /// <summary>
    /// 건물 없음
    /// </summary>
    public void GenerateBuildingExist(Vector3 pos)
    {
        var ui = ObjectPool.Instance.GetPool("UI/Building Exist", UIManager.Instance.transform);
        ui.transform.position = pos;
        ui.transform.localScale = Vector3.zero;

        var seq = DOTween.Sequence();
        seq.Join(ui.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutBack));
        seq.Join(ui.transform.DOMove(ui.transform.position.AddY(50f), 0.5f));
        seq.Append(ui.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutBack));
        seq.AppendCallback(() => ObjectPool.Instance.AddPool(ui));
    }

    /// <summary>
    /// 아이템 제작
    /// </summary>
    public void GenerateCraftItem(Vector3 pos, Sprite icon)
    {
        var ui = ObjectPool.Instance.GetPool("UI/Craft Alert", UIManager.Instance.transform);
        ui.transform.position = pos;
        ui.transform.localScale = Vector3.zero;
        ui.transform.Find("Icon").GetComponent<Image>().sprite = icon;

        var seq = DOTween.Sequence();
        seq.Join(ui.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutBack));
        seq.Join(ui.transform.DOMove(ui.transform.position.AddY(50f), 0.5f));
        seq.Append(ui.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutBack));
        seq.AppendCallback(() => ObjectPool.Instance.AddPool(ui));
    }



    Text Generate(Vector3 point)
    {
        var obj = ObjectPool.Instance.GetPool("UI/FloatingText", UIManager.Instance.transform);
        obj.SetActive(false);

        var text = obj.GetComponentInChildren<Text>();

        obj.GetComponent<FloatingText>().InitText(point + Vector3.up);
        obj.SetActive(true);

        obj.GetComponent<Animator>().SetTrigger("trigger");

        StartCoroutine(hide(obj));

        obj.transform.SetSiblingIndex(0);


        return text;
    }

    IEnumerator hide(GameObject text)
    {
        yield return new WaitForSeconds(1.5f);

        ObjectPool.Instance.AddPool(text);
    }
}
