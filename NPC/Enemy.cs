using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;



public class Enemy : NPC
{

    protected override void StateMachine()
    {
        if (stat.ai_Type == AI_TYPE.PREY && (currentSTATE == AI_STATE.PATROL || currentSTATE == AI_STATE.IDLE || currentSTATE == AI_STATE.FOLLOW) && currentTarget != null)
        {
            currentSTATE = AI_STATE.RUNAWAY;
        }
        else if (currentSTATE == AI_STATE.RETURN)
        {
            //print(Vector3.Distance(transform.position, NPCBeacon.transform.position));
            if (Vector3.Distance(transform.position, NPCBeacon.transform.position) < 2f)
                currentSTATE = AI_STATE.IDLE;
        }
        else if (currentTarget != null)
        {
            if (CalculateDist(NPCBeacon.transform.position) > returnDistance_betweenBeacon)
            {
                currentSTATE = AI_STATE.RETURN;
            }
            else if (CalculateDist(currentTarget.transform.position) > returnDistance_betweenTarget)
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
        else if (currentSTATE == AI_STATE.IDLE && stat.enableSleep)
        {
            currentSTATE = AI_STATE.SLEEP;
        }
        else if (stat.enablePatrol)
        {
            currentSTATE = AI_STATE.PATROL;
        }
        else
        {
            currentSTATE = AI_STATE.IDLE;
        }

        AI_Action();
    }

}
