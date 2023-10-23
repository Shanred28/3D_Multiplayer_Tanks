using UnityEngine;
using Mirror;
using UnityEngine.Events;

public enum TypeProjectile
{ 
    TypeA,
    TypeB
}

public class Turret : NetworkBehaviour
{
    public UnityAction<int, int> AmmoChanged;

    [SerializeField] protected Transform _launchPoint;
    public Transform LaunchPoint => _launchPoint;

    [SerializeField] private float _fireRate;
    [SerializeField] private Projectile _projectilePrefA;
    [SerializeField] private Projectile _projectilePrefB;
    public Projectile ProjectilePrefA => _projectilePrefA;
    public Projectile ProjectilePrefB => _projectilePrefB;

    private float _fireTimer;
    public float FireTimerNormalize => _fireTimer / _fireRate;

    [SyncVar]
    [SerializeField] protected int _ammoCountA;
    [SerializeField] protected int _ammoCountB;
    public int AmmoCountA => _ammoCountA;
    public int AmmoCountB => _ammoCountB;

    [SerializeField] protected float _minSpreadShootRange;
    [SerializeField] protected float _maxSpreadShootRange;
    [SerializeField] protected float _currentSpreadShootRange;
    public float CurrentSpreadShootRange => _currentSpreadShootRange;

    [Server]
    public void SvAddAmmo(int count, int type)
    {
        if (type == 0)
        {
            _ammoCountA += count;
        }
        else
             _ammoCountB += count;

        RpcAmmoChanged(type);
    }

    [Server]
    protected virtual bool SvDrawAmmo(int count, int type)
    {
        if (type == 0)
        {
            if (_ammoCountA == 0) return false;
            if (_ammoCountA >= count)
            {
                _ammoCountA -= count;
                RpcAmmoChanged(type);
                return true;
            }
        }
        if (type == 1)
        {
            if (_ammoCountB == 0) return false;
            if (_ammoCountB >= count)
            {
                _ammoCountB -= count;
                RpcAmmoChanged(type);
                return true;
            }
        }

        return false;
    }

    [ClientRpc]
    private void RpcAmmoChanged(int type)
    {
        if (type == 0)
            AmmoChanged?.Invoke(AmmoCountA, 0);
        else
            AmmoChanged?.Invoke(AmmoCountB, 1);
    }

    protected virtual void OnFire() { }

    public void Fire(int type)
    {
        if (isOwned == false) return;

        if (isClient == true)
            CmdFire(type);
    }

    [Command]
    private void CmdFire(int type)
    {
        if (_fireTimer > 0) return;

        if (SvDrawAmmo(1, type) == false) return;

        OnFire();
        _fireTimer = _fireRate;

        RpcFire();
    }

    [ClientRpc]
    private void RpcFire()
    {
        if (isServer == true) return;

        _fireTimer = _fireRate;

        OnFire();
    }

    protected virtual void Update()
    {
        if (_fireTimer > 0)
        {
            _fireTimer -= Time.deltaTime;
        }
    }

    public void Reloded()
    {
        _fireTimer = _fireRate;
    }
}
