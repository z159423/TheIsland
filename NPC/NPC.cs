using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.Events;

public enum AI_STATE
{
    NONE = -1,
    IDLE = 0,
    PATROL = 1,
    FOLLOW = 2,
    ATTACk = 3,
    RETURN = 4,
    SLEEP = 5,
    COLLECT = 6,
    RUNAWAY = 7
}

public enum AI_TYPE
{
    //중립
    NEUTRAL = 0,
    //선공
    AGGRESSIVE = 1,
    //아군
    FRIENDLY = 2,
    COMPANION = 3,
    PREY = 4
}

public abstract class NPC : NPCObject, IAttackTriggerController
{
    [field: SerializeField] public readonly int ID;

    [field: FoldoutGroup("참조")][field: SerializeField] public NPCScriptableObject stat { get; private set; }
    [field: FoldoutGroup("참조")][field: SerializeField] protected NavMeshAgent agent { get; private set; }
    [field: FoldoutGroup("참조")][field: SerializeField] protected override Animator animator { get; set; }
    [field: FoldoutGroup("참조")][field: SerializeField] public NPCBeacon NPCBeacon { get; set; }
    [field: FoldoutGroup("참조")][field: SerializeField] public override HealthBar HpBar { get; protected set; }

    [field: FoldoutGroup("설정")][field: SerializeField] public override Vector3 HpBarPosition { get; protected set; } = new Vector3(0, 1.3f, 0);
    [field: FoldoutGroup("설정")][field: SerializeField] public bool HpBarScaleFixed { get; protected set; } = false;

    [field: FoldoutGroup("설정")][field: SerializeField] public Vector3 HpBarScale { get; protected set; } = new Vector3(1f, 1f, 1f);

    [field: FoldoutGroup("설정")][field: SerializeField] public bool footStepParticle { get; protected set; } = false;
    [field: FoldoutGroup("설정")][field: ShowIf("footStepParticle")][field: SerializeField] public ParticleSystem[] footStepParticles { get; protected set; }





    [field: BoxGroup("Npc Stat")][field: SerializeField] public override bool isDie { get; protected set; } = false;
    [field: BoxGroup("Npc Stat")][field: SerializeField] public override int maxHP { get; protected set; }
    [field: BoxGroup("Npc Stat")][field: ProgressBar(0, "maxHP", ColorGetter = "GetHealthBarColor")][field: SerializeField] public override int currentHP { get; protected set; }


    [field: Header("현재 타겟")]
    [field: BoxGroup("STATE")][field: SerializeField][ReadOnly] public NPCObject currentTarget { get; protected set; } = null;
    [field: BoxGroup("STATE")][field: EnumToggleButtons][field: SerializeField] public AI_STATE currentSTATE { get; protected set; }

    [field: Space]

    [field: Header("복귀 거리")]
    [field: SerializeField] protected float returnDistance_betweenBeacon { get; private set; } = 10;
    [field: SerializeField] protected float returnDistance_betweenTarget { get; private set; } = 5;

    [field: Space]

    [field: BoxGroup("STATE")][field: SerializeField] public bool isAttacking { get; protected set; } = false;

    [field: Header("공격 충돌감지 클라이더")]
    [field: SerializeField] protected Collider[] attackTriggerColliders { get; private set; }

    [field: Space]

    [field: SerializeField] protected Dictionary<string, TrailRenderer> weaponTrails = new Dictionary<string, TrailRenderer>();

    [field: Space]

    [field: BoxGroup("ragdoll")][field: SerializeField] public override bool ragdollOnDie { get; protected set; } = false;
    [field: BoxGroup("ragdoll")][field: ShowIf("ragdollOnDie")][field: SerializeField] public override Collider[] ragdollColliders { get; protected set; }
    [BoxGroup("ragdoll")][SerializeField] private float ragdollDeleteTime = 1.65f;

