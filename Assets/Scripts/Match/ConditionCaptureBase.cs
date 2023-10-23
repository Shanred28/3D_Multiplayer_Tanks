using UnityEngine;
using Mirror;

public class ConditionCaptureBase : NetworkBehaviour, IMatchCondition
{
    [SerializeField] private TeamBase _redBase;
    [SerializeField] private TeamBase _blueBase;

    [SyncVar]
    private float _redBaseCaptureLevel;
    public float RedBaseCaptureLevel => _redBaseCaptureLevel;

    [SyncVar]
    private float _blueBaseCaptureLevel;
    public float BlueBaseCaptureLevel => _blueBaseCaptureLevel;

    private bool _isTriggered;

    bool IMatchCondition.IsTriggered => _isTriggered;

    public void OnServerMatchStart(MatchController controller)
    {
        Reset();
    }

    public void OnServerMatchEnd(MatchController controller)
    {
        enabled = false; 
    }

    private void Start()
    {
        enabled = false; 
    }

    private void Update() 
    {
        if (isServer == true)
        {
            _redBaseCaptureLevel = _redBase.CaptureLevel;
            _blueBaseCaptureLevel = _blueBase.CaptureLevel;

            if (_redBaseCaptureLevel == 100 || _blueBaseCaptureLevel == 100)
            { 
                _isTriggered = true;
            }
        }
    
    }

    private void Reset()
    {
        _redBase.Reset();
        _blueBase.Reset();

        _redBaseCaptureLevel = 0;
        _blueBaseCaptureLevel = 0;

        enabled = true;
        _isTriggered = false;
    }
}
