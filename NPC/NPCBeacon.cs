using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

public class NPCBeacon : MonoBehaviour
{
    [SerializeField] protected NPCScriptableObject npc;

    [SerializeField] bool startSpawn;
    [field: SerializeField] public NPC spawnedNPC { get; protected set; }

    [TitleGroup("Respawn")][SerializeField] protected bool respawnable = true;
    [TitleGroup("Respawn")][ShowIf("respawnable")][SerializeField] protected float respawnTime = 5f;

    [field: SerializeField] public NpcGroup npcGroups { get; set; }


    private void Start()
    {
        if (startSpawn)
            SpawnNPC();
    }

    public void SpawnNPC()
    {
        // var scriptableObjects = Resources.LoadAll<NPCScriptableObject>("NPC").ToList();
        // var find = scriptableObjects.Find(f => f.ID == npc.ID);

        if (npc != null)
        {
            var _npc = Instantiate(npc.Prefab, transform.position, transform.rotation);
            _npc.GetComponentInChildren<NPC>().InitStat(npc, this);

            spawnedNPC = _npc.GetComponentInChildren<NPC>();
        }
        else
        {
            Debug.LogError("해당 ID에 해당하는 NPC가 없습니다.");
        }
    }

    public void Respawn()
    {
        if (!respawnable)
            return;

        this.TaskDelay(respawnTime, () =>
        {
            SpawnNPC();
            var particle = ObjectPool.Instance.GetPool("Particle/FX_ShardRock_Dust_End_01");
            particle.transform.position = transform.position;
            particle.GetComponentInChildren<ParticleSystem>().Play();
            ObjectPool.Instance.AddPool(particle, 3f);
        });
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        Util.GizmoText($"[SPAWN] {npc.Name} {(respawnable ? respawnTime : "")}{(respawnable ? "s" : "")}", transform.position, Color.blue);
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        Util.CastGround(transform);
    }

    public void GroupTargeting(NPCObject target)
    {
        if (npcGroups != null)
            npcGroups.GroupTargeting(target);
    }
}