using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.AI;

public class Player : NPCObject
{
    [field: FoldoutGroup("참조")][field: SerializeField] protected override Animator animator { get; set; }
    [field: FoldoutGroup("참조")][field: SerializeField] public override HealthBar HpBar { get; protected set; }
    [field: FoldoutGroup("참조")][field: SerializeField] public override Vector3 HpBarPosition { get; protected set; } = new Vector3(0, 2.1f, 0);
    [field: FoldoutGroup("참조")][field: SerializeField] public Transform playerObtainTrans { get; private set; }

    private PlayerAttack playerattack;

    //임시
    [field: BoxGroup("스탯")][field: SerializeField] public override int maxHP { get; protected set; } = 20;
    [field: BoxGroup("스탯")][field: SerializeField] public override int currentHP { get; protected set; }

    [field: Space]

    [field: BoxGroup("스탯")][field: SerializeField] public int maxHunger { get; private set; } = 100;
    [field: BoxGroup("스탯")][field: SerializeField] public int currentHunger { get; private set; } = 100;

    [field: BoxGroup("스탯")][field: SerializeField] public int maxStamina { get; private set; } = 100;
    [field: BoxGroup("스탯")][field: SerializeField] public int currentStamina { get; private set; } = 100;

    [field: Space]

    [field: BoxGroup("스탯")][field: SerializeField] public override bool isDie { get; protected set; } = false;

    [field: BoxGroup("스탯")][field: SerializeField] public float currentAnimationSpeed { get; private set; } = 1;


    [field: BoxGroup("ragdoll")][field: SerializeField] public override bool ragdollOnDie { get; protected set; } = true;
    [field: BoxGroup("ragdoll")][field: ShowIf("ragdollOnDie")][field: SerializeField] public override Collider[] ragdollColliders { get; protected set; }

    public override Coroutine hpBarHideCoroutine { get; protected set; }

    TaskUtil.WhileTaskMethod playerConsumeHungerRepeat = null;
    TaskUtil.WhileTaskMethod hpRegenTask = null;
    TaskUtil.DelayTaskMethod hpRegenCool = null;
    const float hpRegenRepeatTime = 1f;
    const float hpRegenCoolTime = 5f;

    bool hpRegen = true;

    FloatingIcon floatingIcon;

    Sequence damageSeq;

    bool spawnPermanentCompanion;

    public void SetSPC(bool isSpawned) => spawnPermanentCompanion = isSpawned;

    private UIBase[] introOffUI;

    private void Awake()
    {

        PlayerManager.Instance.InitPlayer(this);
    }

    private void Start()
    {
        currentHP = maxHP;

        InitHpBar();

        playerattack = GetComponent<PlayerAttack>();
        playerattack.Init();

        maxHP = SaveManager.Instance.PlayerController.GetPlayerMaxHp();
        currentHP = SaveManager.Instance.PlayerController.GetPlayerCurrentHp();
        maxHunger = SaveManager.Instance.PlayerController.GetPlayerMaxHunger();
        currentHunger = SaveManager.Instance.PlayerController.GetPlayerCurrentHunger();
        maxStamina = SaveManager.Instance.PlayerController.GetPlayerMaxStamina();
        currentStamina = SaveManager.Instance.PlayerController.GetPlayerCurrentStamaina();

        HungerUI.Instance?.SetMaxHunger(maxHunger);
        HungerUI.Instance?.SetCurrentHunger(currentHunger);

        StaminaUI.Instance?.SetMaxStamina(maxStamina);
        StaminaUI.Instance?.SetCurrentStamina(currentStamina);

        // OnChangeHP();

        // playerConsumeHungerRepeat = this.TaskWhile(PlayerSaveData.hungerCunsumeRepeatTime, 0, () => ConsumeHunger(1));

        hpRegenTask = this.TaskWhile(hpRegenRepeatTime, 0, () => RegenHp(1));


        if (SaveManager.Instance.PlayerController.GetRevive())
        {
            Heal(10);
            Eat(maxHP);
        }

        if (currentHP <= 0)
            Die();

        if (!SaveManager.Instance.PlayerController.GetIntro())
        {
            animator.SetTrigger("Intro");
            GetComponent<PlayerMovement>().canMove = false;
            SaveManager.Instance.PlayerController.SetIntro(true);
            CameraManager.Instance.SetCustomCamera("IntroVirtualCamera", -1, 2);

            introOffUI = UIManager.Instance.GetComponent<Canvas>().GetComponentsInChildren<UIBase>();
            foreach (var ui in introOffUI)
            {
                ui.gameObject.SetActive(false);
            }

            CameraManager.Instance.DisableCullingMask("UI");
            UIManager.Instance.ShowScreen("UI/Intro_TouchToStart");
        }

        HpBar.gameObject.SetActive(false);
    }

