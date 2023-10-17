using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDropTable", menuName = "Game/ItemDropTable")]
public class ItemDropTableScriptable : VerificationObject
{
    [field: SerializeField] public ItemDropTable[] dropTable { get; private set; }

    public override void Verification()
    {
        foreach (var item in dropTable)
        {
            if (item.Item == null)
            {
                Debug.LogError($"Drop Table Item Null : {name}");
                break;
            }
        }
    }
}
