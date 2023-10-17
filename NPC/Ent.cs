using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Ent : Enemy
{
    public enum AttackPattern
    {
        None = 0,
        LowAttack = 1,
        GroundSlam = 2,
        ShardRock = 3,
        FootStamp = 4
    }

    [Space]

    public AttackPattern currentAttackPattern;

    [Space]
    [Header("Attack Pattern")]

    [TabGroup("Ground Slam")][SerializeField] private Transform groundSlam_explodePoint;
    [TabGroup("Ground Slam")][SerializeField] private float groundSlam_explodeRadius;
    [Range(0, 2)]
    [TabGroup("Ground Slam")][SerializeField] private float groundSlam_explodeDamagePercent;
    [TabGroup("Ground Slam")][SerializeField] private Transform groundSlam_Circle;
    [TabGroup("Ground Slam")][SerializeField] private string groundSlam_ParticlePath;


    [TabGroup("Shard Vine")][SerializeField] private float shardRockRadius;
    [Range(0, 2)]
    [TabGroup("Shard Vine")][SerializeField] private float shardRockRadius_DamagePercent;
    [TabGroup("Shard Vine")][SerializeField][MinMaxSlider(0, 20, true)] private Vector2 shardRockRange;
    [TabGroup("Shard Vine")][SerializeField] private float shardRockDelay = 1.5f;
    [TabGroup("Shard Vine")][SerializeField] private float shardRockCoolTime;
    [TabGroup("Shard Vine")][SerializeField] private bool shardRockReady = true;
    [TabGroup("Shard Vine")][SerializeField] private string shardRockParticlePath;

    private Vector3 shardRockPoint;

    [TabGroup("Foot Stamp")][SerializeField] private Transform footStamp_explodePoint;
    [TabGroup("Foot Stamp")][SerializeField] private float footStamp_explodeRadius;
    [Range(0, 2)]
    [TabGroup("Foot Stamp")][SerializeField] private float footStamp_explodeDamagePercent;
    [TabGroup("Foot Stamp")][SerializeField] private float footStampDelay = 1;
    [TabGroup("Foot Stamp")][SerializeField] private string footStampParticlePath;


    [TabGroup("Triple Strate Vine")][SerializeField] private Transform TSV_explodePoint;
    [TabGroup("Triple Strate Vine")][SerializeField] private float TSV_explodeRadius;
    [Range(0, 2)]
    [TabGroup("Triple Strate Vine")][SerializeField] private float TSV_explodeDamagePercent;
    [TabGroup("Triple Strate Vine")][SerializeField] private float TSV_Delay = 1;


    protected override void StateMachine()
    {
        if (currentSTATE == AI_STATE.RETURN)
        {
            if (Vector3.Distance(transform.position, NPCBeacon.transform.position) < 1f)
                currentSTATE = AI_STATE.IDLE;
        }
        else if (currentTarget != null)
        {
            if (Vector3.Distance(transform.position, NPCBeacon.transform.position) > returnDistance_betweenBeacon)
            {
                currentSTATE = AI_STATE.RETURN;
            }
            else if (GetTargetDistance() > returnDistance_betweenTarget)
            {
                currentSTATE = AI_STATE.RETURN;
            }
            else if (GetTargetDistance() < stat.AttackDist && CheckTargetInSight(currentTarget.transform))
            {

                print(CheckTargetInSight(currentTarget.transform));
                currentSTATE = AI_STATE.ATTACk;


                switch (Random.Range(0, 3))
                {
                    case 0:
                        currentAttackPattern = AttackPattern.LowAttack;
                        break;

                    case 1:
                        currentAttackPattern = AttackPattern.GroundSlam;
                        break;

                    case 2:
                        currentAttackPattern = AttackPattern.FootStamp;
                        break;
                }

            }
            else if (shardRockReady && GetTargetDistance() > shardRockRange.x && GetTargetDistance() < shardRockRange.y)
            {
                currentSTATE = AI_STATE.ATTACk;
                currentAttackPattern = AttackPattern.ShardRock;
            }
            else
            {
                currentSTATE = AI_STATE.FOLLOW;
            }
        }
        else
        {
            currentSTATE = AI_STATE.IDLE;
        }

        AI_Action();
    }

    protected override void AI_Action()
    {
        if (isDie)
            return;

        switch (currentSTATE)
        {
            case AI_STATE.IDLE:
                agent.speed = 0;
                animator.SetBool("Move", false);
                break;

            case AI_STATE.FOLLOW:
                agent.destination = currentTarget.transform.position;
                agent.speed = stat.MoveSpeed;
                animator.SetBool("Move", true);
                break;

            case AI_STATE.ATTACk:
                agent.destination = currentTarget.transform.position;
                agent.speed = stat.MoveSpeed / 4f;

                if (!isAttacking)
                    AttackPatternStateMachine();
                break;

            case AI_STATE.RETURN:
                agent.destination = NPCBeacon.transform.position;
                agent.speed = stat.MoveSpeed * 1.5f;
                animator.SetBool("Move", true);
                currentTarget = null;
                Heal(1);
                break;

            default:
                Debug.LogError("정의된 ai 패턴이 아닙니다.");
                break;

        }

        if (isAttacking)
            agent.destination = transform.position;
    }

    public void AttackPatternStateMachine()
    {
        if (isAttacking)
            return;

        switch (currentAttackPattern)
        {
            case AttackPattern.LowAttack:
                isAttacking = true;

                if (Random.Range(0, 2) == 0)
                    animator.SetTrigger("LowAttack1");
                else
                    animator.SetTrigger("LowAttack2");

                break;

            case AttackPattern.GroundSlam:
                isAttacking = true;
                Draw2DMesh.DrawCircleCharge(1.5f, groundSlam_Circle.position, groundSlam_explodeRadius, Color.red);
                animator.SetTrigger("GroundSlam");

                break;

            case AttackPattern.ShardRock:
                isAttacking = true;
                animator.SetTrigger("ShardRock");
                shardRockReady = false;
                break;

            case AttackPattern.FootStamp:
                isAttacking = true;
                animator.SetTrigger("FootStamp");
                break;

            default:

                Debug.LogError("해당되는 공격 패턴이 없습니다.");

                break;

        }

        currentAttackPattern = AttackPattern.None;
    }

    public void GroundSlam()
    {
        var particle = ObjectPool.Instance.GetPool(groundSlam_ParticlePath);

        particle.transform.position = groundSlam_explodePoint.position;

        Util.RadiusAttack(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Companion"), groundSlam_explodePoint.position, groundSlam_explodeRadius, Mathf.RoundToInt(stat.Damage * groundSlam_explodeDamagePercent));

        // var colliders = Physics.OverlapSphere(groundSlam_explodePoint.position, groundSlam_explodeRadius, 1 << LayerMask.NameToLayer("Player"));

        // foreach (Collider collider in colliders)
        // {
        //     if (collider.TryGetComponent<Player>(out Player player))
        //     {
        //         player.TakeDamage(Mathf.RoundToInt(stat.Damage * groundSlam_explodeDamagePercent));
        //     }
        // }

        SoundManager.Instance.PlaySFX("ShortExplosion", 0.3f, Random.Range(0.8f, 1.2f));

        this.TaskDelay(3.5f, () => ObjectPool.Instance.AddPool(particle));
    }

    public void ShardRock()
    {
        shardRockPoint = currentTarget.transform.position;
        Draw2DMesh.DrawCircleCharge(shardRockDelay, shardRockPoint, shardRockRadius, Color.red);
        this.TaskDelay(shardRockDelay, () => Effect());

        void Effect()
        {
            var particle = ObjectPool.Instance.GetPool(shardRockParticlePath);
            particle.transform.position = shardRockPoint;
            Util.RadiusAttack(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Companion"), shardRockPoint, shardRockRadius, Mathf.RoundToInt(stat.Damage * shardRockRadius_DamagePercent));
            SoundManager.Instance.PlaySFX("ShortExplosion", 0.3f, Random.Range(0.8f, 1.2f));
            this.TaskDelay(6f, () => ObjectPool.Instance.AddPool(particle));
            this.TaskDelay(shardRockCoolTime, () => shardRockReady = true);
        }
    }

    public void FootStamp()
    {
        Draw2DMesh.DrawCircleCharge(footStampDelay, footStamp_explodePoint.position, footStamp_explodeRadius, Color.red);
        this.TaskDelay(footStampDelay, () => Effect());

        void Effect()
        {
            var particle = ObjectPool.Instance.GetPool(footStampParticlePath);
            particle.transform.position = footStamp_explodePoint.position;
            Util.RadiusAttack(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Companion"), footStamp_explodePoint.position, footStamp_explodeRadius, Mathf.RoundToInt(stat.Damage * footStamp_explodeDamagePercent));
            SoundManager.Instance.PlaySFX("ShortExplosion", 0.3f, Random.Range(0.8f, 1.2f));
            this.TaskDelay(3.5f, () => ObjectPool.Instance.AddPool(particle));
        }
    }

    public override void CustomAnimationEvent(string eventString)
    {
        SendMessage(eventString);
    }

    public override void AttackTriggerOn()
    {
        //base.AttackTriggerOn();

        SoundManager.Instance.PlaySFX("Heavy_Swing", 0.5f, Random.Range(0.8f, 1.2f));
    }

    public void R_ColliderOn()
    {
        attackTriggerColliders[0].enabled = true;
        AttackTriggerOn();
    }

    public void L_ColliderOn()
    {
        attackTriggerColliders[1].enabled = true;
        AttackTriggerOn();
    }

}
