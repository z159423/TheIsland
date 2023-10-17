using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : SingletonStatic<PlayerManager>
{
    [field: SerializeField] private PlayerRespawnPointBeacon currentRespawnPoint;

    [field: SerializeField] public Player player { get; private set; }
    [field: SerializeField] public Transform playerObtainTrans { get; private set; }

    [field: SerializeField] public Vector3 playerDefaultRespawnPoint = new Vector3(-3.38f, 3.52f, -101.65f);

    [SerializeField] public GameObject dummyPlayerPrefab;

    private PlayerDummy currentDummy;


    [SerializeField] private float reviveTime = 5;

    public PlayerMovement Movement { get; private set; }

    private void Awake()
    {
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += (s, i) => TouchInit();
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += (s, i) => TouchInit();
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += (s, i) => TouchInit();
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += (s, i) => TouchInit();

        this.SetListener(GameObserverType.IAP.ON_PURCHASE_ASSISTANT_PACK, () => { if (player != null && SaveManager.Instance.IAPController.GetPurchaseAssistantPack()) player.SpawnPermanentCompanion(); });
    }

    public void InitPlayer(Player playerObject)
    {
        this.player = playerObject;
        Movement = player.GetComponent<PlayerMovement>();

        if (currentDummy == null)
            currentDummy = Instantiate(ResourceManager.Instance.GetResource<GameObject>("Animation/Models/Player Dummy"), transform).GetComponentInChildren<PlayerDummy>();
        currentDummy.Init();

        Instantiate(ResourceManager.Instance.GetResource<GameObject>("UI/shop_worker"), transform);
    }

    void TouchInit()
    {
        Movement.TouchInit();
    }

    public void DummyInit() => currentDummy.Init();

    public void ChangeDummyPlayerEquipment(EquipmentType type, Mesh mesh)
    {
        if (type == EquipmentType.CHEST)
        {
            currentDummy.OnChangeDummyChestArmor(mesh);
        }
        else
        {
            currentDummy.OnChangeDummyEquipment(mesh);
        }
    }

    public void ChangeDummySkin(string guid) => currentDummy.ChangeDummySkin(guid);

    public void RotateDummy(Vector2 delta)
    {
        currentDummy.PlayerDummyRotate(delta);
    }

    public void OnPlayerDie()
    {
        //플레이어가 죽었을때 이벤트

        var diePanel = UIManager.Instance.ShowPopup("UI/PlayerDiePanel").GetComponent<PlayerDiePanel>();

        diePanel.ReviveIn(reviveTime);

        StartCoroutine(revive());

        IEnumerator revive()
        {
            yield return new WaitForSeconds(reviveTime);

            player.Revive();
            diePanel.Hide();
        }
    }

    public void StopSavePlayerPosition() => Movement.StopSavePlayerPosition();
}