    public void SpawnPermanentCompanion()
    {
        if (spawnPermanentCompanion)
            return;

        spawnPermanentCompanion = true;

        var companion = Instantiate(ResourceManager.Instance.GetResource<GameObject>("Companion/Companion"), Util.GetRandomPositionAroundTarget(transform, 2, 3), Quaternion.identity);

        companion.GetComponentInChildren<Companion>().InitPermanentCompanion(ResourceManager.Instance.GetResource<CompanionDatas>("Companion/Companion Data").GetRandomCompanionSkins());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ConsumeHunger(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Eat(5);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TakeDamage(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Heal(5);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5) && !SaveManager.Instance.PlayerController.GetHungerSystemApply())
        {
            SaveManager.Instance.PlayerController.HungerSystemApply();
        }

        if (Input.GetKeyDown(KeyCode.Alpha6) && !SaveManager.Instance.PlayerController.GetStaminaSystemApply())
        {
            SaveManager.Instance.PlayerController.StaminaSystemApply();
        }

        if (Input.GetKeyDown(KeyCode.X))
            print(playerattack.GetArmor());


        if (Input.GetKeyDown(KeyCode.V))
            SaveManager.Instance.PlayerController.GetExp(30);

        if (Input.GetKeyDown(KeyCode.M))
            SpawnPermanentCompanion();
    }

    public override void InitHpBar()
    {
        var clone = ObjectPool.Instance.GetPool("UI/Hp bar", transform);
        HpBar = clone.GetComponentInChildren<HealthBar>(true);
        HpBar.GetComponentInParent<Billboard>().GetComponent<RectTransform>().anchoredPosition = HpBarPosition;
        HpBar.SetMaxHealth(maxHP);
        HpBar.SetHealth(currentHP);
        ObjectPool.Instance.AddClearReserve(clone);
    }

    public override void TakeDamage(int damage, NPCObject from = null, string cause = null)
    {
        if (isDie)
            return;

        damage -= playerattack.GetArmor();

        // int levelDiff = 0;

        // if (from != null)
        // {
        //     if (from.TryGetComponent<Enemy>(out Enemy enemy))
        //     {
        //         levelDiff = SaveManager.Instance.PlayerController.GetPlayerLevel() - enemy.stat.Level;
        //     }
        // }

        // damage -= levelDiff;

        damage = Mathf.Clamp(damage, 1, int.MaxValue);

        currentHP -= damage;

        if (hpRegenCool != null)
            hpRegenCool.Kill();

        hpRegen = false;
        hpRegenCool = this.TaskDelay(hpRegenCoolTime, () => hpRegen = true);

        if (currentHP <= 0)
        {
            Die();
            if (!string.IsNullOrEmpty(cause))
                AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.DEATH, cause);
        }
        else
        {
            if (damageSeq != null)
                damageSeq.Kill();

            //transform.rotation = Quaternion.identity;
            //transform.localScale = Vector3.one;

            damageSeq = DOTween.Sequence();
            damageSeq.Join(transform.DOPunchRotation(new Vector3(5f, transform.localRotation.y, 5f), 0.3f));
        }

        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        OnChangeHP();

