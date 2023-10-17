using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NPC", menuName = "Game/NPC")]
public class NPCScriptableObject : SerializedScriptableObject
{
    [field: SerializeField][field: BoxGroup("INFO")] public int ID { get; private set; }
    [field: SerializeField][field: BoxGroup("INFO")] public string Name { get; private set; }
    [field: SerializeField][field: BoxGroup("INFO")] public string Description { get; private set; }
    [field: SerializeField][field: BoxGroup("INFO")] public AI_TYPE ai_Type { get; private set; }

    [field: SerializeField][field: BoxGroup("STAT")] public int MaxHp { get; private set; }
    [field: SerializeField][field: BoxGroup("STAT")] public int Level { get; private set; }
    [field: SerializeField][field: BoxGroup("STAT")] public float MoveSpeed { get; private set; }
    [field: SerializeField][field: BoxGroup("STAT")] public int Damage { get; private set; }
    [field: SerializeField][field: BoxGroup("STAT")] public int Armor { get; private set; }

    [field: SerializeField][field: BoxGroup("STAT")] public float SearchingRadius { get; private set; }
    [field: SerializeField][field: BoxGroup("STAT")] public float AttackDist { get; private set; }

    [field: Space]
    [field: SerializeField][field: BoxGroup("STAT")] public bool enablePatrol { get; private set; } = true;
    [field: SerializeField][field: BoxGroup("STAT")][field: ShowIf("enablePatrol")] public float patrolRange { get; private set; } = 3f;
    [field: SerializeField][field: BoxGroup("STAT")][field: ShowIf("enablePatrol")] public float patrolMoveSpeed { get; private set; }
    [field: SerializeField][field: BoxGroup("STAT")][field: ShowIf("enablePatrol")][field: MinMaxSlider(5, 15, true)] public Vector2 patrolTime { get; private set; } = new Vector2(5, 10);
    [field: Space]

    [field: SerializeField][field: BoxGroup("STAT")] public bool enableSleep { get; private set; } = false;
    [field: SerializeField][field: BoxGroup("STAT")][field: ShowIf("enableSleep")] public Vector3 sleepParticlePosition { get; private set; } = new Vector3(0, 0.75f, 0);

    [field: Space]
    [field: SerializeField][field: BoxGroup("STAT")] public bool faceAnimation { get; private set; } = false;
    [field: SerializeField][field: BoxGroup("STAT")][field: ShowIf("faceAnimation")] public Dictionary<string, faceOffset> faceDicCustom = new Dictionary<string, faceOffset>();
    [field: SerializeField][field: BoxGroup("STAT")][field: ShowIf("faceAnimation")] public Dictionary<AI_STATE, faceOffset> faceDicForAi_State = new Dictionary<AI_STATE, faceOffset>();


    public struct faceOffset
    {
        public Vector2 eyeOffset;
        public Vector2 mouthOffset;
    }

    [field: SerializeField] public ItemDropTableScriptable itemDropTable { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
}
