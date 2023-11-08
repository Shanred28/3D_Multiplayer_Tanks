using UnityEngine;

public class TestDimension : MonoBehaviour
{
    [SerializeField] private VehicleDimensions _vehicleDimensions;

    private void Update()
    {
        Debug.Log(_vehicleDimensions.IsVisableFromPoint(transform.root,transform.position, Color.red));
    }
}
