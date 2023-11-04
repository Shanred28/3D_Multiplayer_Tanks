using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Destructible : NetworkBehaviour
{
    public event UnityAction<int> HitPointChanged;
    public event UnityAction<Destructible> Destroyed;
    public event UnityAction<Destructible> Recovered;

    [SerializeField] private int _maxHitPoint;
    public int MaxHitPoint => _maxHitPoint;

    [SerializeField] private UnityEvent _eventDestroyed;
    [SerializeField] private UnityEvent _eventRecovered;

    private int _currentHp;
    public int HitPoint => _currentHp;
   
    [SyncVar(hook = nameof(SyncHitPoint))]
    private int _syncCurrentHP;

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();

        _syncCurrentHP = _maxHitPoint;
        _currentHp = _maxHitPoint;
    }

    [Server]
    public void SvApplyDamage(int damage)
    {
        _syncCurrentHP -= damage;

        if (_syncCurrentHP <= 0)
        {
            _syncCurrentHP = 0;
            RpcDestroy();
        }
    }

    [Server]
    protected void SvRecovery()
    {
        _syncCurrentHP = _maxHitPoint;
        _currentHp = _maxHitPoint;

        RpcRecovery();
    }

    #endregion


    #region Client

     private void SyncHitPoint(int oldValue, int newValue)
     {
        _currentHp = newValue;
        HitPointChanged?.Invoke(newValue);
     }

    [ClientRpc]
    private void RpcDestroy()
    {
       Destroyed?.Invoke(this);
        _eventDestroyed?.Invoke();
    }

    [ClientRpc]
    private void RpcRecovery()
    { 
      Recovered?.Invoke(this);
        _eventRecovered?.Invoke();
    }

    #endregion
}
