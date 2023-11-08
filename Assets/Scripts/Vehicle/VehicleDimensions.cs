using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class VehicleDimensions : MonoBehaviour
{
    [SerializeField] private Transform[] _points;

    private Vehicle _vehicle;
    public Vehicle Vehicle => _vehicle;

    private void Awake()
    {
        _vehicle = GetComponent<Vehicle>();
    }

    public bool IsVisableFromPoint(Transform source, Vector3 point, Color color)
    {

        bool isVisable = true;

        for (int i = 0; i < _points.Length; i++)
        {
            Debug.DrawLine(point, _points[i].position,color);

            RaycastHit[] hits = Physics.RaycastAll(point, (_points[i].position - point).normalized, Vector3.Distance(point, _points[i].position));

            isVisable = true;

            for (int j = 0; j < hits.Length; j++)
            {
                if (hits[j].collider.transform.root == source) continue;
                if(hits[j].collider.transform.root == transform.root) continue;

                isVisable = false;
            }
            if (isVisable == true)
                return isVisable;
        }

        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(_points == null) return;

        Gizmos.color = Color.blue;
        for (int i = 0; i < _points.Length; i++)
            Gizmos.DrawSphere(_points[i].position, 0.2f);
    }
#endif
}
