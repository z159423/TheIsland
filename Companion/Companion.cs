using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.AI;

public enum COMPANION_ACTION_STATE
{
    DEFAULT = 0,
    ATTACK = 1,
    //벌목
    FELLING = 2,
    MINING = 3
}

public enum COMPANION_TYPE
{
    NORMAL = 0,
    PERMANENT = 1
}

public enum COMPANION_SEARCH_TYPE
{
    ENEMY = 0,
    RESOURCE = 1
}

public class Companion : NPC
{

    [TitleGroup("Companion")][SerializeField] private COMPANION_TYPE companionType;
    [TitleGroup("Companion")][SerializeField] private float returnDist = 8f;
    [TitleGroup("Companion")][SerializeField] private float collectResourceDist = 1f;
    [TitleGroup("Companion")][SerializeField] private float searchingRadius = 4.5f;
    [TitleGroup("Companion")][SerializeField] private float targetFollowDist = 6f;

    [TitleGroup("Companion")][SerializeField] private float greetingsDist = 8f;

    [TitleGroup("Companion")][SerializeField] private float warpDistToPlayer = 15f;



    [TitleGroup("Companion")][SerializeField] public Player player;
    [TitleGroup("Companion")][SerializeField] public WorldEnviromentObject resourceTarget;

    [TitleGroup("Companion")][SerializeField] public LayerMask searcingLayer;

    [TitleGroup("Companion")][SerializeField] public float holdTime = 30f;

    [TitleGroup("Companion")][SerializeField] private bool hired = false;
    [TitleGroup("Companion")][SerializeField] private CompanionSpawnBeacon beacon;
    [TitleGroup("Companion")][SerializeField] private Image timerImage;
    [TitleGroup("Companion")][SerializeField] private SkinnedMeshRenderer skin;

    [TitleGroup("Companion")][SerializeField] private Gradient gradient;

    public bool GetHired => hired;

    [TitleGroup("Companion_Action")][HideInInspector] public List<Collider> triggerdCollider = new List<Collider>();

    [TitleGroup("Companion_Action")][field: SerializeField] public CompanionEquipment currentSword { get; private set; }
    [TitleGroup("Companion_Action")][field: SerializeField] public CompanionEquipment currentAxe { get; private set; }
    [TitleGroup("Companion_Action")][field: SerializeField] public CompanionEquipment currentPickaxe { get; private set; }
    [TitleGroup("Companion_Action")][SerializeField] private CompanionEquipment currentWeapon;

    [TitleGroup("Companion_Action")][field: EnumToggleButtons][field: SerializeField] public COMPANION_ACTION_STATE companionActionCurrentState;

    [TitleGroup("Companion_Permanent")][field: SerializeField] public float resurrectionTime = 5;


    private TaskUtil.DelayTaskMethod task = null;

    private float leftTime;
    private NavMeshPath path;
    private bool greetings = false;
    private TaskUtil.WhileTaskMethod triggerTask;


    public void Init(CompanionSpawnBeacon beacon, SkinnedMeshRenderer skinMesh, bool isTrial, COMPANION_TYPE type = COMPANION_TYPE.NORMAL)
    {
        this.beacon = beacon;
        skin.sharedMesh = skinMesh.sharedMesh;

        GetComponentInChildren<UIFloatingCompanion>().InitBtn(isTrial);

        companionType = type;
    }

    public void InitPermanentCompanion(SkinnedMeshRenderer skin)
    {
        hired = true;
        player = PlayerManager.Instance.player;
        companionType = COMPANION_TYPE.PERMANENT;
        this.skin.sharedMesh = skin.sharedMesh;

        animator.SetTrigger("Hired");
    }

    protected override void Start()
    {
        base.Start();
        var particle = ObjectPool.Instance.GetPool("Particle/FX_ShardRock_Dust_End_01");
        particle.transform.position = transform.position;
        particle.GetComponentInChildren<ParticleSystem>().Play();
        this.TaskDelay(2.5f, () => ObjectPool.Instance.AddPool(particle), true);

        leftTime = holdTime;
        path = new NavMeshPath();

        GetComponentInChildren<HealthBar>(true).level.transform.parent.gameObject.SetActive(false);

        if (triggerTask != null)
            triggerTask.Kill();
        triggerTask = this.TaskWhile(0.5f, 0f, DistanceTrigger);
    }

    private void Update()
    {
        if (currentMoveSpeed != agent.velocity.sqrMagnitude)
            OnChangeAgentMoveSpeed();

        if (hired)
        {
            timerImage.fillAmount = leftTime / holdTime;
            leftTime -= Time.deltaTime;

            timerImage.color = gradient.Evaluate(timerImage.fillAmount);
        }
    }

