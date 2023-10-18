using UnityEngine;

public class VehicleInputControl : MonoBehaviour
{
    [SerializeField] private Vehicle _vehicle;

    protected virtual void Update()
    {
        _vehicle.SetTargetControl(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical")));
    }

}
