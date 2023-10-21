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
        }      
    }

    public static Vector3 TraceAimPointWithoutPlayerVehicle(Vector3 start, Vector3 direction)
    { 
        Ray ray = new Ray(start, direction);

        RaycastHit[] hits = Physics.RaycastAll(ray, AIMDISTANCE);

        var m = Player.Local.activeVehicle.GetComponent<Rigidbody>();

        foreach (var hit in hits)
        {
            if (hit.rigidbody == m)
                continue;

            return hit.point;        
        }
        return ray.GetPoint(AIMDISTANCE);
    }
}
