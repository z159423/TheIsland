using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CustomTriggerEventTag : MonoBehaviour
{
    [SerializeField] string tag;

    [TitleGroup("Island")]
    [SerializeField] bool islandParticle = false;
    [ShowIf("islandParticle")] [SerializeField] GameObject islandObject;
    [ShowIf("islandParticle")] [ReadOnly] [SerializeField] string islandGuid;
    [ShowIf("islandParticle")] [ReadOnly] [SerializeField] Vector3 islandPosition;

    string guid;
    bool init;

    public bool IsIslandParticle() => islandParticle;
    public string GetIslandGuid() => islandGuid;
    public Vector3 GetIslandPosition() => islandPosition;

    [ShowIf("islandParticle")]
    [Button]
    void IslandFind()
    {
        var islands = FindObjectsOfType<WorldIsland>();
        foreach (var island in islands)
        {
            var find = island.FindIsland(islandObject);
            if (find != null)
            {
                islandGuid = find.Collector.GetGuid();
                islandPosition = islandObject.GetComponent<MeshRenderer>().bounds.center.SetY(islandObject.GetComponent<MeshRenderer>().bounds.max.y);
                break;
            }
        }
    }

    public void CustomTrigger()
    {
        QuestManager.Instance.CustomQuestTrigger(tag);

        print("custom quest triggered : " + tag);

        SaveManager.Instance.QuestController.SaveQeustTrigger(tag);
    }

    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        Init();
    }

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        if (init)
            return;

        init = true;
        guid = System.Guid.NewGuid().ToString();
        QuestManager.Instance.questTriggerTransformDic.Add(guid, new QuestTriggerData() { _tag = this.tag, eventTag = this, _transform = this.transform });
    }
}

public struct QuestTriggerData
{
    public string _tag;
    public CustomTriggerEventTag eventTag;
    public Transform _transform;
}
