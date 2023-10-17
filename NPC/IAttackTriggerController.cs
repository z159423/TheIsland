using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackTriggerController
{

    public void AttackTriggerOn();
    public void AttackTriggerOff();

    public void CustomAnimationEvent(string eventString);
}
