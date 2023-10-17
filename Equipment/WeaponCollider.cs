using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponCollider", menuName = "Game/Equipment/WeaponCollider")]
public class WeaponCollider : ScriptableObject
{
    public Vector3 center;
    public Vector3 size;

    public void SetColliderValue(BoxCollider collider)
    {
        collider.center = this.center;
        collider.size = this.size;
    }
}
