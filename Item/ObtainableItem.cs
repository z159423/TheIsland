using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EPOOutline;
using DG.Tweening;
using UnityEngine.AI;

public class ObtainableItem : MonoBehaviour
{
    private Item item;

    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshCollider meshCollider;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Outlinable outlinable;


    [SerializeField] Material[] defaultMats;

    Rigidbody rigid;

    public Rigidbody GetRigidbody() => rigid;

    private void Awake()
    {
        rigid = GetComponentInChildren<Rigidbody>();

        transform.localScale = Vector3.one;
    }

    public void InitAttract(int id)
    {
        item = ResourceManager.Instance.GetItem(id);
        if (item != null)
        {
            meshFilter.sharedMesh = item.Mesh;
            meshCollider.sharedMesh = item.Mesh;

            if (item.Materials != null)
            {
                if (item.Materials.Length > 0)
                {
                    meshRenderer.materials = item.Materials;
                    outlinable.enabled = false;
                }
                else
                {
                    meshRenderer.materials = defaultMats;
                    outlinable.enabled = true;
                }
            }
            else
            {
                meshRenderer.materials = defaultMats;
                outlinable.enabled = true;
            }

        }

        this.TaskDelay(Random.Range(0.3f, 0.5f), Attract);
    }

    public void InitDummy(int id)
    {
        item = ResourceManager.Instance.GetItem(id);
        if (item != null)
        {
            meshFilter.sharedMesh = item.Mesh;
            meshCollider.sharedMesh = item.Mesh;

            if (item.Materials != null)
            {
                if (item.Materials.Length > 0)
                {

                    meshRenderer.materials = item.Materials;
                    outlinable.enabled = false;
                }
                else
                {
                    meshRenderer.materials = defaultMats;
                    outlinable.enabled = true;
                }
            }
            else
            {
                meshRenderer.materials = defaultMats;
                outlinable.enabled = true;
            }
        }
    }


    void Attract()
    {
        transform.DoJumpLive(PlayerManager.Instance.player.playerObtainTrans, 2f, 0.5f).OnComplete(() =>
        {
            SoundManager.Instance.PlaySFX("Item Pickup", 1f);
            HapticManager.Instance.Haptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);

            ObjectPool.Instance.AddPool(gameObject);
        });

        transform.DOScale(0, 0.25f).SetDelay(0.25f);
    }
}

