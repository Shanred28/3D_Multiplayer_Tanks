using UnityEngine;

public enum ProjectileHitType
{ 
  Penetration,
  NoPenetration,
  Ricochet,
  ModulePenetration,
  ModuleNoPenetration,
  Enviroment
}

public class ProjectileHitResult
{ 
    public ProjectileHitType type;
    public float damage;
    public Vector3 point;
}

[RequireComponent(typeof(Projectile))]
public class ProjectileHit : MonoBehaviour
{
    private const float RAYADVANCE = 1.1f;

    private bool _isHit;
    private RaycastHit _raycastHit;
    private Armor _hitArmor;

    public bool IsHit => _isHit;
    public RaycastHit RaycastHit => _raycastHit;
    public Armor HitArmor => _hitArmor;


    private Projectile _projectile;

    private void Awake()
    {
        _projectile = GetComponent<Projectile>();
    }

    public void Check()
    { 
       if(_isHit == true) return;

        if (Physics.Raycast(transform.position, transform.forward, out _raycastHit, _projectile.Properties.Velocity * Time.deltaTime * RAYADVANCE))
        {
            if (_raycastHit.collider.TryGetComponent(out Armor armor))
            { 
              _hitArmor = armor;
            }
            _isHit = true;

            if(_raycastHit.collider.TryGetComponent(out PartialBarrier barrier))
                _isHit = false;
        }
    }

    public ProjectileHitResult GetHitResult()
    {
        ProjectileHitResult hitResult = new ProjectileHitResult();

        hitResult.damage = 0;

        if (_hitArmor == null)
        {
            hitResult.type = ProjectileHitType.Enviroment;
            hitResult.point = _raycastHit.point;
            return hitResult;
        }

        float normalization = _projectile.Properties.NormalizationAngle;

        if (_projectile.Properties.Caliber > _hitArmor.Thickness * 2)
        {
            normalization = (_projectile.Properties.NormalizationAngle * 1.4f * _projectile.Properties.Caliber) / _hitArmor.Thickness;
        }

        float angle = Mathf.Abs(Vector3.SignedAngle(-_projectile.transform.forward, _raycastHit.normal, _projectile.transform.right)) - normalization;
        float reducedArmor = _hitArmor.Thickness / Mathf.Cos(angle * Mathf.Deg2Rad);
        float projectilePenetration = _projectile.Properties.GetSpreadArmorPenetration();

        //Visual angles for debug
/*        Debug.DrawRay(_raycastHit.point, -_projectile.transform.forward, Color.red);
        Debug.DrawRay(_raycastHit.point, _raycastHit.normal, Color.green);
        Debug.DrawRay(_raycastHit.point, _projectile.transform.right, Color.yellow);*/

        if (angle > _projectile.Properties.RicochetAngle && _projectile.Properties.Caliber < _hitArmor.Thickness * 3 && _hitArmor.Type == ArmorType.Vehicle)
            hitResult.type = ProjectileHitType.Ricochet;

        else if (projectilePenetration >= reducedArmor)
        {
            hitResult.type = ProjectileHitType.Penetration;
            hitResult.damage = _projectile.Properties.GetSpreadDamage();
        }
            
        else if (projectilePenetration < reducedArmor)
        {
            hitResult.type = ProjectileHitType.NoPenetration;
            if (_projectile.Properties.Type == ProjectileType.HightExplosive)
            { 
                float dmg = _projectile.Properties.GetSpreadDamage();
                hitResult.damage = dmg / 2;
            }
        }

        if (_hitArmor.Type == ArmorType.Module)
        {
            if (hitResult.type == ProjectileHitType.Penetration)
                hitResult.type = ProjectileHitType.ModulePenetration;

            if (hitResult.type == ProjectileHitType.NoPenetration)
                hitResult.type = ProjectileHitType.ModuleNoPenetration;
        }

        hitResult.point = _raycastHit.point;
        
        //For debug result hit
        //Debug.Log($"armor: {_hitArmor.Thickness}, reducedArmor: {reducedArmor}, angle: {angle}, norm: {normalization}, penetation: {projectilePenetration}, resultType: {hitResult.type}");

        return hitResult;
    }
}
