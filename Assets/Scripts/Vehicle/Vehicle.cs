using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField] protected float _maxLinearVelosity;

    [Header("Engine Sound")]
    [SerializeField] private AudioSource _engineSound;
    [SerializeField] private float _enginePithModifier;


    [Header("Vehicle")]
    [SerializeField] protected Transform _zoomOpticsPoint;
    public Transform zoomOpticsPosition => _zoomOpticsPoint;

    public virtual float LinearVelocity => 0;

    public float NormalizedLinearVelocity
    {
        get 
        {
            if (Mathf.Approximately(0, LinearVelocity) == true) return 0;

            return Mathf.Clamp01(LinearVelocity / _maxLinearVelosity);
        }
    }

    protected Vector3 TargetInputControl;

    public void SetTargetControl(Vector3 control)
    {
        TargetInputControl = control.normalized;
    }  

    protected virtual void Update()
    {
        if(isStartDrive == true)
           UpdateEngineSFX();
    }

    private void UpdateEngineSFX()
    {
        if (_engineSound != null)
        {
            _engineSound.pitch = 1.0f + NormalizedLinearVelocity * _enginePithModifier;
            _engineSound.volume = 0.5f + NormalizedLinearVelocity;
        }
    }

    private bool isStartDrive;
    public virtual void OnStartDrive()
    {
        isStartDrive = true;
        _engineSound.Play();
    }

    public virtual void OffStartDrive()
    {
        isStartDrive = false;
        _engineSound.Stop();
    }
}
