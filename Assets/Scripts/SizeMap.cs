using UnityEngine;

public class SizeMap : MonoBehaviour
{
    [SerializeField] private Vector2 _size;
    public Vector2 Size { get { return _size; } }

    public Vector3 GetNormPos(Vector3 pos)
    {
        return new Vector3(pos.x / _size.x, 0, pos.z / _size.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(_size.x, 0, _size.y));
    }
}