        FloatingTextManager.Instance.GenearteFloatingText_Damage(damage, Color.red, transform.position, false);
        HapticManager.Instance.Haptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
    }

    public override void Heal(int heal)
    {
        if (currentHP != maxHP)
        {
            currentHP += heal;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            FloatingTextManager.Instance.GenearteFloatingText_Damage(heal, Color.green, transform.position, true);
            OnChangeHP();
        }
    }

    public override void Die(Transform from = null)
    {
        isDie = true;

        foreach (Collider collider in ragdollColliders)
        {
            collider.enabled = true;
            collider.GetComponent<Rigidbody>().useGravity = true;
        }
        animator.enabled = false;

        PlayerManager.Instance.OnPlayerDie();
    }

    public override void OnChangeHP()
    {
        HpBar.SetMaxHealth(maxHP);
        HpBar.SetHealth(currentHP);

        HpBar.gameObject.SetActive(true);

        if (hpBarHideCoroutine != null)
            StopCoroutine(hpBarHideCoroutine);

        hpBarHideCoroutine = StartCoroutine(HideHpBar());

        IEnumerator HideHpBar()
        {
            yield return new WaitForSeconds(3f);

            HpBar.gameObject.SetActive(false);
        }

        SaveManager.Instance.PlayerController.SavePlayerStatus(this);
    }

    public void Revive()
    {
        //foreach (Collider collider in ragdollColliders)
        //{
        //    collider.enabled = false;
        //    collider.GetComponent<Rigidbody>().useGravity = false;
        //}
        //animator.enabled = true;

        SaveManager.Instance.PlayerController.SetRevive(true);

        var respawnData = SaveManager.Instance.PlayerController.GetRespawnData();
        UIManager.Instance.ScreenFadeIn(() =>
        {
            ObjectPool.Instance.CleanTempRoot();
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(respawnData.World);
        }, false);
    }

    [ContextMenu("Get Ragdoll Colliders")]
    void GetRagdollCollider()
    {
        ragdollColliders = transform.GetComponentsInChildren<Collider>();
    }

    public void FullHunger()
    {
        currentHunger = maxHunger;

        HungerUI.Instance.SetCurrentHunger(currentHunger);

        HungerFloating();

        SaveManager.Instance.PlayerController.SavePlayerStatus(this);
    }

    public void Eat(int value)
    {

        if (isDie || !SaveManager.Instance.PlayerController.GetHungerSystemApply())
            return;

        currentHunger += value;

        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);

        HungerFloating();

        HungerUI.Instance.SetCurrentHunger(currentHunger);

        SaveManager.Instance.PlayerController.SavePlayerStatus(this);

        SoundManager.Instance.PlaySFX("Eat2", 0.5f, Random.Range(0.8f, 1.2f));
    }

    void HungerFloating()
    {
        if (floatingIcon == null)
        {
            floatingIcon = ObjectPool.Instance.GetPool("UI/Floating Icon", transform).GetComponent<FloatingIcon>();
            floatingIcon.Init(FloatingIcon.IconType.HUNGER);
            floatingIcon.transform.localPosition = Vector3.up * 4f;
        }

        if (((float)currentHunger / maxHunger) < 0.25f)
            floatingIcon.Show();
        else
            floatingIcon.Hide();
    }

    public void ConsumeHunger(int value)
    {
        if (isDie || !SaveManager.Instance.PlayerController.GetHungerSystemApply())
            return;

        if (currentHunger <= 0)
            TakeDamage(1, cause: "HUNGER");

        currentHunger -= value;

        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);

        HungerFloating();

        HungerUI.Instance.SetCurrentHunger(currentHunger);
        SaveManager.Instance.PlayerController.SavePlayerStatus(this);

        //print(((float)currentHunger/(float)maxHunger));
        if (((float)currentHunger / (float)maxHunger) > 0.7f)
        {
            print("배부픔 효과 : 체력회복");
            Heal(2);
        }
    }

    public void RegenHp(int value)
    {
        if (!hpRegen)
            return;

        Heal(1);
    }


    public void ChargeStamina(int value)
    {
        if (isDie || !SaveManager.Instance.PlayerController.GetStaminaSystemApply())
            return;

        currentStamina += value;

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        StaminaUI.Instance.SetCurrentStamina(currentStamina);
        SaveManager.Instance.PlayerController.SavePlayerStatus(this);
    }

    public void UseStamina(int value)
    {
        if (isDie || !SaveManager.Instance.PlayerController.GetStaminaSystemApply())
            return;

        currentStamina -= value;

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        StaminaUI.Instance.SetCurrentStamina(currentStamina);
        SaveManager.Instance.PlayerController.SavePlayerStatus(this);

    }

    public override void ragdollEnable(Transform from = null)
    {
        return;
    }

    public void IntroStart()
    {
        CameraManager.Instance.OffAllCustomCamera();
        CameraManager.Instance.SetCustomCamera("IntroVirtualCamera", 5, 2);

        animator.SetTrigger("IntroNext");
    }

    public void IntroEnd()
    {
        TouchPreventionUI.Instance.GetComponent<TouchPreventionUIBase>().Hide();
        GetComponent<PlayerMovement>().canMove = true;

        AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.UI, "Intro_End");

        foreach (var ui in introOffUI)
        {
            ui.gameObject.SetActive(true);
        }

        CameraManager.Instance.EnableCullingMask("UI");

        this.TaskDelay(1.5f, () => { if (!SaveManager.Instance.QuestController.GetHeadQuestToggle()) QuestManager.Instance.QuestUI.ButtonHeadQuestToggle(); });
    }

    public void AddIntroOffBtn(Button btn)
    {

    }

}
