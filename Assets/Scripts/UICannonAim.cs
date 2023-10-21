using UnityEngine;
using UnityEngine.UI;

public class UICannonAim : MonoBehaviour
{
    [SerializeField] private Image _aimImage;

    [SerializeField] private Image _reloadSlider;

    private Vector3 _aimPosition;

    private void Update()
    {
        if(Player.Local == null) return;
        if(Player.Local.activeVehicle == null) return;

        Vehicle v = Player.Local.activeVehicle;

        _reloadSlider.fillAmount = v.Turret.FireTimerNormalize;

        _aimPosition = VehicleInputControl.TraceAimPointWithoutPlayerVehicle(v.Turret.LaunchPoint.position, v.Turret.LaunchPoint.forward);

        Vector3 result = Camera.main.WorldToScreenPoint(_aimPosition);

        if (result.z > 0)
        {
            result.z = 0;
        }
        _aimImage.transform.position = result;
       //_aimImage.transform.position = Vector3.MoveTowards(_aimImage.transform.position, result, 500 * Time.deltaTime);
    }
}
