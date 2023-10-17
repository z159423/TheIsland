using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender : MonoBehaviour
{
    [SerializeField] protected IAttackTriggerController controller;

    private void Start()
    {
        controller = GetComponentInParent<IAttackTriggerController>();
    }

    public void AttackTriggerOn()
    {
        controller.AttackTriggerOn();
    }

    public void AttackTriggerOff()
    {
        controller.AttackTriggerOff();
    }

    public void SendCustomEvent(string eventName)
    {
        controller.CustomAnimationEvent(eventName);
    }
}
