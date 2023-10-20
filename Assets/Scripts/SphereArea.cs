using UnityEngine;

public class SphereArea : MonoBehaviour
{
    [SerializeField] private float _radius;

    [SerializeField] private Color _color = Color.green;

    public Vector3 RandomInside
    {
        get
        { 
            var pos = Random.insideUnitSphere * _radius + transform.position;

            pos.y = transform.position.y;

            return pos;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.DrawSphere(transform.position, _radius);
    }
}
