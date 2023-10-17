using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using Sirenix.OdinInspector;

public enum PLAYER_ACTION_STATE
{
    DEFAULT = 0,
    ATTACK = 1,
    //벌목
    FELLING = 2,
    MINING = 3
}

public class PlayerAttack : MonoBehaviour, IAttackTriggerController
{
    [BoxGroup("참조")][SerializeField] private Animator animator;
    [BoxGroup("참조")][SerializeField] private Collider bodyCollider;
    [BoxGroup("참조")][SerializeField] private PlayerObjectDetector playerObjectDetector;
    public bool isAttacking = false;

    [field: HorizontalGroup("Weapon", 0.3f, LabelWidth = 10)]
    [field: BoxGroup("Weapon/Sword")][field: SerializeField] public int currentSwordID { get; private set; } = 1;
    [field: BoxGroup("Weapon/Sword")][field: SerializeField] public PlayerEquipment currentSword { get; private set; }
    [field: BoxGroup("Weapon/Pickaxe")][field: SerializeField] public int currentPickaxeID { get; private set; } = 2;
    [field: BoxGroup("Weapon/Pickaxe")][field: SerializeField] public PlayerEquipment currentPickaxe { get; private set; }
    [field: BoxGroup("Weapon/Axe")][field: SerializeField] public int currentAxeID { get; private set; } = 3;
    [field: BoxGroup("Weapon/Axe")][field: SerializeField] public PlayerEquipment currentAxe { get; private set; }
    [field: BoxGroup("Weapon/Chest")][field: SerializeField] public int currentChestArmorID { get; private set; }
    [field: BoxGroup("Weapon/Chest")][field: SerializeField] public PlayerEquipment currentChestArmor { get; private set; }

    [Space]

    [SerializeField] private PlayerEquipment currentWeapon;

    [Space]

    [field: EnumToggleButtons][field: SerializeField] public PLAYER_ACTION_STATE playerCurrentState;

    [Space]

    [SerializeField] public List<Collider> triggerdCollider = new List<Collider>();

    [Space]

    [SerializeField] private ParticleSystem[] footStepParticles;

    [Space]

    [BoxGroup("참조")][SerializeField] private Transform handPoint;
    [BoxGroup("참조")][SerializeField] private Transform grapPoint;


