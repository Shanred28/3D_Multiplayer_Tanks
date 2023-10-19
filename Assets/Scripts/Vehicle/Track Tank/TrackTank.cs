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
}
public class TrackTank : Vehicle
{
    public override float LinearVelocity => _rb.velocity.magnitude;

    [SerializeField] private Transform _ceenterOfMass;

    [Header("Tracks")]   
    [SerializeField] private TrackWheelRow _leftWheelRow;
    [SerializeField] private TrackWheelRow _rightWheelRow;

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
    public float LRightWheelRpm => _rightWheelRow._minRpm;

    private Rigidbody _rb;
    private float _currentMotorTorque;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = _ceenterOfMass.localPosition;
    }

    private void FixedUpdate()
    {
        float targetMotorTorque =  TargetInputControl.z > 0? _maxForwardTorque * Mathf.RoundToInt(TargetInputControl.z) : _maxBackwardMotorTorque * Mathf.RoundToInt(TargetInputControl.z);
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
                _leftWheelRow.SetTorque(_rotateTorqueInMotion);
                _rightWheelRow.SetTorque(_rotateTorqueInMotion);
            }
            else
            {
                if (steering < 0)
                {
                    _leftWheelRow.Break(_rotateBreakInMotion);
                    _rightWheelRow.SetTorque(_rotateTorqueInMotion);
                }

                if (steering > 0)
                {
                    _leftWheelRow.SetTorque(_rotateTorqueInMotion);
                    _rightWheelRow.Break(_rotateBreakInMotion);
                }
            }

            _leftWheelRow.SetSidewayStiffness(1.0f + _minSidewayStiffnessInMotion - Mathf.Abs(steering));
            _rightWheelRow.SetSidewayStiffness(1.0f + _minSidewayStiffnessInMotion - Mathf.Abs(steering));
        }


        _leftWheelRow.UpdateMeshTransform();
        _rightWheelRow.UpdateMeshTransform();
    }
}
