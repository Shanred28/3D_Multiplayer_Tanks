using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Ammunition : NetworkBehaviour
{
    public event UnityAction<int> AmmoCountChanged;

    [SerializeField] private ProjectileProperties _projectileProperties;

    [SyncVar(hook = nameof(SyncAmmoCount))]
    [SerializeField] protected int _syncAmmoCount;

    public ProjectileProperties ProjectileProp => _projectileProperties;
    public int AmmoCount => _syncAmmoCount;

    #region Server

    [Server]
    public void SvAddAmmo(int count)
    {
        _syncAmmoCount += count;
    }

    [Server]
    public bool SvDrawAmmo(int count)
    { 
        if(_syncAmmoCount == 0)
            return false;

        if (_syncAmmoCount >= count)
        { 
            _syncAmmoCount -= count;
            return true;
        }
        return false;
    }
    #endregion

    #region Client
    private void SyncAmmoCount(int oldValue, int newValue)
    {
        AmmoCountChanged?.Invoke(newValue);
    }
    #endregion
}
