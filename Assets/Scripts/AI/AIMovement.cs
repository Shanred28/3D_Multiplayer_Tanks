using UnityEngine;

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


    private Vehicle _vehicle;
    private Vector3 _target;

    private void Awake()
    {
        _vehicle = GetComponent<Vehicle>();
    }

    private void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Finish").transform.position;
    }

    private void Update()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
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
        var targetPos = _target.GetPositionZX();

        return (tankPos - tankPos).normalized;
    }
}
