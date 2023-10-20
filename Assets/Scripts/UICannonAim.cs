using UnityEngine;
using UnityEngine.UI;

public class UICannonAim : MonoBehaviour
{
    [SerializeField] private Image _aimImage;

    private Vector3 _aimPosition;

    private void Update()
    {
        if(Player.Local == null) return;
        if(Player.Local.activeVehicle == null) return;

        Vehicle v = Player.Local.activeVehicle;

        _aimPosition = VehicleInputControl.TraceAimPointWithoutPlayerVehicle(v.Turret.LaunchPoint.position, v.Turret.LaunchPoint.forward);

        Vector3 result = Camera.main.WorldToScreenPoint(_aimPosition);

        if (result.z > 0)
        {
            result.z = 0;

            _aimImage.transform.position = result;
        }

    }
}
