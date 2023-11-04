using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Turret : NetworkBehaviour
{
    public event UnityAction<int> UpdateSelectedAmmunation;

    [SerializeField] protected Transform _launchPoint;
    public Transform LaunchPoint => _launchPoint;

    [SerializeField] private float _fireRate;
    [SerializeField] private Ammunition[] _ammunitions;
    public Ammunition[] Ammunitions => _ammunitions;

    private float _fireTimer;
    public float FireTimerNormalize => _fireTimer / _fireRate;

    public ProjectileProperties SelectedProjectile => Ammunitions[_syncSelectedAmmunitionIndex].ProjectileProp;

    [SyncVar]
    private int _syncSelectedAmmunitionIndex;
    public int SelectedAmmunitionIndex => _syncSelectedAmmunitionIndex;

    [SerializeField] protected float _minSpreadShootRange;
    [SerializeField] protected float _maxSpreadShootRange;
    [SerializeField] protected float _currentSpreadShootRange;
    public float CurrentSpreadShootRange => _currentSpreadShootRange;

    protected virtual void OnFire() { }

    public void SetSelectProjectile(int index)
    { 
        if(isOwned == false) return;

        if(index < 0 || index > _ammunitions.Length) return;

        _syncSelectedAmmunitionIndex = index;

        if (isClient == true)
            CmdReloadAmmunation();

        UpdateSelectedAmmunation?.Invoke(index);
    }

    public void Fire()
    {
        if (isOwned == false) return;

        if (isClient == true)
            CmdFire();
    }

    [Command]
    private void CmdReloadAmmunation()
    {
        _fireTimer = _fireRate;
    }

    [Command]
    private void CmdFire()
    {
        if (_fireTimer > 0) return;

        if (_ammunitions[_syncSelectedAmmunitionIndex].SvDrawAmmo(1) == false) return;

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
}
