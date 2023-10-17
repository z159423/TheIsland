using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackDetector : ObjectDetector
{
    private Enemy enemy;

    private void OnEnable()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    public override void ClearObjectList()
    {
        triggeredObjects.Clear();

        //detector.enabled = true;
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (!enemy.isAttacking)
            return;

        if (other.TryGetComponent<Player>(out Player player))
        {
            if (!triggeredObjects.Contains(other))
            {
                triggeredObjects.Add(other);
            }

            player.TakeDamage(enemy.stat.Damage, enemy, enemy.stat.Name);
        }

        if (other.TryGetComponent<Companion>(out Companion companion))
        {
            if (!triggeredObjects.Contains(other))
            {
                triggeredObjects.Add(other);
            }

            companion.TakeDamage(enemy.stat.Damage, enemy, enemy.stat.Name);
        }

        if (triggeredObjects.Count > 0)
            detector.enabled = false;
    }
}
