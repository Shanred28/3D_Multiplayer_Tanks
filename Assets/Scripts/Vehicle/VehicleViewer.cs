using UnityEngine;
using Mirror;
using System.Collections.Generic;

[RequireComponent(typeof(Vehicle))]
public class VehicleViewer : NetworkBehaviour
{
    private const float UPDATE_INTERVAL = 0.33f;
    private const float  X_RAY_DISTANCE = 50.0f;
    private const float  BASE_EXIT_TIME_FROM_DISCOVERY = 10.0f;
    private const float CAMOUFLAGE_DISTANCE = 150.0f;

    [SerializeField] private float _viewDistance;
    [SerializeField] private Transform[] _viewPoints;
    [SerializeField] private Color _color;

    private List<VehicleDimensions> _allVehicleDimensions = new List<VehicleDimensions>();
    private SyncList<NetworkIdentity> _visableVehicles = new SyncList<NetworkIdentity>();

    public List<float> _remainingTime = new List<float>();

    private Vehicle _vehicle;
    private float _remaningTimeLastUpdate;

    public override void OnStartServer()
    {
        base.OnStartServer();

        _vehicle = GetComponent<Vehicle>();

        NetworkSessionManager.Match.SvMatchStart += OnSvMatchStart;
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        NetworkSessionManager.Match.SvMatchStart -= OnSvMatchStart;
    }

    private void Update()
    {
        if (isServer == false) return;

        _remaningTimeLastUpdate += Time.deltaTime;

        if (_remaningTimeLastUpdate >= UPDATE_INTERVAL)
        {
            for (int i = 0; i < _allVehicleDimensions.Count; i++)
            {
                if (_allVehicleDimensions[i].Vehicle == null) continue;

                bool isVisable = true;

                for (int j = 0; j < _viewPoints.Length; j++)
                {

                    isVisable = CheckVisibility(_viewPoints[j].position, _allVehicleDimensions[i], i);
                    if (isVisable == true) break;
                }

                if (isVisable == true && _visableVehicles.Contains(_allVehicleDimensions[i].Vehicle.netIdentity) == false)
                {
                    _visableVehicles.Add(_allVehicleDimensions[i].Vehicle.netIdentity);
                    _remainingTime.Add(-1);
                }

                if (isVisable == true && _visableVehicles.Contains(_allVehicleDimensions[i].Vehicle.netIdentity) == true)
                {
                    _remainingTime[_visableVehicles.IndexOf(_allVehicleDimensions[i].Vehicle.netIdentity)] = -1;
                }


                if (isVisable == false && _visableVehicles.Contains(_allVehicleDimensions[i].Vehicle.netIdentity) == true)
                {
                    if (_remainingTime[_visableVehicles.IndexOf(_allVehicleDimensions[i].Vehicle.netIdentity)] == -1)
                        _remainingTime[_visableVehicles.IndexOf(_allVehicleDimensions[i].Vehicle.netIdentity)] = BASE_EXIT_TIME_FROM_DISCOVERY;

                }
            }
            _remaningTimeLastUpdate = 0;
        }
            for (int i = 0; i < _remainingTime.Count; i++)
            {
                if (_remainingTime[i] > 0)
                {
                    _remainingTime[i] -= Time.deltaTime;

                    if (_remainingTime[i] <= 0)
                        _remainingTime[i] = 0;
                }

                if (_remainingTime[i] == 0)
                {
                    _remainingTime.RemoveAt(i);
                    _visableVehicles.RemoveAt(i);
                }
            }                  
    }

    public bool IsVisable(NetworkIdentity identity)
    { 
        return _visableVehicles.Contains(identity);
    }

    public List<Vehicle> GetAllVehicle()
    { 
        List<Vehicle> av = new List<Vehicle>(_allVehicleDimensions.Count);

        for (int i = 0; i < _allVehicleDimensions.Count; i++)
        {
            av.Add(_allVehicleDimensions[i].Vehicle);
        }
        return av;
    }

    public List<Vehicle> GetAllVisableVehicle()
    {
        List<Vehicle> av = new List<Vehicle>(_allVehicleDimensions.Count);

        for (int i = 0; i < _visableVehicles.Count; i++)
        {
            av.Add(_visableVehicles[i].GetComponent<Vehicle>());
        }
        return av;
    }

    private void OnSvMatchStart()
    {
        _color = Random.ColorHSV();

        Vehicle[] allVeh = FindObjectsOfType<Vehicle>();

        for (int i = 0; i < allVeh.Length; i++)
        {
            if (_vehicle == allVeh[i]) continue;
            VehicleDimensions vd = allVeh[i].GetComponent<VehicleDimensions>();

            if (vd == null) continue;

            if (_vehicle.TeamId != allVeh[i].TeamId)
                _allVehicleDimensions.Add(vd);
            else
            {
                _visableVehicles.Add(vd.Vehicle.netIdentity);
                _remainingTime.Add(-1);
            }               
        }
    }

    private bool CheckVisibility(Vector3 viewPoint, VehicleDimensions vehicleDimensions, int index)
    {
        if (Vector3.Distance(_vehicle.transform.position, _allVehicleDimensions[index].transform.position) <= X_RAY_DISTANCE) return true;

        float distance = Vector3.Distance(transform.position, vehicleDimensions.transform.position);

        if(distance > _viewDistance) return false;

        float currentViewDist = _viewDistance;

        if (distance >= CAMOUFLAGE_DISTANCE)
        {
            VehicleCamouflage vehicleCamouflage = vehicleDimensions.Vehicle.GetComponent<VehicleCamouflage>();

            if (vehicleCamouflage != null)
            {
               var distBarr = vehicleDimensions.DistanseToPartialBarrier(transform.root, viewPoint);
                currentViewDist = _viewDistance - vehicleCamouflage.CurrentDistance + distBarr /2;
            }              
        }

        if(distance > currentViewDist) return false;       

        return vehicleDimensions.IsVisableFromPoint(transform.root, viewPoint, _color);
    }
}