    protected override void AI_Action()
    {
        if (isDie)
            return;

        switch (currentSTATE)
        {
            case AI_STATE.IDLE:
                agent.speed = 0;

                break;

            case AI_STATE.FOLLOW:
                if (currentTarget != null)
                {
                    SetDestination(currentTarget.transform);
                }
                else if (resourceTarget != null)
                {
                    SetDestination(resourceTarget.transform);
                }
                else
                {
                    SetDestination(player.transform);
                }

                agent.speed = stat.MoveSpeed;

                if (Vector3.Distance(transform.position, player.transform.position) > warpDistToPlayer)
                    agent.Warp(Util.GetRandomPositionAroundTarget(player.transform, 2, 3));

                break;

            case AI_STATE.ATTACk:
                if (currentTarget != null)
                {
                    SetDestination(currentTarget.transform);

                    ActionStateMachine(COMPANION_ACTION_STATE.ATTACK);
                }
                else if (resourceTarget != null)
                {
                    SetDestination(resourceTarget.transform);

                    if (resourceTarget.GetBreakEquipmentType() == EquipmentType.PICKAXE)
                        ActionStateMachine(COMPANION_ACTION_STATE.MINING);
                    else if (resourceTarget.GetBreakEquipmentType() == EquipmentType.AXE)
                        ActionStateMachine(COMPANION_ACTION_STATE.FELLING);
                }

                agent.speed = stat.MoveSpeed / 4f;

                if (!isAttacking)
                {
                    animator.SetTrigger("Swing");
                }
                break;

            case AI_STATE.COLLECT:
                SetDestination(resourceTarget.transform);
                agent.speed = stat.MoveSpeed / 4f;
                break;

            case AI_STATE.RETURN:
                currentTarget = null;
                resourceTarget = null;
                SetDestination(player.transform);
                agent.speed = stat.MoveSpeed * 1.5f;
                Heal(1);

                agent.CalculatePath(agent.destination, path);
                if (path.status != NavMeshPathStatus.PathComplete)
                    agent.Warp(player.transform.position);
                break;

            case AI_STATE.SLEEP:
                agent.speed = 0;
                currentTarget = null;
                animator.SetBool("Sleep", true);
                Heal(1);
                break;

            default:
                Debug.LogError("정의된 ai 패턴이 아닙니다.");
                break;

        }

        if (isAttacking)
            SetDestination(transform);
    }

    protected override void StateMachine()
    {
        if (!hired)
        {
            if (beacon != null)
                SetDestination(beacon.transform);
            return;
        }

        if (currentSTATE == AI_STATE.RETURN)
        {
            currentTarget = null;
            resourceTarget = null;
            //print(Vector3.Distance(transform.position, NPCBeacon.transform.position));
            if (Vector3.Distance(transform.position, player.transform.position) < 2f)
                currentSTATE = AI_STATE.IDLE;
        }
        else if (currentTarget != null || resourceTarget != null)
        {
            var target = currentTarget != null ? currentTarget.transform : resourceTarget.transform;

            if (Vector3.Distance(transform.position, player.transform.position) > returnDist)
            {
                currentSTATE = AI_STATE.RETURN;
            }
            // else if (Vector3.Distance(transform.position, target.position) > playerFollowDist)
            // {
            //     currentSTATE = AI_STATE.RETURN;
            // }
            else if (Vector3.Distance(transform.position, target.position) < collectResourceDist)
            {
                currentSTATE = AI_STATE.ATTACk;
            }
            else
            {
                currentSTATE = AI_STATE.FOLLOW;
            }
        }
        else if (currentSTATE == AI_STATE.IDLE && stat.enableSleep)
        {
            currentSTATE = AI_STATE.SLEEP;
        }
        else if (stat.enablePatrol)
        {
            currentSTATE = AI_STATE.PATROL;
        }
        else if (Vector3.Distance(transform.position, player.transform.position) < 2f)
        {
            currentSTATE = AI_STATE.IDLE;
        }
        else
        {
            currentSTATE = AI_STATE.FOLLOW;
        }
        // else
        // {
        //     currentSTATE = AI_STATE.IDLE;
        // }

        AI_Action();
    }

