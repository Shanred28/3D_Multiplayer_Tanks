using Mirror;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject _visualModel;

    [SerializeField] private float _velocity;
    [SerializeField] private float _lifeTime;
    [SerializeField] private float _mass;

    [SerializeField] private float _damage;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float _damageScatter;
    [SerializeField] private float _impactForce;

    public NetworkIdentity Owner { get; set; }

    private const float RAYADVANCE = 1.1f;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        UpdateProjectile();
    }

    private void UpdateProjectile()
    {
        transform.forward = Vector3.Lerp(transform.forward, -Vector3.up, Mathf.Clamp01(Time.deltaTime * _mass)).normalized;

        Vector3 step = transform.forward * _velocity * Time.deltaTime;
        RaycastHit hit;
       // Vector3 offset = Random.insideUnitSphere * _spreadShootRange;
        //Raycast hit effect
        if (Physics.Raycast(transform.position, transform.forward, out hit, _velocity * Time.deltaTime * RAYADVANCE))
        { 
            transform.position = hit.point;

            if (hit.transform.root.TryGetComponent(out Destructible destrictible))
            {
                if (NetworkSessionManager.Instance.IsServer)
                {
                    float dmg = _damage + Random.Range(-_damageScatter, _damageScatter) * _damage;

                    destrictible?.SvApplyDamage((int)dmg);

                    if (destrictible.HitPoint <= 0)
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
            }

            OnProjectileLifeEnd(hit.collider, hit.point,hit.normal);
            return;
        }
       
        //offset = offset * Vector3.Distance(firePoint.position, pos) * _spreadShootDistanceFactor;


        transform.position += step;
    }

    private void OnProjectileLifeEnd(Collider col, Vector3 pos, Vector3 normal)
    { 
      _visualModel.SetActive(false);
        enabled = false;
    }
}
