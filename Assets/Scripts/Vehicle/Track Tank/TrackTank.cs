using Mirror;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrackWheelRow
{
    [SerializeField] private WheelCollider[] _colliders;
    [SerializeField] private Transform[] _meshes;

    public float _minRpm;

    //Public
    public void SetTorque(float torque)
    {
        for (int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].motorTorque = torque;
        }
    }

    public void Break(float breakTorquee)
    {
        for (int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].brakeTorque = breakTorquee;
        }
    }

    public void Reset()
    {
        for (int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].brakeTorque = 0;
            _colliders[i].motorTorque = 0;
        }
    }

    public void SetSidewayStiffness(float stiffness)
    { 
         WheelFrictionCurve wheelFrictionCurve = new WheelFrictionCurve();

        for (int i = 0; i < _colliders.Length; i++)
        {
            wheelFrictionCurve = _colliders[i].sidewaysFriction;
            wheelFrictionCurve.stiffness = stiffness;

            _colliders[i].sidewaysFriction = wheelFrictionCurve;
        }
    }

    public void UpdateMeshTransform()
    {
        //Find min rpm
        List<float> allRpm = new List<float>();

        for (int i = 0; i < _colliders.Length; i++)
        {
            if (_colliders[i].isGrounded == true)
            {
                allRpm.Add(_colliders[i].rpm);
            }
        }

        if (allRpm.Count > 0)
        {
            _minRpm = Mathf.Abs( allRpm[0]);
            for (int i = 0; i < allRpm.Count; i++)
            {
                if (Mathf.Abs(allRpm[i]) < _minRpm)
                {
                    _minRpm = Mathf.Abs (allRpm[i]);
                }
            }
            _minRpm = _minRpm * Mathf.Sign(allRpm[0]);
        }

        float angle = _minRpm * 360.0f / 60.0f * Time.fixedDeltaTime;

        for (int i = 0; i < _meshes.Length; i++)
        {
            Vector3 position;
            Quaternion rotation;

            _colliders[i].GetWorldPose(out position, out rotation);

            _meshes[i].position = position;
            _meshes[i].Rotate(angle,0,0);
        }
    }

    public void UpdateMeshRotationByRpm(float rpm)
    {
        float angle = rpm * 360.0f / 60.0f * Time.fixedDeltaTime;

        for (int i = 0; i < _meshes.Length; i++)
        {
            Vector3 position;
            Quaternion rotation;

            _colliders[i].GetWorldPose(out position, out rotation);

            _meshes[i].position = position;
            _meshes[i].Rotate(angle,0,0);
        }
    }
}

public class TrackTank : Vehicle
{
    public override float LinearVelocity => _rb.velocity.magnitude;

    [SerializeField] private Transform _centerOfMass;

    [Header("Tracks")]   
    [SerializeField] private TrackWheelRow _leftWheelRow;
    [SerializeField] private TrackWheelRow _rightWheelRow;
    [SerializeField] private GameObject _visualModel;
    [SerializeField] private GameObject _destroedPref;

    [Header("Movement")]
    [SerializeField] private ParameterCurve _forwardTorqueCurve;
    [SerializeField] private float _maxForwardTorque;

    [SerializeField] private ParameterCurve _backwardTorqueCurve;
    [SerializeField] private float _maxBackwardMotorTorque;
    
    [SerializeField] private float _breakTorque;
    [SerializeField] private float _rollingResistance;

    [Header("Rotation")]
    [SerializeField] private float _rotateTorqueInPlace;
    [SerializeField] private float _rotateBreakInPlace;
    [Space(2)]
    [SerializeField] private float _rotateTorqueInMotion;
    [SerializeField] private float _rotateBreakInMotion;

    [Header("Friction")]
    [SerializeField] private float _minSidewayStiffnessInPlace;
    [SerializeField] private float _minSidewayStiffnessInMotion;

    public float LeftWheelRpm => _leftWheelRow._minRpm;
    public float RightWheelRpm => _rightWheelRow._minRpm;

    private Rigidbody _rb;
    private float _currentMotorTorque;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {      
        _rb.centerOfMass = _centerOfMass.localPosition;
        Destroyed += OnTrackTankDestroyed;
    }

    private void OnDestroy()
    {
        Destroyed -= OnTrackTankDestroyed;
    }

    private void FixedUpdate()
    {
        if (isServer == true)
        {
            UpdateMotorTorque();

            SvUpdateWheelRpm(LeftWheelRpm, RightWheelRpm);

            SvUpdateLinearVelocity(LinearVelocity);
        }

        if (isOwned == true)
        {
            UpdateMotorTorque();

            CmdUpdateWheelRpm(LeftWheelRpm, RightWheelRpm);

            CmdUpdateLinearVelocity(LinearVelocity);
        }
    }

    private void OnTrackTankDestroyed(Destructible arg0)
    {
        GameObject ruinedVisualModel = Instantiate(_destroedPref);

        ruinedVisualModel.transform.position = _visualModel.transform.position;
        ruinedVisualModel.transform.rotation = _visualModel.transform.rotation;
    }