    protected override void StartSearching()
    {
        if (!hired)
            return;

        if (currentTarget != null)
            if (currentTarget.isDie)
                currentTarget = null;

        if (resourceTarget != null)
            if (resourceTarget.IsBreak())
                resourceTarget = null;


        if (stat.ai_Type == AI_TYPE.COMPANION)
        {
            var find = Physics.OverlapSphere(transform.position, searchingRadius, searcingLayer);

            // if (agent.path != null)
            // {
            //     float dist = 0f;
            //     for (int i = 1; i < agent.path.corners.Length; i++)
            //         dist += Vector3.Distance(agent.path.corners[i], agent.path.corners[i - 1]);
            //     Debug.Log(dist);
            // }

            var resource_result = find.Where(n => n.TryGetComponent<WorldEnviromentObject>(out WorldEnviromentObject obj) && CheckIsCollectable(obj) && CheckOnDist(n)).ToList()
            .OrderBy(n =>
            {
                return CalculateDist(n.transform.position);
            }).ToList()
            .Where(n => n.GetComponent<WorldEnviromentObject>().GetCanBreak && !n.GetComponent<WorldEnviromentObject>().IsBreak()).ToList();

            var enemy_result = find.Where(n => n.TryGetComponent<Enemy>(out Enemy obj) && CheckOnDist(n)).ToList()
            .OrderBy(n =>
            {
                return CalculateDist(n.transform.position);
            }).ToList()
            .Where(n => !n.GetComponent<Enemy>().isDie).ToList();

            if (enemy_result.Count > 0)
            {
                if (currentTarget == null)
                {
                    agent.destination = enemy_result[0].GetComponent<Enemy>().transform.position;
                }

                currentTarget = enemy_result[0].GetComponent<Enemy>();
            }
            else if (resource_result.Count > 0)
            {
                if (resourceTarget == null)
                {
                    agent.destination = resource_result[0].GetComponent<WorldEnviromentObject>().transform.position;
                }

                resourceTarget = resource_result[0].GetComponent<WorldEnviromentObject>();
            }

        }

        StateMachine();
    }

    Transform FindClosestTarget(Collider[] find, COMPANION_SEARCH_TYPE type)
    {
        Transform result = null;
        switch (type)
        {
            case COMPANION_SEARCH_TYPE.RESOURCE:

                var resource_result = find.Where(n => n.TryGetComponent<WorldEnviromentObject>(out WorldEnviromentObject obj) && CheckIsCollectable(obj) && CheckOnDist(n)).ToList()
                            .OrderBy(n =>
                            {
                                return CalculateDist(n.transform.position);
                            }).ToList()
                            .Where(n => n.GetComponent<WorldEnviromentObject>().GetCanBreak && !n.GetComponent<WorldEnviromentObject>().IsBreak()).ToList();

                result = resource_result[0].transform;
                break;

            case COMPANION_SEARCH_TYPE.ENEMY:

                var enemy_result = find.Where(n => n.TryGetComponent<Enemy>(out Enemy obj) && CheckOnDist(n)).ToList()
                            .OrderBy(n =>
                            {
                                return CalculateDist(n.transform.position);
                            }).ToList()
                            .Where(n => !n.GetComponent<Enemy>().isDie).ToList();

                result = enemy_result[0].transform;

                break;
        }

        return result;
    }



    private bool CheckIsCollectable(WorldEnviromentObject obj)
    {
        if (obj == null)
            return false;
        else if (obj.GetPhaseData() == null)
            return false;

        return obj.GetPhaseData().EquipmentRequireTier <= SaveManager.Instance.PlayerController.GetCurrentEquipmentData(obj.GetBreakEquipmentType()).Tier;
    }

