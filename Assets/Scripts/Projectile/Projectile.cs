using Mirror;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileProperties _properties;
    [SerializeField] private ProjectileMovement _movement;
    [SerializeField] private ProjectileHit _hit;

    [Space(5)]
    [SerializeField] private GameObject _visualModel;

    [Space(5)]
    [SerializeField] private float _delayBeforeDestroy;
    [SerializeField] private float _lifeTime;

    public NetworkIdentity Owner { get; set; }
    public ProjectileProperties Properties => _properties;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        _hit.Check();
        _movement.Move();

        if (_hit.IsHit == true)
            OnHit();
    }

    private void OnHit()
    {
        transform.position = _hit.RaycastHit.point;

        if (NetworkSessionManager.Instance.IsServer == true)
        {
            ProjectileHitResult hitResult = _hit.GetHitResult();

            if (hitResult.type == ProjectileHitType.Penetration || hitResult.type == ProjectileHitType.ModulePenetration)
            {
                SvTakeDamage(hitResult);

                SvAddFrags();
            }

            if (Owner != null)
            {
                Player p = Owner.GetComponent<Player>();

                if(p != null)
                   p.SvInvokeProjectileHit(hitResult);
            }
        }
       
        Destroy();
    }

    private void SvTakeDamage(ProjectileHitResult hitResult)
    {
        float damage = _properties.Damage;
        _hit.HitArmor.Destructible.SvApplyDamage((int)hitResult.damage);
    }

    private void SvAddFrags()
    {
        if (_hit.HitArmor.Type == ArmorType.Module) return;

        if (_hit.HitArmor.Destructible.HitPoint <= 0)
        {
            if (Owner != null)
            {
                MatchMember m = Owner.GetComponent<MatchMember>();

                if (m != null)
                {
                    m.SvAddFrags();
                }
            }
        }
    }

    private void Destroy()
    {
        _visualModel.SetActive(false);
        enabled = false;

        Destroy(gameObject,_delayBeforeDestroy);
    }

    public void SetProperties(ProjectileProperties properties)
    { 
      _properties = properties;
    }
}
