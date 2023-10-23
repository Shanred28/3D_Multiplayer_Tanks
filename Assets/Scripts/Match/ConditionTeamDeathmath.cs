using UnityEngine;

public class ConditionTeamDeathmath : MonoBehaviour, IMatchCondition
{
    private int _red;
    private int _blue;

    private int _winTeamId;
    public int WinTeamId => _winTeamId;

    private bool _isTriggeret;

    bool IMatchCondition.IsTriggered => _isTriggeret;

    public void OnServerMatchStart(MatchController controller)
    {
        Reset();

        foreach (var v in FindObjectsOfType<Player>())
        {
            if (v.activeVehicle != null)
            {
                v.activeVehicle.OnEventDeadth.AddListener( EventDeeathHandler);
                if (v.TeamId == TeamSide.TeamRed)
                    _red++;
                else if(v.TeamId == TeamSide.TeamBlue)
                    _blue++;
            }
        }
    }

    public void OnServerMatchEnd(MatchController controller)
    {
        
    }

    private void EventDeeathHandler(Destructible des)
    { 
        var ownerPlayer = des.Owner?.GetComponent<Player>();

        if (ownerPlayer == null) return;

        switch (ownerPlayer.TeamId)
        { 
            case TeamSide.TeamRed:
                _red--;
                break;

            case TeamSide.TeamBlue:
                _blue--;
                break;
        }

        if (_red == 0)
        {
            _winTeamId = 1;
            _isTriggeret = true;
        }
        else if (_blue == 0)
        {
            _winTeamId = 0;
            _isTriggeret = true;
        }
    }

    private void Reset()
    {
        _red = 0;
        _blue = 0;
        _isTriggeret = false;
    }
}
