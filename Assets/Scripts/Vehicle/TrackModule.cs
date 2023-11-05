using UnityEngine;
using Mirror;
using System;

[RequireComponent(typeof(TrackTank))]
public class TrackModule : NetworkBehaviour
{
    [Header("Visual")]
    [SerializeField] private GameObject _leftTrackMesh;
    [SerializeField] private GameObject _leftTrackRuinedMesh;
    [SerializeField] private GameObject _rightTrackMesh;
    [SerializeField] private GameObject _rightTrackRuinedMesh;

    [Space(5)]
    [SerializeField] private VehicleModule _leftTrack;
    [SerializeField] private VehicleModule _rightTrack;

    private TrackTank _trackTank;

    private void Start()
    {
        _trackTank = GetComponent<TrackTank>();

        _leftTrack.Destroyed += OnLeftTrackDestroed;
        _rightTrack.Destroyed += OnRightTrackDestroed;

        _leftTrack.Recovered += OnLeftTrackRecovered;
        _rightTrack.Recovered += OnRightTrackRecovered;
    }

    private void OnDestroy()
    {
        _leftTrack.Destroyed -= OnLeftTrackDestroed;
        _rightTrack.Destroyed -= OnRightTrackDestroed;

        _leftTrack.Recovered -= OnLeftTrackRecovered;
        _leftTrack.Recovered -= OnRightTrackRecovered;
    }

    private void OnLeftTrackDestroed(Destructible arg0)
    {
        ChangeActiveObjects(_leftTrackMesh, _leftTrackRuinedMesh);
        TakeAwayMibility();
    }

    private void OnLeftTrackRecovered(Destructible arg0)
    {
        ChangeActiveObjects(_leftTrackMesh, _leftTrackRuinedMesh);

        if (_rightTrack.HitPoint > 0)
            RegainMibility();
    }

    private void OnRightTrackDestroed(Destructible arg0)
    {
        ChangeActiveObjects(_rightTrackMesh, _rightTrackRuinedMesh);
        TakeAwayMibility();
    }

    private void OnRightTrackRecovered(Destructible arg0)
    {
        ChangeActiveObjects(_rightTrackMesh, _rightTrackRuinedMesh);

        if(_leftTrack.HitPoint > 0)
             RegainMibility();
    }

    private void ChangeActiveObjects(GameObject a, GameObject b)
    {
        a.SetActive(b.activeSelf);
        b.SetActive(!b.activeSelf);
    }

    private void TakeAwayMibility()
    { 
        _trackTank.enabled = false;
    }

    private void RegainMibility()
    {
        _trackTank.enabled = true;
    }
}