    [BoxGroup("ragdoll")]
    [Button]
    public void GetRagDollParts()
    {
        var joints = GetComponentsInChildren<CharacterJoint>();

        ragdollColliders = new Collider[joints.Length];

        for (int i = 0; i < joints.Length; i++)
        {
            ragdollColliders[i] = joints[i].GetComponent<Collider>();
            joints[i].GetComponent<Collider>().enabled = false;
            joints[i].GetComponent<Rigidbody>().useGravity = false;
            joints[i].GetComponent<Rigidbody>().isKinematic = false;

        }
    }

    [field: Space]

    [BoxGroup("View Angle")][SerializeField] bool DebugMode = false;
    [BoxGroup("View Angle")][SerializeField][Range(0f, 360f)] float ViewAngle = 45f;
    [BoxGroup("View Angle")][SerializeField] float ViewRadius = 3f;
    // [SerializeField] LayerMask TargetMask;
    // [SerializeField] LayerMask ObstacleMask;

    [field: Space]

    [field: BoxGroup("Die")][field: SerializeField] protected bool dieAnimation = false;

    protected TaskUtil.WhileTaskMethod searchingTargetTask = null;
    protected TaskUtil.WhileTaskMethod stateMachineTask = null;
    protected TaskUtil.WhileTaskMethod patrolPositionTask = null;
    protected AI_STATE recentState = AI_STATE.NONE;
    protected FaceAnimation faceAnimation;

    [HideInInspector][SerializeField] protected EnemyAttackDetector objectDetector;

    private Vector3 patrolPoint;
    protected float currentMoveSpeed;
    private ParticleSystem sleepParticle;

    public UnityEvent onDeath;

    Sequence damageSeq;
    public override Coroutine hpBarHideCoroutine { get; protected set; }