    private bool CheckOnDist(Collider n)
    {
        if (CalculateDist(n.transform.position) > targetFollowDist)
            return false;
        else
            return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, searchingRadius);
    }
    void OnHire()
    {
        hired = true;
        player = PlayerManager.Instance.player;

        this.TaskDelay(holdTime, EndHire);

        timerImage.transform.parent.gameObject.SetActive(true);
        AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.COMPANION, "Hire Companion - " + beacon.GetGuid());

        animator.SetTrigger("Hired");
    }

    public void OnClickTrialBtn()
    {
        SaveManager.Instance.PlayerController.UseCompanionTrial();
        OnHire();
    }

    public void OnClickRvBtn()
    {
        if (SaveManager.Instance.IAPController.GetRVToken() == 0)
        {
            MondayOFF.AdsManager.ShowRewarded(() =>
            {
                OnHire();
                AnalyticsManager.Instance.RewardVideoEvent("Hire Companion - " + beacon.GetGuid());
            });
        }
        else
        {
            OnHire();
            SaveManager.Instance.IAPController.AddRVToken(-1);
            AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.TOKEN, "Hire Companion - " + beacon.GetGuid());
        }
    }

    public void EndHire()
    {
        AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.COMPANION, "End Hire - TimeOut");

        var particle = ObjectPool.Instance.GetPool("Particle/FX_ShardRock_Dust_End_01");
        particle.transform.position = transform.position;
        particle.GetComponentInChildren<ParticleSystem>().Play();
        this.TaskDelay(2.5f, () => ObjectPool.Instance.AddPool(particle), true);

        Destroy(gameObject);
    }

    public override void SetDestination(Transform target)
    {
        if (Vector3Round(agent.destination) != Vector3Round(target.transform.position))
            agent.destination = target.position;


        // if (agent.destination != pos)
        // {
        //     print("Change Target /" + agent.destination + " / " + pos);
        //     agent.destination = pos;
        // }
        // else
        //     print("Change Target2 /" + agent.destination + " / " + pos);
    }

    public Vector3 Vector3Round(Vector3 origin)
    {
        return new Vector3(
    Mathf.Round(origin.x * 10f) / 10f,
    Mathf.Round(origin.y * 10f) / 10f,
    Mathf.Round(origin.z * 10f) / 10f
);
    }


    #region Action

    public void Equip()
    {
        switch (companionActionCurrentState)
        {
            case COMPANION_ACTION_STATE.ATTACK:
                currentSword.ChangeEquipment(SaveManager.Instance.PlayerController.GetCurrentEquipmentData(EquipmentType.SWORD));
                currentSword.gameObject.SetActive(true);
                animator.SetLayerWeight(1, 1);
                currentWeapon = currentSword;
                break;

            case COMPANION_ACTION_STATE.FELLING:
                currentAxe.ChangeEquipment(SaveManager.Instance.PlayerController.GetCurrentEquipmentData(EquipmentType.AXE));
                currentAxe.gameObject.SetActive(true);
                animator.SetLayerWeight(1, 1);
                currentWeapon = currentAxe;
                break;

            case COMPANION_ACTION_STATE.MINING:
                currentPickaxe.ChangeEquipment(SaveManager.Instance.PlayerController.GetCurrentEquipmentData(EquipmentType.PICKAXE));
                currentPickaxe.gameObject.SetActive(true);
                animator.SetLayerWeight(1, 1);
                currentWeapon = currentPickaxe;
                break;

            default:
                Debug.LogError("해당되는 무기 타입이 없습니다.");
                break;
        }
    }

    public void UnEquip()
    {
        // switch (companionActionCurrentState)
        // {
        //     case COMPANION_ACTION_STATE.ATTACK:
        //         currentSword.gameObject.SetActive(false);
        //         animator.SetLayerWeight(1, 0);
        //         break;
        //     case COMPANION_ACTION_STATE.FELLING:
        //         currentAxe.gameObject.SetActive(false);
        //         animator.SetLayerWeight(1, 0);
        //         break;
        //     case COMPANION_ACTION_STATE.MINING:
        //         currentPickaxe.gameObject.SetActive(false);
        //         animator.SetLayerWeight(1, 0);

        //         break;

        //     default:

        //         Debug.LogError("해당되는 무기 타입이 없습니다.");
        //         break;
        // }

        currentSword.gameObject.SetActive(false);
        currentAxe.gameObject.SetActive(false);
        currentPickaxe.gameObject.SetActive(false);
        animator.SetLayerWeight(1, 0);

        companionActionCurrentState = COMPANION_ACTION_STATE.DEFAULT;
    }

    public void ActionEnd()
    {
        isAttacking = false;

        triggerdCollider.Clear();
        UnEquip();

        if (resourceTarget != null)
            if (resourceTarget.IsBreak())
                resourceTarget = null;

    }

    public override void AttackTriggerOn()
    {
        currentWeapon.ColliderOn();
        currentWeapon.WeaponTrailOn();
    }

    public override void AttackTriggerOff()
    {
        currentWeapon.ColliderOff();
        currentWeapon.WeaponTrailOff();
    }

    public void ActionStateMachine(COMPANION_ACTION_STATE state)
    {
        companionActionCurrentState = state;
        Equip();
    }

    #endregion

    private void OnDestroy()
    {
        if (beacon != null)
            beacon.RespwanCountStart();

        var particle = ObjectPool.Instance.GetPool("Particle/FX_ShardRock_Dust_End_01");
        particle.transform.position = transform.position;
        particle.GetComponentInChildren<ParticleSystem>().Play();
        PlayerManager.Instance.TaskDelay(2.5f, () => ObjectPool.Instance.AddPool(particle), true);

        if (companionType == COMPANION_TYPE.PERMANENT)
        {
            PlayerManager.Instance.player.SetSPC(false);
            PlayerManager.Instance.TaskDelay(resurrectionTime, () => player.SpawnPermanentCompanion(), true);
        }

    }

    public void Hello()
    {
        animator.SetTrigger("Hello");
        greetings = true;

        this.TaskDelay(5f, () => greetings = false);
    }

    void DistanceTrigger()
    {
        var player = PlayerManager.Instance.player;
        if (player == null || greetings == true || hired)
            return;

        if (Vector3.Distance(player.transform.position, transform.position) <= greetingsDist)
        {
            // if (triggerTask != null)
            //     triggerTask.Kill();
            Hello();
        }
    }
}
