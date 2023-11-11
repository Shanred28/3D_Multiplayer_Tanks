using UnityEngine;

public class AIRaySensor : MonoBehaviour
{
    [SerializeField] private Transform[] _rays;
    [SerializeField] private float _raycastDistance;
    public float RaycastDistance => _raycastDistance;

    public (bool, float) Raycast()
    {
        float dist = -1;

        foreach (var v in _rays)
        {
            RaycastHit hit;
            if (Physics.Raycast(v.position, v.forward, out hit, _raycastDistance))
            {
                if (dist < 0 || hit.distance < dist)
                { 
                    dist = hit.distance;
                }
            }
        }

        return (dist > 0, dist);
    }

    private void OnDrawGizmos()
    {
        foreach (var v in _rays)
        { 
            Gizmos.color = Color.red;
            Gizmos.DrawLine(v.position, v.position + v.forward * _raycastDistance);
        }
    }
}
