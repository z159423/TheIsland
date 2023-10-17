using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ItemManager : SingletonStatic<ItemManager>
{
    public const int MAX_DUMMY = 30;

    [RuntimeInitializeOnLoadMethod]
    static void LoadInit() { var t = Instance; }

    /// <summary>
    /// 아이템 테이블로 드랍아이템 생성
    /// </summary>
    public void GenerateNewItemDrop(ItemDropTableScriptable dropTable, Vector3 position)
    {
        for (int i = 0; i < dropTable.dropTable.Length; i++)
        {
            if (dropTable.dropTable[i].GetDropSuccess())
            {
                var count = dropTable.dropTable[i].GetDropAmount();

                FloatingTextManager.Instance.GenerateFloatingText_ItemPickUp(dropTable.dropTable[i].Item, count, position + new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)));
                SaveManager.Instance.InventoryController.GetItem(dropTable.dropTable[i].Item.ID, count);

                for (int j = 0; j < count; j++)
                {
                    var clone = ObjectPool.Instance.GetPool("DropItemPrefab");

                    clone.transform.position = position + (Random.insideUnitSphere.SetY(0f) * 0.5f);

                    var item = clone.GetComponent<ObtainableItem>();
                    item.InitAttract(dropTable.dropTable[i].Item.ID);

                    item.GetRigidbody().AddForce(Random.insideUnitSphere * 6f, ForceMode.Impulse);
                    item.GetRigidbody().AddTorque(Random.insideUnitSphere * 20f, ForceMode.Impulse);
                }
            }
        }
    }

    public ObtainableItem GenerateItemDrop(int id, Vector3 position, bool rigid = false)
    {
        var clone = ObjectPool.Instance.GetPool("DropItemPrefab");

        clone.transform.position = position;
        var item = clone.GetComponent<ObtainableItem>();
        item.InitAttract(id);

        if (rigid)
        {
            item.GetRigidbody().drag = 3;
            item.GetRigidbody().AddForce(GetRandomDirection() * 6f, ForceMode.Impulse);
            item.GetRigidbody().AddTorque(Random.insideUnitSphere * 20f, ForceMode.Impulse);
        }

        return item;
    }

    private Vector3 GetRandomDirection()
    {
        Vector2 randomCirclePoint = Random.insideUnitCircle.normalized;
        Vector3 randomDirection = new Vector3(randomCirclePoint.x, Mathf.Abs(randomCirclePoint.y), randomCirclePoint.y);
        return randomDirection;
    }

    public ObtainableItem GenerateDummyItem(int id, Vector3 pos, bool rigid = false)
    {
        var clone = ObjectPool.Instance.GetPool("DropItemPrefab");

        clone.transform.position = pos;

        var item = clone.GetComponent<ObtainableItem>();
        item.InitDummy(id);

        if (rigid)
        {
            item.GetRigidbody().drag = 0;
            item.GetRigidbody().AddForce(Random.insideUnitSphere * 6f, ForceMode.Impulse);
            item.GetRigidbody().AddTorque(Random.insideUnitSphere * 20f, ForceMode.Impulse);
        }

        return item;
    }
}
