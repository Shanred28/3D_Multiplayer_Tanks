using System.Collections.Generic;
using UnityEngine;

public class AIShooter : MonoBehaviour
{
    [SerializeField] private VehicleViewer _vehicleViewer;
    [SerializeField] private Transform _firePosition;
    [SerializeField] private float _findTargetUpdateRate;

    private Vehicle _vehicle;
    private Vehicle _target;
    private Transform _lookTransform;
    private float _timerFindTargetUpdate;

    public bool hasTarget =>  _target != null;

    private void Awake()
    {
        _vehicle = GetComponent<Vehicle>();
    }

    private void Update()
    {
        if (_findTargetUpdateRate > 0)
        {
            _timerFindTargetUpdate += Time.deltaTime;

            if (_timerFindTargetUpdate > _findTargetUpdateRate)
            {
                FindTarget();
                _timerFindTargetUpdate = 0;
            }
        }

        LookOnTarget();
        TryFire();
    }

    public void FindTarget()
    { 
         List<Vehicle> v = _vehicleViewer.GetAllVisableVehicle();

        float minDist = float.MaxValue;
        int index = -1;

        for (int i = 0; i < v.Count; i++)
        {
            if (v[i].HitPoint == 0) continue;
            if (v[i].TeamId == _vehicle.TeamId) continue;

            float dist = Vector3.Distance(transform.position, v[i].transform.position);

            if (dist < minDist)
            { 
                minDist = dist;
                index = i;
            }
        }

        if (index != -1)
        {
            _target = v[index];

            VehicleDimensions vehicleDimensions = _target.GetComponent<VehicleDimensions>();

            if (vehicleDimensions == null) return;

            _lookTransform = GetPriorityFirePoint(vehicleDimensions);
        }
        else
        { 
            _target = null;
            _lookTransform = null;
        }
    }

    private void LookOnTarget()
    {
        if (_lookTransform == null) 
        {
            _vehicle.NetAimPoit = _firePosition.position + _vehicle.transform.forward;
            return;
        }          
       _vehicle.NetAimPoit = _lookTransform.position;
    }

    private void TryFire()
    { 
         if(_target == null) return;

         RaycastHit hit;

        if (Physics.Raycast(_firePosition.position, _firePosition.forward, out hit, 1000))
        {
            if (hit.collider.transform.root == _target.transform.root)
            {
                _vehicle.Turret.SvFire();
            }
        }      
    }

    private Transform GetPriorityFirePoint(VehicleDimensions vehicleDimensions)
    {
        Transform[] firePoints = vehicleDimensions.GetFirePoint();
        List<Transform> targets = new List<Transform>();

        RaycastHit hit;

        for (int i = 0; i < firePoints.Length; i++)
        { 
            Vector3 dir = firePoints[i].position - _firePosition.position;
            if (Physics.Raycast(_firePosition.position, dir, out hit))
            {
                if (hit.collider.transform.root == _target.transform.root)
                {
                    targets.Add(firePoints[i]);
                }
            }
        }

        if (targets.Count > 0)
            return targets[Random.Range(0, targets.Count)];
        else
            return firePoints[0];
    }
}