    protected virtual void Start()
    {
        InitHpBar();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (agent == null)
            agent = GetComponentInChildren<NavMeshAgent>();

        searchingTargetTask = this.TaskWhile(Random.Range(0.2f, 0.3f), 0, StartSearching);

        // stateMachineTask = this.TaskWhile(Random.Range(0.2f, 0.3f), 0, StateMachine);

        patrolPositionTask = this.TaskWhile(Random.Range(stat.patrolTime.x, stat.patrolTime.y), 0, SetPatrolPosition);

        if (objectDetector == null)
            objectDetector = GetComponentInChildren<EnemyAttackDetector>();

        if (stat.faceAnimation)
        {
            faceAnimation = gameObject.AddComponent<FaceAnimation>();
            faceAnimation.Init(GetComponentInChildren<SkinnedMeshRenderer>(), stat);
        }

        if (stat.enableSleep)
        {
            sleepParticle = ObjectPool.Instance.GetPool("Particle/Sleep Particle", transform).GetComponent<ParticleSystem>();
            sleepParticle.transform.localPosition = stat.sleepParticlePosition;
        }

        foreach (var coll in ragdollColliders)
        {
            coll.GetComponent<Rigidbody>().useGravity = false;
            coll.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void LateUpdate()
    {
        OnChangeState();
    }

    private void Update()
    {
        if (currentMoveSpeed != agent.velocity.sqrMagnitude)
            OnChangeAgentMoveSpeed();
    }

    public void InitStat(NPCScriptableObject NPC_DATA, NPCBeacon beacon)
    {
        stat = NPC_DATA;

        maxHP = stat.MaxHp;
        currentHP = stat.MaxHp;

        this.NPCBeacon = beacon;
    }

    public override void InitHpBar()
    {
        var clone = ObjectPool.Instance.GetPool("UI/Hp bar");
        HpBar = clone.GetComponentInChildren<HealthBar>(true);
        HpBar.gameObject.SetActive(false);
        HpBar.SetMaxHealth(maxHP, stat.Level);
        HpBar.SetHealth(currentHP);
        clone.transform.SetParent(transform);
        clone.transform.localPosition = Vector3.zero;
        HpBar.GetComponentInParent<Billboard>().GetComponent<RectTransform>().anchoredPosition = HpBarPosition;
        // if (HpBarScaleFixed)
        // HpBar.transform.localScale = HpBar.transform.localScale * ;
        ObjectPool.Instance.AddClearReserve(clone);
    }

    protected virtual void AI_Action()
    {
        if (isDie)
            return;

        switch (currentSTATE)
        {
            case AI_STATE.IDLE:
                agent.speed = 0;
                break;

            case AI_STATE.PATROL:
                agent.speed = stat.patrolMoveSpeed;
                break;

            case AI_STATE.FOLLOW:
                SetDestination(currentTarget.transform);
                agent.speed = stat.MoveSpeed;
                break;

            case AI_STATE.ATTACk:
                SetDestination(currentTarget.transform);
                agent.speed = stat.MoveSpeed / 4f;
                if (!isAttacking)
                    animator.SetTrigger("Attack");
                break;

            case AI_STATE.RETURN:
                SetDestination(NPCBeacon.transform);
                agent.speed = stat.MoveSpeed * 1.5f;
                currentTarget = null;
                Heal(1);
                break;

            case AI_STATE.SLEEP:
                agent.speed = 0;
                currentTarget = null;
                animator.SetBool("Sleep", true);
                Heal(1);
                break;

            case AI_STATE.RUNAWAY:
                agent.destination = Util.GetOppositeDirection(transform.position, currentTarget.transform.position, 5, 10);
                agent.speed = stat.MoveSpeed * 1.5f;
                break;

            default:
                Debug.LogError("정의된 ai 패턴이 아닙니다.");
                break;

        }

        if (isAttacking)
            SetDestination(transform.transform);
    }

    public virtual void SetDestination(Transform target)
    {
        if (currentTarget != null)
            agent.destination = target.position;

    }

    public override void TakeDamage(int damage, NPCObject from = null, string cause = null)
    {
        damage -= stat.Armor;

        // int levelDiff = 0;

        // if (from != null)
        // {
        //     if (from.TryGetComponent<Player>(out Player player))
        //     {
        //         levelDiff = stat.Level - SaveManager.Instance.PlayerController.GetPlayerLevel();
        //     }
        // }

        // damage -= levelDiff;

        //데미지 최솟값 1
        damage = Mathf.Clamp(damage, 1, int.MaxValue);

        currentHP -= damage;

        OnChangeHP();

        if (currentHP <= 0)
            Die(from?.transform);
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

        FloatingTextManager.Instance.GenearteFloatingText_Damage(damage, Color.yellow, transform.position, false);

        if (from != null)
        {
            if (currentTarget == null && stat.ai_Type != AI_TYPE.FRIENDLY)
            {
                currentTarget = from;
            }

            NPCBeacon?.npcGroups?.GroupTargeting(from);
        }

    }

    public override void Heal(int heal)
    {
        if (currentHP >= maxHP)
            return;

        currentHP += heal;

        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        OnChangeHP();
    }

    public override void OnChangeHP()
    {
        HpBar.SetMaxHealth(maxHP, stat.Level);
        HpBar.SetHealth(currentHP);

        HpBar.gameObject.SetActive(true);

        if (hpBarHideCoroutine != null)
            StopCoroutine(hpBarHideCoroutine);

        hpBarHideCoroutine = StartCoroutine(HideHpBar());

        IEnumerator HideHpBar()
        {
            yield return new WaitForSeconds(5f);

            HpBar.gameObject.SetActive(false);
        }
    }

    [Button]
    [BoxGroup("Die")]
    public override void Die(Transform from = null)
    {
        isDie = true;

        if (ragdollOnDie)
        {
            if (dieAnimation)
            {
                animator.SetTrigger("OnDie");
                agent.enabled = false;
            }
            else
                ragdollEnable(from);
        }
        else
        {
            Destroy(gameObject);
        }

        onDeath?.Invoke();

        HpBar.gameObject.SetActive(false);

        NPCBeacon?.Respawn();

        if (from != null)
        {
            if (from.TryGetComponent(out Player player) || from.TryGetComponent(out Companion companion))
            {
                AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.KILL, stat.Name);
                SaveManager.Instance.QuestController.AddKillCount(stat.ID);
            }
        }
    }

    protected virtual void AfterDieAnimation()
    {
        ragdollEnable();
    }

    protected virtual void StartSearching()
    {
        if (currentTarget != null)
        {
            if (currentTarget.isDie)
                currentTarget = null;
        }

        if (stat.ai_Type == AI_TYPE.AGGRESSIVE)
        {
            var find = Physics.OverlapSphere(transform.position, stat.SearchingRadius, 1 << LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("Companion"));
            var result = find.Where(n => n.TryGetComponent<Player>(out Player player)).ToList()
            .OrderBy(n => Vector3.Distance(transform.position, n.transform.position)).ToList()
            .Where(n => !n.GetComponent<Player>().isDie).ToList();

            if (result.Count > 0)
            {
                if (currentTarget == null)
                    SetDestination(result[0].transform);

                currentTarget = result[0].GetComponent<Player>();
            }
        }

        StateMachine();
    }

    protected abstract void StateMachine();

    public virtual void AttackTriggerOn()
    {
        foreach (Collider collider in attackTriggerColliders)
        {
            collider.enabled = true;
        }
    }

    public virtual void AttackTriggerOff()
    {
        foreach (Collider collider in attackTriggerColliders)
        {
            collider.enabled = false;
        }
    }

    public virtual void TrailEmit(string key)
    {
        weaponTrails[key].emitting = true;
    }

    public virtual void TrailStop(string key)
    {
        weaponTrails[key].emitting = false;
    }

    public override void ragdollEnable(Transform from = null)
    {
        foreach (Collider collider in ragdollColliders)
        {
            collider.enabled = true;
            collider.GetComponent<Rigidbody>().useGravity = true;
            collider.GetComponent<Rigidbody>().isKinematic = false;

        }

        animator.enabled = false;

        GetComponent<Collider>().enabled = false;

        if (from != null)
        {
            foreach (Collider collider in ragdollColliders)
            {
                collider.GetComponent<Rigidbody>().AddForce(((transform.position - from.position) + Vector3.up).normalized * 4, ForceMode.Impulse);
                collider.GetComponent<Rigidbody>().AddTorque(((transform.position - from.position) + Vector3.up).normalized * 60, ForceMode.Impulse);

            }
        }

        GetComponent<NavMeshAgent>().enabled = false;

        this.TaskDelay(ragdollDeleteTime, AfterNpcDie);
    }

    public virtual void AfterNpcDie()
    {
        var particle = ObjectPool.Instance.GetPool("Particle/GenericDeath");
        particle.transform.position = GetComponentInChildren<SkinnedMeshRenderer>().bounds.center;

        ItemManager.Instance.GenerateNewItemDrop(stat.itemDropTable, GetComponentInChildren<SkinnedMeshRenderer>().bounds.center.AddY(1f));
        Destroy(gameObject);

        this.TaskDelay(3f, () => ObjectPool.Instance.AddPool(particle));
    }

    public virtual bool CheckTargetInSight(Transform target)
    {
        float lookingAngle = transform.eulerAngles.y;  //캐릭터가 바라보는 방향의 각도

        Vector3 myPos = transform.position + Vector3.up * 0.5f;

        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
        Vector3 lookDir = AngleToDir(lookingAngle);

        Vector3 targetPos = target.position;
        Vector3 targetDir = (targetPos - myPos).normalized;
        float targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg;

        if (targetAngle <= ViewAngle * 0.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }

    public virtual void CustomAnimationEvent(string eventString)
    {

    }

    private void OnDrawGizmos()
    {
        if (DebugMode && currentTarget != null)
        {
            Vector3 myPos = transform.position + Vector3.up * 0.5f;

            float lookingAngle = transform.eulerAngles.y;  //캐릭터가 바라보는 방향의 각도

            Vector3 rightDir = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);
            Vector3 leftDir = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
            Vector3 lookDir = AngleToDir(lookingAngle);

            Vector3 targetPos = currentTarget.transform.position;
            Vector3 targetDir = (targetPos - myPos).normalized;

            Debug.DrawRay(myPos, leftDir * ViewRadius, Color.blue);
            Debug.DrawRay(myPos, lookDir * ViewRadius, Color.cyan);
            Gizmos.DrawWireSphere(myPos, ViewRadius);
            Debug.DrawRay(myPos, rightDir * ViewRadius, Color.blue);

            float targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg;

            if (targetAngle <= ViewAngle * 0.5f)
            {
                Debug.DrawLine(myPos, targetPos, Color.red);
                //print("1");
            }
        }
    }

    private Color GetHealthBarColor(float value)
    {
        return Color.Lerp(Color.red, Color.green, Mathf.Pow(value / 100f, 2));
    }

    public void GroupTargeting(NPCObject target)
    {
        if (currentTarget == null)
        {
            currentTarget = target;
        }
    }

    public void SetPatrolPosition()
    {
        if (currentSTATE != AI_STATE.PATROL)
            return;

        patrolPoint = GetRandomPoint(NPCBeacon.transform.position, stat.patrolRange);
        agent?.SetDestination(patrolPoint);

        if (patrolPositionTask != null)
            patrolPositionTask.SetIntervalTime(Random.Range(stat.patrolTime.x, stat.patrolTime.y));
    }

    public static Vector3 GetRandomPoint(Vector3 center, float maxDistance)
    {
        // Get Random Point inside Sphere which position is center, radius is maxDistance
        Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;

        NavMeshHit hit; // NavMesh Sampling Info Container

        // from randomPos find a nearest point on NavMesh surface in range of maxDistance
        NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas);

        return hit.position;
    }

