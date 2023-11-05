using UnityEngine;

public class VehicleModule : Destructible
{
    [SerializeField] private string _titleModule;
    [SerializeField] private Armor _armor;
    [SerializeField] private float _recoveryTime;

    private float _remainingRecoveryTime;

    private void Awake()
    {
        _armor.SetDestrictible(this);
    }

    private void Start()
    {
        Destroyed += OnModuleDestroyed;
        enabled = false;
    }

    private void Update()
    {
        if (isServer == true)
        {
            _remainingRecoveryTime -= Time.deltaTime;

            if (_remainingRecoveryTime <= 0)
            {
                _remainingRecoveryTime = 0.0f;
                SvRecovery();
                enabled = false;
            }
        }
    }

    private void OnDestroy()
    {
        Destroyed -= OnModuleDestroyed;
    }

    private void OnModuleDestroyed(Destructible arg0)
    {
        _remainingRecoveryTime = _recoveryTime;
        enabled = true;
    }
}
