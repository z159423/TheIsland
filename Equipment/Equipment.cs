using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Sirenix.OdinInspector;
using System.Linq;

public enum EquipmentType
{
    HAND = 0, SWORD = 1, PICKAXE = 2, AXE = 3, CHEST = 4
}

[CreateAssetMenu(fileName = "Equipment", menuName = "Game/Equipment")]
public class Equipment : VerificationObject
{
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public EquipmentType Type { get; private set; }
    [field: SerializeField] public int Tier { get; private set; }

    [field: Space]

    [field: BoxGroup("Localization")][field: SerializeField] public LocalizedString NameLocal { get; private set; }
    [field: BoxGroup("Localization")][field: SerializeField] public LocalizedString DescLocal { get; private set; }

    [field: Space]

    [field: BoxGroup("Stat")][field: SerializeField] public int Damage { get; private set; }
    [field: BoxGroup("Stat")][field: SerializeField] public int Armor { get; private set; }

    [field: Space]

    [field: BoxGroup("asset")][field: SerializeField] public Sprite Icon { get; private set; }
    [field: BoxGroup("asset")][field: SerializeField] public GameObject EquipmentPrefab { get; private set; }
    [field: BoxGroup("asset")][field: ReadOnly][field: SerializeField] public string CraftExpectedTime { get; private set; }
    [field: BoxGroup("asset")][field: SerializeField] public ItemStack[] Recipe { get; private set; }
    [field: BoxGroup("asset")][field: SerializeField] public Mesh mesh { get; private set; }
    [field: BoxGroup("asset")][field: SerializeField] public Equipment NextEquipment { get; private set; }
    [field: BoxGroup("asset")][field: SerializeField][field: Range(0f, 10f)] public float Time { get; private set; }

    [field: Space]

    [field: ShowIf("@this.Type == EquipmentType.HAND ||this.Type == EquipmentType.SWORD || this.Type == EquipmentType.PICKAXE || this.Type == EquipmentType.AXE")]
    [field: SerializeField] public WeaponCollider weaponCollider { get; private set; }
    [field: ShowIf("@this.Type == EquipmentType.HAND ||this.Type == EquipmentType.SWORD || this.Type == EquipmentType.PICKAXE || this.Type == EquipmentType.AXE")]
    [field: SerializeField] public string attackTriggerStr { get; private set; }
    [field: ShowIf("@this.Type == EquipmentType.HAND ||this.Type == EquipmentType.SWORD || this.Type == EquipmentType.PICKAXE || this.Type == EquipmentType.AXE")]
    [field: SerializeField] public bool trailOn { get; private set; }

    [field: Space]

    [field: SerializeField] public int RV_RequireLVL { get; private set; }

    private void OnValidate()
    {
        Verification();

        // Expected

        if (Recipe.Length > 0)
        {
            var craft = Resources.LoadAll<CraftScriptable>("Craft");

            var times = new float[Recipe.Length];
            float collect = 0f;

            for (int i = 0; i < Recipe.Length; i++)
            {
                var find = craft.FirstOrDefault(f => f.Result.Item == Recipe[i].Item);
                if (find != null)
                    times[i] = find.Time * Recipe[i].Count;
                else
                    collect += Recipe[i].Count / 3;
            }

            CraftExpectedTime = $"예상 {Mathf.Max(times.Max(), collect):F0}초";
        }        
    }

    public override void Verification()
    {
        foreach (var item in Recipe)
        {
            if (item.Item == null)
                Debug.LogError($"{ID} Equipment wrong recipe");
        }

        if (Resources.LoadAll<Equipment>("Equipment").FirstOrDefault(f => f != this && f.ID == ID) != null)
            Debug.LogError($"Equipment ID Conflict {ID}");
    }
}
