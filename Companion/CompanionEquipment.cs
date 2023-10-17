using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionEquipment : MonoBehaviour
{
    [SerializeField] private Companion companion;

    [field: SerializeField] public Equipment equipment { get; private set; }
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private SkinnedMeshRenderer skinnedMesh;


    private ParticleSystem weaponTrail;
    private TrailRenderer weaponTrailRenderer;
    [SerializeField] private BoxCollider weaponCollider;

    private void Awake()
    {
        companion = GetComponentInParent<Companion>();
        weaponTrail = GetComponentInChildren<ParticleSystem>();
        weaponCollider = GetComponent<BoxCollider>();
        meshFilter = GetComponent<MeshFilter>();
        weaponTrailRenderer = GetComponentInChildren<TrailRenderer>();

        if (companion == null)
            Debug.LogError("Not Found companion Scripts");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (companion.triggerdCollider.Contains(other) || companion == null)
            return;

        if (other.TryGetComponent<WorldEnviromentObject>(out WorldEnviromentObject breakable))
        {
            if (breakable.CanBreak() && (equipment.Type == EquipmentType.AXE || equipment.Type == EquipmentType.PICKAXE))
            {
                if (equipment.Type == breakable.GetBreakEquipmentType())
                {
                    breakable.Damage(equipment.Damage, equipment);
                    companion.triggerdCollider.Add(other);
                }

                GameObject particle = null;

                if (breakable.GetBreakEquipmentType() == EquipmentType.PICKAXE)
                {
                    particle = ObjectPool.Instance.GetPool("Particle/SparkRadialExplosionYellow");
                    SoundManager.Instance.PlaySFX("Pickaxe Hit", 0.5f, Random.Range(0.8f, 1.2f));
                }
                else if (breakable.GetBreakEquipmentType() == EquipmentType.AXE)
                {
                    particle = ObjectPool.Instance.GetPool("Particle/PowerupActivateYellow");
                    SoundManager.Instance.PlaySFX("Axe Hit", 0.5f, Random.Range(0.8f, 1.2f));
                }

                if (particle != null)
                {
                    particle.transform.position = other.ClosestPointOnBounds(GetComponent<Collider>().bounds.center);
                    this.TaskDelay(2f, () => ObjectPool.Instance.AddPool(particle), true);
                };
            }
        }

        if (other.TryGetComponent<Enemy>(out Enemy enemy) && !enemy.isDie)
        {
            enemy.TakeDamage(equipment.Damage, companion.GetComponent<Companion>());

            companion.triggerdCollider.Add(other);

            var particle = ObjectPool.Instance.GetPool("Particle/SwordHitYellow");
            SoundManager.Instance.PlaySFX("Sword Hit", 0.5f, Random.Range(0.8f, 1.2f));

            particle.transform.position = other.ClosestPointOnBounds(GetComponent<Collider>().bounds.center);

            this.TaskDelay(2f, () => ObjectPool.Instance.AddPool(particle), true);
        }
    }

    public void ColliderOn()
    {
        weaponCollider.enabled = true;
    }

    public void ColliderOff()
    {
        weaponCollider.enabled = false;
    }

    public void WeaponTrailOn()
    {
        // if (weaponTrail != null && equipment.trailOn)
        //     weaponTrail.Play();

        if (weaponTrailRenderer != null && equipment.trailOn)
            weaponTrailRenderer.emitting = true;
    }


    public void WeaponTrailOff()
    {
        // if (weaponTrail != null && equipment.trailOn)
        //     weaponTrail.Stop();

        if (weaponTrailRenderer != null && equipment.trailOn)
            weaponTrailRenderer.emitting = false;
    }

    public void ChangeEquipment(Equipment equipment)
    {
        this.equipment = equipment;

        if (equipment.mesh != null)
        {
            if (meshFilter != null)
                meshFilter.mesh = equipment.mesh;

            if (skinnedMesh != null)
                skinnedMesh.sharedMesh = equipment.mesh;
        }

        if (equipment.weaponCollider != null)
            equipment.weaponCollider.SetColliderValue(weaponCollider);
    }
}
