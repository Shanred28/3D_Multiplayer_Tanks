using UnityEngine;


[RequireComponent(typeof(Vehicle))]
public class VehicleCamouflage : MonoBehaviour
{
    [Header("DistanceVisable")]
    [SerializeField] private float _baseDistance;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float _percent;
    [SerializeField] private float _percentLerpRate;
    [SerializeField] private float _percentOnFire;

    private Vehicle _vehicle;
    private float _targetPercent;
    private float _currentDistance;
    public float CurrentDistance => _currentDistance;
    private void Start()
    {
        if (NetworkSessionManager.Instance.IsServer == false) return;

        _vehicle = GetComponent<Vehicle>();
        _vehicle.Turret.Shot += OnShot;
    }

    private void Update()
    {
        if (NetworkSessionManager.Instance.IsServer == false) return;

        if (_vehicle.NormalizedLinearVelocity > 0.1f)
            _targetPercent = 0.5f;

        if (_vehicle.NormalizedLinearVelocity <= 0.01f)
            _targetPercent = 1.0f;

        _percent = Mathf.MoveTowards(_percent, _targetPercent, Time.deltaTime * _percentLerpRate);
        _percent = Mathf.Clamp01(_percent);
        _currentDistance = _baseDistance * _percent;
    }

    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance.IsServer == false) return;
        _vehicle.Turret.Shot -= OnShot;
    }

    private void OnShot()
    {
        _percent = _percentOnFire;
    }
}
