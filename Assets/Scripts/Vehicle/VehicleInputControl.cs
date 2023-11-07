using UnityEngine;

public class VehicleInputControl : MonoBehaviour
{
    public const float AIMDISTANCE = 1000;

    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }
    protected virtual void Update()
    {
        if(_player == null) return;
        if (_player.activeVehicle == null) return;

        if (_player.isOwned && _player.isLocalPlayer)
        {
            _player.activeVehicle.SetTargetControl(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical")));
            _player.activeVehicle.NetAimPoit = TraceAimPointWithoutPlayerVehicle(VehicleCamera.Instance.transform.position, VehicleCamera.Instance.transform.forward);

            if (Input.GetMouseButtonDown(0))
            {
               _player.activeVehicle.Fire();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _player.activeVehicle.Turret.SetSelectProjectile(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _player.activeVehicle.Turret.SetSelectProjectile(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _player.activeVehicle.Turret.SetSelectProjectile(2);
            }

        }      
    }

    public static Vector3 TraceAimPointWithoutPlayerVehicle(Vector3 start, Vector3 direction)
    { 
        Ray ray = new Ray(start, direction);

        RaycastHit[] hits = Physics.RaycastAll(ray, AIMDISTANCE);

        var t = Player.Local.activeVehicle;

        for (int i = hits.Length - 1; i >= 0; i--)
        {
            if (hits[i].collider.isTrigger == true)
                continue;

            if (hits[i].collider.transform.root.GetComponent<Vehicle>() == t)
                continue;

            return hits[i].point;

        }
        return ray.GetPoint(AIMDISTANCE);
    }
}
