using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class IntroController : SerializedMonoBehaviour
{
    [SerializeField] private Animator tentacle1;
    [SerializeField] private Animator tentacle2;

    [SerializeField] private Animator tentacle3;


    [SerializeField] private Animator player;

    [SerializeField] private ParticleSystem boatDestoryParticle;
    [SerializeField] private ParticleSystem waterSplashParticle;
    [SerializeField] private ParticleSystem chest_waterSplashParticle;


    [SerializeField] private Transform chest;
    [SerializeField] private Transform tentacleEnd;

    [SerializeField] private CinemachineVirtualCamera currentCamera;

    [SerializeField] private Transform ui;
    [SerializeField] private Button skipBtn;

    [SerializeField] private Dictionary<string, CinemachineVirtualCamera> cameraDic = new Dictionary<string, CinemachineVirtualCamera>();

    // Start is called before the first frame update

    public void IntroSeq_tentacleAttack() => tentacle1.SetTrigger("Attack");

    public void BoatDestoryParticle() => boatDestoryParticle.Play();

    public void PlayerFalling() => player.SetTrigger("Falling");

    public void PlayerWaterSplash() => waterSplashParticle.Play();
    public void ChestWaterSplash() => chest_waterSplashParticle.Play();

    private void Start()
    {
        this.TaskDelay(1.5f, () => skipBtn.gameObject.SetActive(true));
    }


    public void ChangeCamera(string tag)
    {
        currentCamera.Priority = 9;

        currentCamera = cameraDic.First(f => f.Key == tag).Value;
        currentCamera.m_Priority = 10;
    }

    public void Tentacle_Grip_Chest() => tentacle3.SetTrigger("GripChest");

    public void ChangeChestParent()
    {
        chest.SetParent(tentacleEnd);
        // chest.localRotation = Quaternion.Euler(-105f, 158, 11);
        // chest.DOLocalMove(new Vector3(0.057f, -0.264f, 0.052f), 0.3f);
    }

    public void ChangeScene()
    {
        var clone = Instantiate(ResourceManager.Instance.GetResource<GameObject>("UI/Screen Fade"), ui);
        clone.GetComponent<UIScreenFade>().Hide(() => UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Game"), false);

    }

    public void SkipIntro()
    {
        AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.UI, "Intro Skip");
        skipBtn.gameObject.SetActive(false);
        ChangeScene();
    }
}
