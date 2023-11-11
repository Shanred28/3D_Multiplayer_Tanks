using UnityEngine;
using Mirror;

public class MatchTimer : NetworkBehaviour, IMatchCondition
{
    [SerializeField] private float _matchTimer;

    [SyncVar]
    private float _timeLeft;
    public float TimeLeft => _timeLeft;

    private bool _isTimerEnd = false;
    
    bool IMatchCondition.IsTriggered => _isTimerEnd;

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
        _timeLeft = _matchTimer;

        if (isServer == true)
        { 
            enabled = false;
        }
    }

    private void Update()
    {
        if (isServer == true)
        {
            _timeLeft -= Time.deltaTime;

            if (_timeLeft <= 0)
            { 
                _timeLeft = 0;

                _isTimerEnd = true;
            }
        }
    }

    private void Reset()
    {
        enabled = true;
        _timeLeft = _matchTimer;
        _isTimerEnd = false;
    }
}
