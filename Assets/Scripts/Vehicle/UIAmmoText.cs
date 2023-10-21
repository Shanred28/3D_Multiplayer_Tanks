using TMPro;
using UnityEngine;

public class UIAmmoText : MonoBehaviour
{
    [SerializeField] private TMP_Text _ammoCountText;

    private Turret _turret;

    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;
        if(_turret != null)
           _turret.AmmoChanged -= OnAmmoChenger;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        _turret = vehicle.Turret;
        _turret.AmmoChanged += OnAmmoChenger;

        _ammoCountText.text = _turret.AmmoCount.ToString();
    }
    private void OnAmmoChenger(int ammo)
    {
        _ammoCountText.text = ammo.ToString();
    }
}
