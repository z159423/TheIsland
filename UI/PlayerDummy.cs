using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDummy : MonoBehaviour
{
    const float rotateSpeed = 1f;

    [SerializeField] MeshFilter dummyWeapon;
    [SerializeField] SkinnedMeshRenderer dummyChestArmor;

    [SerializeField] Transform cameraTransform;
    [SerializeField] Animator animator;

    [SerializeField] SkinnedMeshRenderer dummySkin;

    [SerializeField] ParticleSystem dustParticle;


    public void Init()
    {
        // cameraTransform.localPosition = new Vector3(0f, 0.85f, 1.85f);
        transform.localPosition = Vector3.zero;
    }

    public void OnChangeDummyEquipment(Mesh mesh)
    {
        dummyWeapon.mesh = mesh;
        // animator.SetTrigger("onWeaponUpgrade");
    }

    public void OnChangeDummyChestArmor(Mesh mesh)
    {
        dummyChestArmor.sharedMesh = mesh;
    }

    public void PlayerDummyRotate(Vector3 deltaPos)
    {
        transform.Rotate(new Vector2(0, deltaPos.x) * rotateSpeed);
    }

    public void ChangeDummySkin(string guid)
    {
        dummySkin.sharedMesh = ResourceManager.Instance.GetPlayerSkin(guid).mesh;
        // dustParticle.Play();
    }

}
