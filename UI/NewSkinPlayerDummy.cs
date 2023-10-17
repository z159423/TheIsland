using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NewSkinPlayerDummy : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer skinMesh;

    public void Init(string skinId)
    {
        skinMesh.sharedMesh = ResourceManager.Instance.GetPlayerSkin(skinId).mesh;

        // transform.DORotate(new Vector3(0, 360, 0), 7f).SetEase(Ease.Linear).SetLoops(-1);
    }
}