    public void Init()
    {
        GenearteEquipment(EquipmentType.SWORD, SaveManager.Instance.PlayerController.GetSwordId());
        GenearteEquipment(EquipmentType.PICKAXE, SaveManager.Instance.PlayerController.GetPickaxeId());
        GenearteEquipment(EquipmentType.AXE, SaveManager.Instance.PlayerController.GetAxeId());
        GenearteEquipment(EquipmentType.CHEST, SaveManager.Instance.PlayerController.GetChestArmorId());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isAttacking == false)
        {
            //SwingStart();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (currentSword.equipment.NextEquipment != null)
                GenearteEquipment(currentSword.equipment.Type, currentSword.equipment.NextEquipment.ID);
            if (currentPickaxe.equipment.NextEquipment != null)
                GenearteEquipment(currentPickaxe.equipment.Type, currentPickaxe.equipment.NextEquipment.ID);
            if (currentAxe.equipment.NextEquipment != null)
                GenearteEquipment(currentAxe.equipment.Type, currentAxe.equipment.NextEquipment.ID);
            if (currentChestArmor.equipment.NextEquipment != null)
                GenearteEquipment(currentChestArmor.equipment.Type, currentChestArmor.equipment.NextEquipment.ID);

            currentPickaxe.transform.localScale = currentPickaxe.transform.localScale + new Vector3(0.15f, 0.15f, 0.15f);

            currentPickaxe.transform.position = currentPickaxe.transform.position + (handPoint.position - grapPoint.position);

            FloatingTextManager.Instance.GenerateCustomText("Level Up!", Color.yellow, transform.position + (Vector3.up * 1.5f), 150);

            var particle = ObjectPool.Instance.GetPool("Particle/LevelupCylinderYellow");

            particle.transform.position = transform.position;

            GetComponentInChildren<Animator>().SetFloat("Speed", 1.25f);
        }
    }

    private void LateUpdate()
    {
        if (isAttacking)
            return;

        if (playerObjectDetector.GetTriggerdObjects().Count > 0)
        {
            PlayerStateMachine();
        }
    }

    private void PlayerStateMachine()
    {
        Collider targetCollider = null;

        foreach (Collider collider in playerObjectDetector.GetTriggerdObjects())
        {
            if (collider.TryGetComponent<WorldEnviromentObject>(out WorldEnviromentObject worldEnviromentObject))
            {
                targetCollider = collider;
                break;
            }
        }

        foreach (Collider collider in playerObjectDetector.GetTriggerdObjects())
        {
            if (collider.TryGetComponent<Enemy>(out Enemy e1))
            {
                targetCollider = collider;
                break;
            }
        }

        if (targetCollider == null)
            return;

        if (targetCollider.TryGetComponent<Enemy>(out Enemy enemy))
        {
            playerCurrentState = PLAYER_ACTION_STATE.ATTACK;
            currentWeapon = currentSword;
            SwordAttack();
        }
        else if (targetCollider.TryGetComponent<WorldEnviromentObject>(out WorldEnviromentObject worldEnviromentObject))
        {
            switch (worldEnviromentObject.GetBreakEquipmentType())
            {
                case EquipmentType.PICKAXE:
                    playerCurrentState = PLAYER_ACTION_STATE.MINING;
                    currentWeapon = currentPickaxe;
                    SwordAttack();
                    break;

                case EquipmentType.AXE:
                    playerCurrentState = PLAYER_ACTION_STATE.FELLING;
                    currentWeapon = currentAxe;
                    SwordAttack();
                    break;

                default:
                    Debug.LogError("해당되는 타입이 없습니다.");
                    break;
            }
        }
        else
        {
            Debug.LogError("해당되는 플레이어 행동패턴이 없습니다");
        }

        Equip();
    }

    private void SwordAttack()
    {
        isAttacking = true;
        animator.SetTrigger(currentWeapon.equipment.attackTriggerStr);
    }

    public void ActionEnd()
    {
        isAttacking = false;

        triggerdCollider.Clear();
        playerObjectDetector.ClearObjectList();

        UnEquip();
    }

    public void AttackTriggerOn()
    {
        currentWeapon.ColliderOn();
        currentWeapon.WeaponTrailOn();
    }

    public void AttackTriggerOff()
    {
        currentWeapon.ColliderOff();
        currentWeapon.WeaponTrailOff();
    }

    ///<summary>
    /// 상황에 맞는 장비 장착 [전투 : 칼] [벌목 : 도끼] [채광 : 곡갱이]
    ///</sumamry>
    public void Equip()
    {
        switch (playerCurrentState)
        {
            case PLAYER_ACTION_STATE.ATTACK:
                currentSword.gameObject.SetActive(true);
                animator.SetLayerWeight(1, 1);
                break;
            case PLAYER_ACTION_STATE.FELLING:
                currentAxe.gameObject.SetActive(true);
                animator.SetLayerWeight(1, 1);
                break;
            case PLAYER_ACTION_STATE.MINING:
                currentPickaxe.gameObject.SetActive(true);
                animator.SetLayerWeight(1, 1);
                break;

            default:

                Debug.LogError("해당되는 무기 타입이 없습니다.");
                break;
        }
    }

    ///<summary>
    /// 상황 종료후 장비 장착 해제
    ///</sumamry>
    public void UnEquip()
    {
        switch (playerCurrentState)
        {
            case PLAYER_ACTION_STATE.ATTACK:
                currentSword.gameObject.SetActive(false);
                animator.SetLayerWeight(1, 0);
                break;
            case PLAYER_ACTION_STATE.FELLING:
                currentAxe.gameObject.SetActive(false);
                animator.SetLayerWeight(1, 0);
                break;
            case PLAYER_ACTION_STATE.MINING:
                currentPickaxe.gameObject.SetActive(false);
                break;

            default:

                Debug.LogError("해당되는 무기 타입이 없습니다.");
                break;
        }

        playerCurrentState = PLAYER_ACTION_STATE.DEFAULT;
    }

    public void GenearteEquipment(EquipmentType type, int id)
    {
        var res = Resources.LoadAll<Equipment>("Equipment").ToList();
        var find = res.Find(f => f != this && f.ID == id);

        if (find == null)
        {
            Debug.LogError("해당되는 ID의 장비를 찾지 못했습니다.");
            return;
        }

        switch (type)
        {
            case EquipmentType.SWORD:
                currentSword.ChangeEquipment(find);
                currentSwordID = find.ID;
                break;

            case EquipmentType.PICKAXE:
                currentPickaxe.ChangeEquipment(find);
                currentPickaxeID = find.ID;
                break;

            case EquipmentType.AXE:
                currentAxe.ChangeEquipment(find);
                currentAxeID = find.ID;
                break;

            case EquipmentType.CHEST:
                currentChestArmor.ChangeEquipment(find);
                currentChestArmorID = find.ID;
                break;

            default:
                Debug.LogError("해당되는 타입의 장비가 없습니다.");
                break;
        }

        SaveManager.Instance.PlayerController.SavePlayerCurrentEquipment(type, find.ID);
    }

    public void footstep1()
    {
        footStepParticles[0].Play();
    }

    public void footstep2()
    {
        footStepParticles[1].Play();
    }

    public void CustomAnimationEvent(string eventString)
    {
        SendMessage(eventString);
    }

    public int GetArmor()
    {
        int total = 0;

        total += currentChestArmor.equipment.Armor;

        return total;
    }
}
