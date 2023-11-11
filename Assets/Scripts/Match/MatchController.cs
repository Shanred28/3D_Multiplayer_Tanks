using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public interface IMatchCondition
{ 
    bool IsTriggered { get; }

    void OnServerMatchStart(MatchController controller);
    void OnServerMatchEnd(MatchController controller);
}

public class MatchController : NetworkBehaviour
{
    private static int TeamIdCounter;

    public static int GetNextTeam()
    { 
        return TeamIdCounter++ % 2;
    }
    public static void ResetTeamCounter()
    {
        TeamIdCounter = 1;
    }

    public event UnityAction MatchStart;
    public event UnityAction MatchEnd;

    public event UnityAction SvMatchStart;
    public event UnityAction SvMatchEnd;

    [SerializeField] private MatchMemberSpawner _spawner;
    [SerializeField] private float _delayAfterSpawnBeforeStartMatch = 0.5f;

    public int WinTeamId = -1;

    [SyncVar]
    private bool _matchActive;
    public bool IsMatchActive => _matchActive;

    private IMatchCondition[] _matchConditions;

    private void Awake()
    {
        _matchConditions = GetComponentsInChildren<IMatchCondition>();
    }

    private void Update()
    {
        if (isServer == true)
        {
            if (_matchActive == true)
            {
                foreach (IMatchCondition condition in _matchConditions)
                {
                    if (condition.IsTriggered == true)
                    {
                        SvEndMatch();
                        break;
                    }
                }
            }
        }
    }

    [Server]
    public void SvRestartMatch()
    {
        if (_matchActive == true) return;

        _matchActive = true;

        _spawner.SvRespawnVehiclesAllMembers();

        StartCoroutine(StartEventMatchWitchDelay(_delayAfterSpawnBeforeStartMatch));
    }

    private IEnumerator StartEventMatchWitchDelay(float delay)
    { 
        yield return new WaitForSeconds(delay);

        foreach (IMatchCondition condition in _matchConditions)
        {
            condition.OnServerMatchStart(this);
        }

        SvMatchStart?.Invoke();

        RpcMatchStart();
    }

    [Server]
    public void SvEndMatch() 
    {
        foreach (IMatchCondition condition in _matchConditions)
        {
            condition.OnServerMatchEnd(this);

/*            if (condition is MatchTimer)
            {
                WinTeamId = -1;
            }*/

            if (condition is ConditionTeamDeathmath && condition.IsTriggered == true)
            {
                WinTeamId = (condition as ConditionTeamDeathmath).WinTeamId;
            }

            if (condition is ConditionCaptureBase && condition.IsTriggered == true)
            {
                if ((condition as ConditionCaptureBase).RedBaseCaptureLevel == 100)
                {
                    WinTeamId = TeamSide.TeamBlue;
                }

                if ((condition as ConditionCaptureBase).BlueBaseCaptureLevel == 100)
                {
                    WinTeamId = TeamSide.TeamRed;
                }
            }
        }

        _matchActive =false;

        SvMatchEnd?.Invoke();

        RpcMatchEnd(WinTeamId);
    }

    [ClientRpc]
    private void RpcMatchStart()
    { 
         MatchStart?.Invoke();
    }

    [ClientRpc]
    private void RpcMatchEnd(int winTeamId)
    {
        WinTeamId = winTeamId;
        MatchEnd?.Invoke();
    }
}
