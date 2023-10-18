using UnityEngine;

public class VehicleCamera : MonoBehaviour
{
    [SerializeField] private Vehicle _vehicle;
    [SerializeField] private Vector3 _offset;

    [Header("Sensetive Limit")]
    [SerializeField] private float _rotateSensetive;
    [SerializeField] private float _scrollSensetive;

    [Header("RotationLimit")]
    [SerializeField] private float maxAngleLimitY;
    [SerializeField] private float minAngleLimitY;

    [Header("Distance")]
    [SerializeField] private float _distance;
    [SerializeField] private float _minDistance;
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _distanceOffsetFromCollisionHit;
    [SerializeField] private float _distanceLerpRate;

    [Header("Zoom Optics")]
    [SerializeField] private GameObject _zoomMaskEffect;
    private float _defaultFov;
    [SerializeField] private float _zoomedFov;
    [SerializeField] private float _zoomedMaxVerticalAngle;

    private Camera _camera;
    private Vector2 _rotateControl;

    private float _deltaRotationX;
    private float _deltaRotationY;
    private float _defaultMaxVerticalAngle;
    private float _currentDistance;
    private float _lastDistance;

    private bool isZoom;

    private void Start()
    {
        _camera = GetComponent<Camera>();

        _defaultFov = _camera.fieldOfView;
        _defaultMaxVerticalAngle = maxAngleLimitY;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        UpdateControl();
        _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);

        isZoom = _distance <= _minDistance;

        _deltaRotationX += _rotateControl.x * _rotateSensetive;
        _deltaRotationY += _rotateControl.y * _rotateSensetive;

        _deltaRotationY = ClampAngle(_deltaRotationY, minAngleLimitY, maxAngleLimitY);

        
        Quaternion finalRotation = Quaternion.Euler(-_deltaRotationY, _deltaRotationX, 0);
        Vector3 finalPosition = _vehicle.transform.position - (finalRotation * Vector3.forward * _distance);
        finalPosition = AddLocalOffset(finalPosition);

        //Calculate current distance
        float targetDistance = _distance;
        RaycastHit hit;

        if (Physics.Linecast(_vehicle.transform.position + new Vector3(0, _offset.y, 0), finalPosition, out hit) == true)
        { 
            float distanceToHit = Vector3.Distance(_vehicle.transform.position + new Vector3(0,_offset.y,0), hit.point);

            if (hit.transform != _vehicle)
            {
                if (distanceToHit < _distance)
                    targetDistance = distanceToHit - _distanceOffsetFromCollisionHit;
            }
        }

        _currentDistance = Mathf.MoveTowards(_currentDistance, targetDistance, Time.deltaTime * _distanceLerpRate);
        _currentDistance = Mathf.Clamp(_currentDistance, _minDistance, _distance);

        //Correct camera position
        finalPosition = _vehicle.transform.position - (finalRotation * Vector3.forward * _currentDistance);
        
        //Apply transform
        transform.rotation = finalRotation;
        transform.position = finalPosition;
        transform.position = AddLocalOffset(transform.position);

        //Zoom
        _zoomMaskEffect.SetActive(isZoom);
        if (isZoom == true)
        {
            transform.position = _vehicle.zoomOpticsPosition.position;
            _camera.fieldOfView = _zoomedFov;
            maxAngleLimitY = _zoomedMaxVerticalAngle;
        }
        else
        {
            _camera.fieldOfView = _defaultFov;
            maxAngleLimitY = _defaultMaxVerticalAngle;
        }
    }

    private void UpdateControl()
    {
        _rotateControl = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        _distance += -Input.mouseScrollDelta.y * _scrollSensetive;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        { 
            isZoom = !isZoom;

            if (isZoom == true)
            {
                _lastDistance = _distance;
                _distance = _minDistance;
            }
            else
            {
                _distance = _lastDistance;
                _currentDistance = _lastDistance;
            }
        }
    }

/*    public void ScrollDistanceCamera(float distanse)
    {
        _distance = Mathf.Clamp(_distance + distanse * _scrollSensetive, _minDistance, _maxDistance);
    }*/

    private Vector3 AddLocalOffset(Vector3 position)
    {
        Vector3 result = position;
        result += new Vector3(0, _offset.y, 0);
        result += transform.right * _offset.x;
        result += transform.forward * _offset.z;

        return result;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;

        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    public void SetTarget(Vehicle target)
    { 
      _vehicle = target;
    }
}
