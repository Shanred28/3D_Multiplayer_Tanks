using UnityEngine;
using UnityEngine.AI;

public static class TransformExtensions
{
    public static Vector3 GetPositionZX(this Transform t)
    { 
        var x = t.position;
        x.y = 0;
        return x;
    }
}

public static class VectorExtensions
{
    public static Vector3 GetPositionZX(this Vector3 v)
    {
        var x = v;
        x.y = 0;
        return x;
    }
}

public class AIMovement : MonoBehaviour
{
    [SerializeField] private AIRaySensor _sensorForward;
    [SerializeField] private AIRaySensor _sensorBackward;
    [SerializeField] private AIRaySensor _sensorRight;
    [SerializeField] private AIRaySensor _sensorLeft;
    [SerializeField] private float _pathUpdateRate;
    [SerializeField] private float _stopDistance;


    private Vehicle _vehicle;
    private Vector3 _target;
    private NavMeshPath _path;
    private Vector3 _nextPathPoint;
    private int _cornerIndex;

    private bool _hasPath;
    private bool _rechedDistination;

    private float _timerUpdatePath;

    public bool HasPatch => _hasPath;
    public bool RestartDistination => _rechedDistination;

    private void Awake()
    {
        _path = new NavMeshPath();
        _vehicle = GetComponent<Vehicle>();
    }

    private void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Finish").transform.position;
    }

    private void Update()
    {
        if (_pathUpdateRate > 0)
        { 
            _timerUpdatePath += Time.deltaTime;

            if (_timerUpdatePath > _pathUpdateRate)
            {
                CalculatePath(_target);
                _timerUpdatePath = 0;
            }

        }

        UpdateTarget();

        MoveToTarget();
    }

    public void SetDestination(Vector3 target)
    {
        if (_target == target && _hasPath == true) return;

        _target = target;
        CalculatePath(target);
    }

    public void ResetPath()
    { 
        _hasPath = false;
        _rechedDistination = false;
    }

    private void UpdateTarget()
    { 
        if(_hasPath == false) return;

        _nextPathPoint = _path.corners[_cornerIndex];

        if (Vector3.Distance(transform.position, _nextPathPoint) < _stopDistance)
        {
            if (_path.corners.Length - 1 > _cornerIndex)
            {
                _cornerIndex++;
                _nextPathPoint = _path.corners[_cornerIndex];
            }
            else
            {
                _hasPath = false;
                _rechedDistination = true;
            }
        }

        for (int i = 0; i < _path.corners.Length - 1; i++)
            Debug.DrawLine(_path.corners[i], _path.corners[i + 1], Color.red);
    }

    private void CalculatePath(Vector3 target)
    {
        NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, _path);
        
        _hasPath = _path.corners.Length > 0;
        _rechedDistination = false;

        _cornerIndex = 1;
    }

    private void MoveToTarget()
    {
        if(_nextPathPoint == null) return;

        if (_rechedDistination == true)
        {
            _vehicle.SetTargetControl(new Vector3(0, 1, 0));
            return;
        }

        float turnControl = 0;
        float forwardThrust = 1;

        var refereenceDirection = GetReferenceMovementDirectionZX();
        var tankDir = GetTankDirectionZX();

        var forwardSensorState = _sensorForward.Raycast();
        var leftSensorState = _sensorLeft.Raycast();
        var rightSensorState = _sensorRight.Raycast();

        if (forwardSensorState.Item1 == true)
        {
            forwardThrust = 0;

            if (leftSensorState.Item1 == false)
            {
                turnControl = -1;
                forwardThrust = -0.2f;
            }
            else if (rightSensorState.Item1 == false)
            {
                turnControl = 1;
                forwardThrust = -0.2f;
            }
            else
            {
                forwardThrust = -1;
            }
        }
        else
        {
            turnControl = Mathf.Clamp(Vector3.SignedAngle(tankDir, refereenceDirection, Vector3.up), -45.0f, 45.0f) / 45.0f;

            float minSideDistance = 1;

            if (leftSensorState.Item1 && leftSensorState.Item2 < minSideDistance && turnControl < 0)
                turnControl = -turnControl;

            if (rightSensorState.Item1 && rightSensorState.Item2 < minSideDistance && turnControl > 0)
                turnControl = -turnControl;
        }
        
        _vehicle.SetTargetControl(new Vector3(turnControl,0,forwardThrust));
    }

    private Vector3 GetTankDirectionZX()
    { 
        var tankDir = _vehicle.transform.forward.GetPositionZX();

        tankDir.Normalize();
        return tankDir;
    }

    private Vector3 GetReferenceMovementDirectionZX()
    { 
        var tankPos = _vehicle.transform.GetPositionZX();
        var targetPos = _nextPathPoint.GetPositionZX();

        return (tankPos - tankPos).normalized;
    }
}
