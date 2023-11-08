using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class VehicleDimensions : MonoBehaviour
{
    private const float DIST_BARRIER_BLOCK_VISABLE = 100.0f;

    [SerializeField] private Transform[] _points;

    private Vehicle _vehicle;
    public Vehicle Vehicle => _vehicle;

    RaycastHit[] hits = new RaycastHit[10];

    private void Awake()
    {
        _vehicle = GetComponent<Vehicle>();
    }

    public bool IsVisableFromPoint(Transform source, Vector3 point, Color color)
    {

        bool isVisable = true;

        for (int i = 0; i < _points.Length; i++)
        {
            //Debug
            Debug.DrawLine(point, _points[i].position,color);  

            int l = Physics.RaycastNonAlloc(point, (_points[i].position - point).normalized, hits, Vector3.Distance(point, _points[i].position));

            isVisable = true;

            for (int j = 0; j < l; j++)
            {
                if (hits[j].collider.transform.root == source) continue;
                if(hits[j].collider.transform.root == transform.root) continue;
                if (hits[j].collider.TryGetComponent(out PartialBarrier barrier) && DIST_BARRIER_BLOCK_VISABLE >=  Vector3.Distance(point, barrier.transform.position)) continue;

                    isVisable = false;
            }
            if (isVisable == true)
                return isVisable;
        }

        return false;
    }

    public float DistanseToPartialBarrier(Transform source, Vector3 point)
    {
        float distBarrier = 0;
        for (int i = 0; i < _points.Length; i++)
        {
            int l = Physics.RaycastNonAlloc(point, (_points[i].position - point).normalized, hits, Vector3.Distance(point, _points[i].position));
            for (int j = 0; j < l; j++)
            {
                if (hits[j].collider.transform.root == source) continue;
                if (hits[j].collider.transform.root == transform.root) continue;

                if (hits[j].collider.TryGetComponent(out PartialBarrier barrier))
                {
                    var dist = Vector3.Distance(source.position, barrier.transform.position);
                    if(distBarrier < dist)
                        distBarrier = dist;
                }
            }
        }

         return distBarrier;
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
