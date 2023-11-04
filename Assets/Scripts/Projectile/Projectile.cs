using Mirror;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileProperties _properties;
    [SerializeField] private ProjectileMovement _movement;
    [SerializeField] private ProjeectileHit _hit;

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

        if (NetworkSessionManager.Instance.IsServer == true && _hit.Destructible != null)
        {
            SvTakeDamage();

            SvAddFrags();
        }

        Destroy();
    }

    private void SvTakeDamage()
    {
        float damage = _properties.Damage;
        _hit.Destructible.SvApplyDamage((int) damage);
    }

    private void SvAddFrags()
    {
        if (_hit.Destructible.HitPoint <= 0)
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
