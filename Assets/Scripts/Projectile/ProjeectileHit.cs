using UnityEngine;


[RequireComponent(typeof(Projectile))]
public class ProjeectileHit : MonoBehaviour
{
    private const float RAYADVANCE = 1.1f;

    private bool _isHit;
    private RaycastHit _raycastHit;
    private Destructible _hitDestructible;

    public bool IsHit => _isHit;
    public RaycastHit RaycastHit => _raycastHit;
    public Destructible Destructible => _hitDestructible;


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
            if (_raycastHit.transform.root.TryGetComponent(out Destructible destrictible))
            {
                _hitDestructible = destrictible;
            }

            _isHit = true;
        }
    }
}
