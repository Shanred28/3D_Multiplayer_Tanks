using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Turret : NetworkBehaviour
{
    public UnityAction<int> AmmoChanged;

    [SerializeField] protected Transform _launchPoint;
    public Transform LaunchPoint => _launchPoint;

    [SerializeField] private float _fireRate;
    [SerializeField] private Projectile _projectilePref;
    public Projectile ProjectilePref => _projectilePref;

    private float _fireTimer;
    public float FireTimerNormalize => _fireTimer / _fireRate;

    [SyncVar]
    [SerializeField] protected int _ammoCount;
    public int AmmoCount => _ammoCount;

    [Server]
    public void SvAddAmmo(int count)
    {
        _ammoCount += count;
        RpcAmmoChanged();
    }

    [Server]
    protected virtual bool SvDrawAmmo(int count)
    { 
        if(_ammoCount == 0) return false;

        if (_ammoCount >= count)
        { 
            _ammoCount -= count;
            RpcAmmoChanged();
            return true;
        }

        return false;
    }

    [ClientRpc]
    private void RpcAmmoChanged()
    { 
        AmmoChanged?.Invoke(AmmoCount);
    }

    protected virtual void OnFire() { }

    public void Fire()
    {
        if (isOwned == false) return;

        if (isClient == true)
            CmdFire();
    }

    [Command]
    private void CmdFire()
    {
        if (_fireTimer > 0) return;

        if(SvDrawAmmo(1) == false) return;

        OnFire();
        _fireTimer = _fireRate;

        RpcFire();
    }

    [ClientRpc]
    private void RpcFire()
    { 
      if(isServer == true) return;

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
}
