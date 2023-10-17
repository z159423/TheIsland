using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;



public class IronSpirit : Enemy
{

    protected override void StateMachine()
    {
        if (isDie)
        {
            faceAnimation.ChangeFaceCustom("Die");
            return;
        }

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
            else if (Vector3.Distance(transform.position, currentTarget.transform.position) > returnDistance_betweenTarget)
            {
                currentSTATE = AI_STATE.RETURN;
            }
            else if (Vector3.Distance(transform.position, currentTarget.transform.position) < stat.AttackDist && CheckTargetInSight(currentTarget.transform))
            {
                currentSTATE = AI_STATE.ATTACk;
            }
            else
            {
                currentSTATE = AI_STATE.FOLLOW;
            }
        }
        //else if (stat.enableSleep && DayCycleManager.Instance.GetCurrentDayType() == DayType.Day)
        //{
        //    currentSTATE = AI_STATE.SLEEP;
        //}
        //else if (stat.enablePatrol && DayCycleManager.Instance.GetCurrentDayType() == DayType.Night)
        //{
        //    currentSTATE = AI_STATE.PATROL;
        //}
        else
        {
            currentSTATE = AI_STATE.IDLE;
        }

        AI_Action();
    }

    protected override void StartSearching()
    {
        if (currentTarget != null)
        {
            if (currentTarget.isDie)
                currentTarget = null;
            else
                return;
        }

        if (stat.ai_Type == AI_TYPE.AGGRESSIVE)
        {
            var find = Physics.OverlapSphere(transform.position, stat.SearchingRadius, 1 << LayerMask.NameToLayer("Player"));
            var result = find.Where(n => n.TryGetComponent<Player>(out Player player)).ToList()
            .OrderBy(n => Vector3.Distance(transform.position, n.transform.position)).ToList()
            .Where(n => !n.GetComponent<Player>().isDie).ToList();

            if (result.Count > 0)
                currentTarget = result[0].GetComponent<Player>();
        }
    }

}
