using UnityEngine;

[System.Serializable]
public class WheelSyncPoint
{
    public Transform bone;
    public Transform mesh;
    [HideInInspector] public Vector3 offset;
}
public class TrackSyncByWheel : MonoBehaviour
{
    [SerializeField] private WheelSyncPoint[] _syncpoints;

    private void Start()
    {
        for (int i = 0; i < _syncpoints.Length; i++)
        {
            _syncpoints[i].offset = new Vector3(_syncpoints[i].bone.localPosition.x, _syncpoints[i].bone.localPosition.y, _syncpoints[i].bone.localPosition.z)  - new Vector3( _syncpoints[i].mesh.localPosition.x, _syncpoints[i].mesh.localPosition.z, _syncpoints[i].mesh.localPosition.y);
        }
    }

    private void Update()
    {
        for (int i = 0; i < _syncpoints.Length; i++)
        {
            _syncpoints[i].bone.localPosition = new Vector3(_syncpoints[i].mesh.localPosition.x, _syncpoints[i].mesh.localPosition.z, _syncpoints[i].mesh.localPosition.y) + _syncpoints[i].offset;
        }
    }
}
