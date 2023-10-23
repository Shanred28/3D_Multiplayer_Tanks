using UnityEngine;
using UnityEngine.UI;

public class UICannonAim : MonoBehaviour
{

    [SerializeField] private Image _aimImage;

    [SerializeField] private Image _reloadSlider;

    [SerializeField] private float _AimMoveRate;

    private Vector3 _aimPosition;
    private Vector3 _scaleDefault;

    private void Start()
    {
        _scaleDefault = _aimImage.rectTransform.localScale;
    }

    public void LateUpdate()
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

        _aimImage.transform.position = Vector3.Lerp(_aimImage.transform.position, result, Time.deltaTime* _AimMoveRate) ;
        _aimImage.rectTransform.localScale = new Vector3(_scaleDefault.x + v.Turret.CurrentSpreadShootRange, _scaleDefault.y + v.Turret.CurrentSpreadShootRange, _scaleDefault.z + v.Turret.CurrentSpreadShootRange); 
            
    }
}
