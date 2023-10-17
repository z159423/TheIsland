using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public abstract class NPCObject : SerializedMonoBehaviour
{
    [SerializeField] public abstract int maxHP { get; protected set; }
    [SerializeField] public abstract int currentHP { get; protected set; }
    [SerializeField] public abstract bool isDie { get; protected set; }

    [SerializeField] public abstract HealthBar HpBar { get; protected set; }
    [SerializeField] protected abstract Animator animator { get; set; }
    [SerializeField] public abstract Vector3 HpBarPosition { get; protected set; }

    [SerializeField] public abstract bool ragdollOnDie { get; protected set; }
    [SerializeField] public abstract Collider[] ragdollColliders { get; protected set; }


    public abstract Coroutine hpBarHideCoroutine { get; protected set; }

    public abstract void InitHpBar();

    public abstract void TakeDamage(int damage, NPCObject from = null, string cause = null);

    public abstract void Heal(int heal);

    public abstract void OnChangeHP();

    public abstract void Die(Transform from = null);

    public abstract void ragdollEnable(Transform from = null);


}
