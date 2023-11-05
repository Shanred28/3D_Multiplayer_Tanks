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

            if (hitResult.type == ProjectileHitType.Penetration)
            {
                SvTakeDamage(hitResult);

                SvAddFrags();
            }

            Owner.GetComponent<Player>().SvInvokeProjectileHit(hitResult);
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
        if (_hit.HitArmor.Destructible.HitPoint <= 0)
        {
            if (Owner != null)
            {
                Player player = Owner.GetComponent<Player>();

                if (player != null)
                {
                    player.Frags++;
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
