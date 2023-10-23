using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAmmoText : MonoBehaviour
{
    [SerializeField] private TMP_Text _ammoACountText;
    [SerializeField] private TMP_Text _ammoBCountText;
    [SerializeField] private Image _ImageA;
    [SerializeField] private Image _ImageB;

    private Turret _turret;

    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
    }

    private void OnDestroy()
    {
        //NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;
        if(_turret != null)
           _turret.AmmoChanged -= OnAmmoChenger;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        _turret = vehicle.Turret;
        _turret.AmmoChanged += OnAmmoChenger;

        _ammoACountText.text = _turret.AmmoCountA.ToString();
        _ammoBCountText.text = _turret.AmmoCountB.ToString();
        _ImageB.enabled = false;
    }
    private void OnAmmoChenger(int ammo,int type)
    {
        if (type == 0)
        {
            _ImageA.enabled = true;
            _ImageB.enabled = false;
            _ammoACountText.text = ammo.ToString();
        }
            
        else if(type == 1)
        {
            _ImageA.enabled = false;
            _ImageB.enabled = true;
            _ammoBCountText.text = ammo.ToString();
        }
            
    }
}
