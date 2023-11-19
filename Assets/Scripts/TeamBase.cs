using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TeamBase : MonoBehaviour
{
    public event UnityAction<bool, int> CaptureBase;

    [SerializeField] private float _captureLevel;
    [SerializeField] private float _captureAmmountPerVehicle;
    [SerializeField] private int _teamId;
    public int TeamIdBase => _teamId;

    public float CaptureLevel => _captureLevel;

    private List<Vehicle> _allVehicles = new List<Vehicle>();

    private bool _isCaptureBase;
    private void OnTriggerEnter(Collider other)
    {
       Vehicle v = other.transform.root.GetComponent<Vehicle>();

        if(v == null) return;

        if (v.HitPoint == 0) return;

        if(_allVehicles.Contains(v) == true) return;

        if(v.Owner.GetComponent<MatchMember>().TeamId == _teamId) return;

        v.HitPointChanged += OnHitPointChange;

        _allVehicles.Add(v);
        CaptureBaseAlarm();
    }

    private void OnTriggerExit(Collider other)
    {
        Vehicle v = other.transform.root.GetComponent<Vehicle>();

        if (v == null) return;

        v.HitPointChanged -= OnHitPointChange;
        _allVehicles.Remove(v);
        NoCaptureBaseAlarm();
    }

    private void Update()
    {
        if (NetworkSessionManager.Instance.IsServer == true)
        {
            bool isAllDead = true;

            for (int i = 0; i < _allVehicles.Count; i++)
            {
                if (_allVehicles[i].HitPoint != 0)
                {
                    isAllDead = false;

                    _captureLevel += _captureAmmountPerVehicle * Time.deltaTime;
                    _captureLevel = Mathf.Clamp(_captureLevel, 0,100);
                }
            }

            if (_allVehicles.Count == 0 || isAllDead == true)
            {
                _captureLevel = 0;
            }
        }
    }

    public void Reset()
    {
        _captureLevel = 0;

        if (_allVehicles.Count != 0)
        {
            for (int i = 0; i < _allVehicles.Count; i++)
            {
                _allVehicles[i].HitPointChanged -= OnHitPointChange;
            }
        }
        NoCaptureBaseAlarm();
        _allVehicles.Clear();
    }

    private void OnHitPointChange(int hitpoint)
    {
        _captureLevel = 0;
    }

    private void CaptureBaseAlarm()
    {
        if (_isCaptureBase == true) return;

        else
        {
            _isCaptureBase = true;
            CaptureBase?.Invoke(_isCaptureBase, _teamId);
        }      
    }

    private void NoCaptureBaseAlarm()
    {
        if (_isCaptureBase == false) return;

        else
        {
            _isCaptureBase = false;
            CaptureBase?.Invoke(_isCaptureBase, _teamId);
        }
    }
}
