using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Destructible : NetworkBehaviour
{
    public UnityAction<int> HitPointChange;

    [SerializeField] private int _maxHitPoint;
    public int MaxHitPoint => _maxHitPoint;
    [SerializeField] private GameObject _destroySfx;

    private int _currentHp;
    public int HitPoint => _currentHp;

    [SyncVar(hook = nameof(ChangeHitPoint))]
    private int _syncCurrentHP;

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
            if (_destroySfx != null)
            {
                GameObject sfx =  Instantiate(_destroySfx, transform.position, Quaternion.identity);
                NetworkServer.Spawn(sfx);
            }

            Destroy(gameObject);
        }
    }

    private void ChangeHitPoint(int oldValue, int newValue)
    {
        _currentHp = newValue;
        HitPointChange?.Invoke(newValue);
    }

    [SyncVar(hook = "T")]
    public NetworkIdentity Owner;

    private void T(NetworkIdentity oldValue, NetworkIdentity newValue )
    { 
    
    }
}
