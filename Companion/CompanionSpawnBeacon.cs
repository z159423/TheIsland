using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CompanionSpawnBeacon : MonoBehaviour
{
    [HorizontalGroup("Guid")][ReadOnly][SerializeField] string beaconGuid;
    public string GetGuid() => beaconGuid;
    [HorizontalGroup("Guid")]
    [Button(SdfIconType.ArrowClockwise)]
    void NewGuid()
    {
        beaconGuid = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    [SerializeField] private GameObject companionPrefab;
    [SerializeField] private float respwanTime = 15;

    [SerializeField] private bool randomSkin = true;
    [SerializeField] private SkinnedMeshRenderer defaultSkin;
    [SerializeField] private List<SkinnedMeshRenderer> skins = new List<SkinnedMeshRenderer>();
    [SerializeField] private bool canTrail = false;
    [SerializeField] private bool spawnOnAwake = false;

    private Companion currentCompanion = null;

    private void Start()
    {
        if (spawnOnAwake)
            SpawnCompanion();
    }

    public void SpawnCompanion()
    {
        if (currentCompanion == null)
        {
            currentCompanion = Instantiate(companionPrefab, transform.position, transform.rotation).GetComponentInChildren<Companion>();

            var skin = randomSkin ? skins.GetRandomElement() : defaultSkin;

            bool trial = false;

            if (canTrail)
                if (!SaveManager.Instance.PlayerController.GetUsedCompanionTrial())
                    trial = true;

            currentCompanion.Init(this, skin, trial);
        }
    }

    public void RespwanCountStart()
    {

        this.TaskDelay(respwanTime, () => { SpawnCompanion(); AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.COMPANION, "Respawn - " + beaconGuid); }, true);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        Util.GizmoText($"[Companion]", transform.position, Color.blue);

        Gizmos.DrawWireSphere(transform.position, 0.5f);

        Util.CastGround(transform);
    }

    public void FocusThisBeacon()
    {
        CameraManager.Instance.SetCustomCamera("QuestNavigationCamera", 2.5f, follow: transform);
        currentCompanion.Hello();
    }
}