    [Command]
    private void CmdUpdateLinearVelocity(float velocity)
    {
        SvUpdateLinearVelocity(velocity); 
    }

    [Server]
    private void SvUpdateLinearVelocity(float velocity)
    {
        syncLinearVelocity = velocity;
    }

    [Command]
    private void CmdUpdateWheelRpm(float leftRpm, float rightRpm)
    {
        SvUpdateWheelRpm(leftRpm,  rightRpm);
    }

    [Server]
    private void SvUpdateWheelRpm(float leftRpm, float rightRpm)
    {
        RpcUpdateWheelRpm(leftRpm, rightRpm);
    }

    [ClientRpc(includeOwner = false)]
    private void RpcUpdateWheelRpm(float leftRpm, float rightRpm)
    { 
        _leftWheelRow._minRpm = leftRpm;
        _rightWheelRow._minRpm = rightRpm;

        _leftWheelRow.UpdateMeshRotationByRpm(leftRpm);
        _rightWheelRow.UpdateMeshRotationByRpm(rightRpm);
    }

    private void UpdateMotorTorque()
    {
        float targetMotorTorque = TargetInputControl.z > 0 ? _maxForwardTorque * Mathf.RoundToInt(TargetInputControl.z) : _maxBackwardMotorTorque * Mathf.RoundToInt(TargetInputControl.z);
        float breakTorque = _breakTorque * TargetInputControl.y;
        float steering = TargetInputControl.x;

        //Update target motor torque
        if (targetMotorTorque > 0)
        {
            _currentMotorTorque = _forwardTorqueCurve.MoveTowards(Time.fixedDeltaTime) * targetMotorTorque;
        }

        if (targetMotorTorque < 0)
        {
            _currentMotorTorque = _backwardTorqueCurve.MoveTowards(Time.fixedDeltaTime) * targetMotorTorque;
        }

        if (targetMotorTorque == 0)
        {
            _currentMotorTorque = _forwardTorqueCurve.Reset();
            _currentMotorTorque = _backwardTorqueCurve.Reset();
        }

        //Break
        _leftWheelRow.Break(_breakTorque);
        _rightWheelRow.Break(_breakTorque);

        //Rolling
        if (targetMotorTorque == 0 && steering == 0)
        {
            _leftWheelRow.Break(_rollingResistance);
            _rightWheelRow.Break(_rollingResistance);
        }
        else
        {
            _leftWheelRow.Reset();
            _rightWheelRow.Reset();
        }


        //Rotate in place
        if (targetMotorTorque == 0 && steering != 0)
        {
            if (Mathf.Abs(_leftWheelRow._minRpm) < 1 || Mathf.Abs(_rightWheelRow._minRpm) < 1)
            {
                _leftWheelRow.SetTorque(_rotateTorqueInPlace);
                _rightWheelRow.SetTorque(_rotateTorqueInPlace);
            }
            else
            {
                if (steering < 0)
                {
                    _leftWheelRow.Break(_rotateBreakInPlace);
                    _rightWheelRow.SetTorque(_rotateTorqueInPlace);
                }

                if (steering > 0)
                {
                    _leftWheelRow.SetTorque(_rotateTorqueInPlace);
                    _rightWheelRow.Break(_rotateBreakInPlace);
                }
            }

            _leftWheelRow.SetSidewayStiffness(1.0f + _minSidewayStiffnessInPlace - Mathf.Abs(steering));
            _rightWheelRow.SetSidewayStiffness(1.0f + _minSidewayStiffnessInPlace - Mathf.Abs(steering));
        }


        //Move
        if (targetMotorTorque != 0)
        {
            if (steering == 0)
            {
                if (LinearVelocity < _maxLinearVelosity)
                {
                    _leftWheelRow.SetTorque(_currentMotorTorque);
                    _rightWheelRow.SetTorque(_currentMotorTorque);
                }
            }

            if (steering != 0 && (Mathf.Abs(_leftWheelRow._minRpm) < 1 || Mathf.Abs(_rightWheelRow._minRpm) < 1))
            {
                _leftWheelRow.SetTorque(_rotateTorqueInMotion * Mathf.Sign(_currentMotorTorque));
                _rightWheelRow.SetTorque(_rotateTorqueInMotion * Mathf.Sign(_currentMotorTorque));
            }
            else
            {
                if (steering < 0)
                {
                    _leftWheelRow.Break(_rotateBreakInMotion);
                    _rightWheelRow.SetTorque(_rotateTorqueInMotion * Mathf.Sign(_currentMotorTorque));
                }

                if (steering > 0)
                {                  
                    _rightWheelRow.Break(_rotateBreakInMotion);
                    _leftWheelRow.SetTorque(_rotateTorqueInMotion * Mathf.Sign(_currentMotorTorque));
                }
            }

            _leftWheelRow.SetSidewayStiffness(1.0f + _minSidewayStiffnessInMotion - Mathf.Abs(steering));
            _rightWheelRow.SetSidewayStiffness(1.0f + _minSidewayStiffnessInMotion - Mathf.Abs(steering));
        }


        _leftWheelRow.UpdateMeshTransform();
        _rightWheelRow.UpdateMeshTransform();
    }
}

