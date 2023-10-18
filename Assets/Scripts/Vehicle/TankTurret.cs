using UnityEngine;


public class TankTurret : MonoBehaviour
{
    //TODO
    [SerializeField] private Transform _aim;

    [SerializeField] private Transform _tower;
    [SerializeField] private Transform _mask;

    [SerializeField] private float _horizontalRotationSpeed;
    [SerializeField] private float _verticalRotationSpeed;

    [SerializeField] private float _maxTopAngle;
    [SerializeField] private float _maxBottomAngle;

    private TrackTank _tank;
    private float _maskCurretAngle;
    private void Start()
    {
        _tank = GetComponent<TrackTank>();
        _maxTopAngle = -_maxTopAngle;
    }

    private void Update()
    {
        ControlTurretAim();
    }

    private void ControlTurretAim()
    {
        //Tower
        Vector3 locPos = _tower.InverseTransformPoint(_aim.position);
        locPos.y = 0;
        Vector3 locPosGlob = _tower.TransformPoint(locPos);
        _tower.rotation = Quaternion.RotateTowards(_tower.rotation, Quaternion.LookRotation((locPosGlob - _tower.position).normalized, _tower.up), _horizontalRotationSpeed * Time.deltaTime);


        //Mask
        _mask.localRotation = Quaternion.identity;

        locPos = _mask.InverseTransformPoint(_aim.position);
        locPos.x = 0;
        locPosGlob = _mask.TransformPoint(locPos);

        float targetAngle = -Vector3.SignedAngle((locPosGlob - _mask.position).normalized, _mask.forward, _mask.right);
        targetAngle = Mathf.Clamp(targetAngle, _maxTopAngle, _maxBottomAngle);

        _maskCurretAngle = Mathf.MoveTowards(_maskCurretAngle, targetAngle, _verticalRotationSpeed * Time.deltaTime);
        _mask.localRotation = Quaternion.Euler(_maskCurretAngle,0,0);
    }
}
