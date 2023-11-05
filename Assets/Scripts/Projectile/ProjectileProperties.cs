using UnityEngine;

public enum ProjectileType   
{ 
    ArmorPiercing,
    HightExplosive,
    Subcaliber
}

[CreateAssetMenu]
public class ProjectileProperties : ScriptableObject
{
    [SerializeField] private ProjectileType _type;

    [Header("Common")]
    [SerializeField] private Projectile _projectilePref;
    [SerializeField] private Sprite _iconProj;

    [Header("Movement")]
    [SerializeField] private float _vilocity;
    [SerializeField] private float _mass;
    [SerializeField] private float _impactForce;

    [Header("Damage")]
    [SerializeField] private float _damage;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float _damageSpred;

    [Header("Caliber")]
    [SerializeField] private float _caliber;

    [Header("Armor Penetration")]
    [SerializeField] private float _armorPenetration;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float _armorPenetrationSpread;
    [Range(0.0f, 90.0f)]
    [SerializeField] private float _normalizationAngle;
    [Range(0.0f, 90.0f)]
    [SerializeField] private float _ricochetAngle;

    public ProjectileType Type => _type;

    public Projectile ProjectilePref => _projectilePref;
    public Sprite IconProjectile => _iconProj;
    public float Velocity => _vilocity;
    public float Mass => _mass;
    public float ImpactForce => _impactForce;

    public float Damage => _damage;
    public float DamageSpred => _damageSpred;
    public float Caliber => _caliber;

    public float ArmorPenetration => _armorPenetration;
    public float ArmorPenetrationSpread => _armorPenetrationSpread;
    public float NormalizationAngle => _normalizationAngle;
    public float RicochetAngle => _ricochetAngle;

    public float GetSpreadDamage()
    {
        return _damage * Random.Range(1 - _damageSpred, 1 + _damageSpred);
    }

    public float GetSpreadArmorPenetration()
    {
        return _armorPenetration * Random.Range(1 - _armorPenetrationSpread, 1 + _armorPenetrationSpread);
    }
}
