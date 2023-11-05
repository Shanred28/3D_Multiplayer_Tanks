using UnityEngine;

public enum ProjectileHitType
{ 
  Penetration,
  NoPenetration,
  Ricochet,
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
        }
    }

    public ProjectileHitResult GetHitResult()
    { 
        ProjectileHitResult hitResult = new ProjectileHitResult();

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
        Debug.DrawRay(_raycastHit.point, -_projectile.transform.forward, Color.red);
        Debug.DrawRay(_raycastHit.point, _raycastHit.normal, Color.green);
        Debug.DrawRay(_raycastHit.point, _projectile.transform.right, Color.yellow);

        if (angle > _projectile.Properties.RicochetAngle && _projectile.Properties.Caliber < _hitArmor.Thickness * 3)
            hitResult.type = ProjectileHitType.Ricochet;

        else if (projectilePenetration >= reducedArmor)
            hitResult.type = ProjectileHitType.Penetration;

            
        else if(projectilePenetration < reducedArmor)
            hitResult.type = ProjectileHitType.NoPenetration;

        if(hitResult.type == ProjectileHitType.Penetration)
            hitResult.damage = _projectile.Properties.GetSpreadDamage();
        else
            hitResult.damage = 0;

        hitResult.point = _raycastHit.point;
        

        //For debug result hit
        Debug.Log($"armor: {_hitArmor.Thickness}, reducedArmor: {reducedArmor}, angle: {angle}, norm: {normalization}, penetation: {projectilePenetration}, resultType: {hitResult.type}");

        return hitResult;
    }
}
