using Mirror;
using UnityEngine;

public class Vehicle : Destructible
{
    [SerializeField] protected float _maxLinearVelosity;

    [Header("Engine Sound")]
    [SerializeField] private AudioSource _engineSound;
    [SerializeField] private AudioSource _trackSound;
    [SerializeField] private float _enginePithModifier;


    [Header("Vehicle")]
    [SerializeField] protected Transform _zoomOpticsPoint;
    public Transform zoomOpticsPosition => _zoomOpticsPoint;

    public virtual float LinearVelocity => 0;

    public Turret Turret;
    public int TeamId;

    [SyncVar]
    private Vector3 _netAimPoint;
    public Vector3 NetAimPoit
    { 
        get => _netAimPoint;

        set
        { 
            _netAimPoint = value;
            CmdSetNetAimPoint(value);
        }
    }


    [Command]
    private void CmdSetNetAimPoint(Vector3 v)
    { 
        _netAimPoint = v;
    }

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
           UpdateEngineSFX();
    }

    public void SetVisibile(bool visible)
    {
        if (visible == true)
            SetLayerToAll("Default");
        else
            SetLayerToAll("Ignore Main Camera");
    }

    private void SetLayerToAll(string layerName)
    { 
        gameObject.layer = LayerMask.NameToLayer(layerName);

        foreach (Transform t in transform.GetComponentsInChildren<Transform>()) 
        { 
            t.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }

    public void Fire()
    {
        Turret.Fire();
    }


    private void UpdateEngineSFX()
    {
        if (_engineSound != null)
        {
            _engineSound.pitch = 1.0f + NormalizedLinearVelocity * _enginePithModifier;
            _engineSound.volume = 0.8f + NormalizedLinearVelocity;

            if (LinearVelocity > 0.1f)
            {
                _trackSound.volume = 0.8f + NormalizedLinearVelocity;
                _trackSound.pitch = 1.0f + NormalizedLinearVelocity * _enginePithModifier;
            } 
            else 
            {
                _trackSound.volume = 0.0f;
            }
        }
    }

    [SyncVar(hook = "T")]
    public NetworkIdentity Owner;

    private void T(NetworkIdentity oldValue, NetworkIdentity newValue)
    {

    }
}
