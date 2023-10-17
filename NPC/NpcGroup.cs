using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcGroup : MonoBehaviour
{
    [SerializeField] string groupName;
    public NPCBeacon[] npcGroup;

    public void GroupTargeting(NPCObject target)
    {
        foreach (NPCBeacon group in npcGroup)
        {
            group?.spawnedNPC?.GroupTargeting(target);
        }
    }

    private void OnValidate()
    {
        if (npcGroup == null)
            return;

        for (int i = 0; i < npcGroup.Length; i++)
        {
            npcGroup[i].gameObject.name = $"{groupName} [{i}]";
            npcGroup[i].npcGroups = this;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red.SetA(0.5f);
        var bounds = new Bounds(npcGroup[0].transform.position, Vector3.one * 2f);
        for (int i = 1; i < npcGroup.Length; i++)
            bounds.Encapsulate(new Bounds(npcGroup[i].transform.position, Vector3.one * 2f));
        Gizmos.DrawCube(bounds.center, bounds.size.SetY(0.5f));
        Util.GizmoText(groupName, bounds.center.SetY(bounds.max.y + 2f), Color.yellow);
    }
}
