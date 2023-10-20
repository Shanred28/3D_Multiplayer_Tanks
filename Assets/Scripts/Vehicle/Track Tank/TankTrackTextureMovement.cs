using UnityEngine;

public class TankTrackTextureMovement : MonoBehaviour
{
    private TrackTank _trackTank;

    [SerializeField] private SkinnedMeshRenderer _leftTrackRenderer;
    [SerializeField] private SkinnedMeshRenderer _rightTrackRenderer;

    [SerializeField] private Vector2 _direction;
    [SerializeField] private float _modefeir;

    private void Start()
    {
        _trackTank = GetComponent<TrackTank>();
    }

    private void FixedUpdate()
    {
       
        float speed = _trackTank.LeftWheelRpm / 60.0f * _modefeir * Time.fixedDeltaTime;
        _leftTrackRenderer.material.SetTextureOffset("_BaseMap", _leftTrackRenderer.material.GetTextureOffset("_BaseMap") + _direction * speed);

        speed = _trackTank.RightWheelRpm/ 60.0f * _modefeir * Time.fixedDeltaTime;
        _rightTrackRenderer.material.SetTextureOffset("_BaseMap", _rightTrackRenderer.material.GetTextureOffset("_BaseMap") + _direction * speed);
    }
}