    protected void OnChangeAgentMoveSpeed()
    {
        if (agent.velocity.sqrMagnitude > 0.2f && currentSTATE != AI_STATE.IDLE)
        {
            animator.SetBool("Move", true);
        }
        else
        {
            animator.SetBool("Move", false);
        }

        currentMoveSpeed = agent.speed;
    }

    protected float GetTargetDistance()
    {
        return Vector3.Distance(transform.position, currentTarget.transform.position);
    }

    public virtual void Attack()
    {
        isAttacking = true;
    }

    public virtual void AttackEnd()
    {
        isAttacking = false;
        objectDetector.ClearObjectList();
    }

    public virtual void OnChangeState()
    {
        if (currentSTATE == recentState)
            return;

        if (currentSTATE != AI_STATE.SLEEP)
        {
            // animator.SetBool("Sleep", false);
            if (stat.enableSleep) sleepParticle?.Stop();
        }
        else if (currentSTATE == AI_STATE.SLEEP)
        {
            if (stat.enableSleep) sleepParticle?.Play();
        }

        if (stat.faceAnimation) faceAnimation.ChangeFaceForAI_State(currentSTATE);

        recentState = currentSTATE;
    }

    public void footstep1()
    {
        if (footStepParticle)
            footStepParticles[0].Play();
    }

    public void footstep2()
    {
        if (footStepParticle)
            footStepParticles[1].Play();
    }

    public float CalculateDist(Vector3 target)
    {
        if (!agent.isOnNavMesh)
            return float.MaxValue;

        NavMeshHit meshHit;
        if (NavMesh.SamplePosition(target, out meshHit, agent.height * 2, NavMesh.AllAreas))
        {
            var path = new NavMeshPath();
            agent.CalculatePath(meshHit.position, path);
            float dist = 0f;

            for (int i = 1; i < path.corners.Length; i++)
            {
                dist += Vector3.Distance(path.corners[i], path.corners[i - 1]);

                Debug.DrawLine(path.corners[i], path.corners[i - 1], Color.blue, 1f);
            }
            return dist;
        }
        return float.MaxValue;
    }
}
