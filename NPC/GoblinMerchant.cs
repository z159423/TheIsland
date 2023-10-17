using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GoblinMerchant : MonoBehaviour
{
    [SerializeField] Animator goblinAnimtor;
    [SerializeField] Animator skinAnimator;
    [SerializeField] SkinnedMeshRenderer skinMesh;
    [SerializeField] WorldCollector collector;

    [SerializeField] ParticleSystem dustParticle;

    [SerializeField] GameObject cage;

    [Space]
    [SerializeField] PlayerSkinScriptableObejct skinData;

    [SerializeField] ItemStack[] costItem;

    private void OnValidate()
    {
        collector.CanvasPreview(costItem);
    }

    private void Start()
    {
        if (!SaveManager.Instance.PlayerController.CheckOwnedSkin(skinData.Guid))
        {

            skinMesh.sharedMesh = skinData.mesh;

            skinAnimator.gameObject.SetActive(true);
            cage.SetActive(true);

            collector.onComplete.AddListener(OnBuySkin);
            collector.gameObject.SetActive(true);
            collector.Init(costItem);
            collector.CanvasPreview(costItem);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }


    public void OnBuySkin()
    {
        AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.SKIN, "Get Skin - " + skinData.Guid);

        SaveManager.Instance.PlayerController.AcquireNewPlayerSkin(skinData.Guid);

        goblinAnimtor.SetTrigger("Clap");

        cage.transform.DOScale(Vector3.zero, 0.7f).SetEase(Ease.InSine);

        // var rigid = cage.GetComponentInChildren<Rigidbody>();

        // rigid.isKinematic = false;
        // rigid.AddForce((Vector3.up + Vector3.up).normalized * 10, ForceMode.Impulse);
        // rigid.AddTorque((Vector3.up + Vector3.up).normalized * 60, ForceMode.Impulse);


        this.TaskDelay(1.5f, () => skinAnimator.SetTrigger("Hurray"));

        this.TaskDelay(3.5f, () => { dustParticle.Play(); skinAnimator.gameObject.SetActive(false); });
        this.TaskDelay(4.3f, () =>
        {
            PlayerManager.Instance.player.GetComponentInChildren<PlayerSkin>().ChangePlayerSkin(skinData.Guid);

            var UI = UIManager.Instance.ShowScreen("UI/New Skin Acquire UI");

            UI.GetComponentInChildren<NewSkinUIBase>().InitDummy(SaveManager.Instance.PlayerController.GetPlayerCurrentSkin());

        });
    }

}
