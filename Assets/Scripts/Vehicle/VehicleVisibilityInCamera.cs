using System.Collections.Generic;
using UnityEngine;

public class VehicleVisibilityInCamera : MonoBehaviour
{
     private List<Vehicle> _vehicles = new List<Vehicle>();


    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Match.MatchStart -= OnMatchStart;
    }

    private void OnMatchStart()
    {
        _vehicles.Clear();

        Vehicle[] allVeh = FindObjectsOfType<Vehicle>();

        for (int i = 0; i < allVeh.Length; i++)
        {
            if (allVeh[i] == Player.Local.activeVehicle) continue;

            _vehicles.Add(allVeh[i]);
        }
    }

    private void Update()
    {
        for (int i = 0; i < _vehicles.Count; i++)
        {
            bool isVisable = Player.Local.activeVehicle.VehicleViewer.IsVisable(_vehicles[i].netIdentity);

            _vehicles[i].SetVisibile(isVisable);
        }
    }
}
