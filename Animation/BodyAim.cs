using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BodyAim : MonoBehaviour
{
    [SerializeField] private NPC npc;
    [SerializeField] private MultiAimConstraint aim;
    [SerializeField] private RigBuilder builder;

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 nullTargetPosition;

    bool isChange = false;

    private float currnetWeight = 0.3f;


    private void Start()
    {
        if (npc == null)
            GetComponent<NPC>();

        if (aim == null)
            GetComponentInChildren<MultiAimConstraint>();
    }

    private void Update()
    {
        if (npc.currentTarget != null)
        {
            target.transform.position = npc.currentTarget.transform.position;
        }
        else
        {
            target.transform.localPosition = nullTargetPosition;
        }
    }

    private void FixedUpdate()
    {
        aim.weight = Mathf.MoveTowards(aim.weight, currnetWeight, 1f * Time.deltaTime);
    }

    public void ChangeRigWeight(float weight)
    {
        currnetWeight = weight;
        //builder.Build();
    }
}
