using UnityEngine;


public class TankTurret : Turret
{
    [SerializeField] private Transform _tower;
    [SerializeField] private Transform _mask;

    [SerializeField] private float _horizontalRotationSpeed;
    [SerializeField] private float _verticalRotationSpeed;

    [SerializeField] private float _maxTopAngle;
    [SerializeField] private float _maxBottomAngle;

    [Header("SFX")]
    [SerializeField] private AudioSource _soundFire;
    [SerializeField] private ParticleSystem _particleFireSFX;
    [SerializeField] private float _forceRecoil;

    private TrackTank _tank;
    private float _maskCurretAngle;
    private Rigidbody _rigidbodyTank;
    private void Start()
    {
        _tank = GetComponent<TrackTank>();
        _rigidbodyTank = GetComponent<Rigidbody>();
        _maxTopAngle = -_maxTopAngle;
    }

    protected override void Update()
    {
        base.Update();

        ControlTurretAim();
    }

    protected override void OnFire()
    {
        base.OnFire();

        GameObject projectile = Instantiate(ProjectilePref.gameObject);

        projectile.transform.position = _launchPoint.position;
        projectile.transform.forward = _launchPoint.forward;

        FireSfx();
    }

    private void FireSfx()
    {
        _soundFire.Play();
        _particleFireSFX.Play();

        _rigidbodyTank.AddForceAtPosition(- _mask.forward * _forceRecoil, _mask.position, ForceMode.Impulse);
    }

    private void ControlTurretAim()
    {
        //Tower
        Vector3 locPos = _tower.InverseTransformPoint(_tank.NetAimPoit);
        locPos.y = 0;
        Vector3 locPosGlob = _tower.TransformPoint(locPos);
        _tower.rotation = Quaternion.RotateTowards(_tower.rotation, Quaternion.LookRotation((locPosGlob - _tower.position).normalized, _tower.up), _horizontalRotationSpeed * Time.deltaTime);


        //Mask
        _mask.localRotation = Quaternion.identity;

        locPos = _mask.InverseTransformPoint(_tank.NetAimPoit);
        locPos.x = 0;
        locPosGlob = _mask.TransformPoint(locPos);

        float targetAngle = -Vector3.SignedAngle((locPosGlob - _mask.position).normalized, _mask.forward, _mask.right);
        targetAngle = Mathf.Clamp(targetAngle, _maxTopAngle, _maxBottomAngle);

        _maskCurretAngle = Mathf.MoveTowards(_maskCurretAngle, targetAngle, _verticalRotationSpeed * Time.deltaTime);
        _mask.localRotation = Quaternion.Euler(_maskCurretAngle,0,0);
    }
}
