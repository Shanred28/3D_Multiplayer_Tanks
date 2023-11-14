using UnityEngine;
using Mirror;

public enum AIBehaviourType
{ 
    Patrol,
    Support,
    InvaderBase,
    DefendBase
}

public class AITank : NetworkBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float _patrolChance;
    [Range(0, 1)]
    [SerializeField] private float _supportChance;
    [Range(0, 1)]
    [SerializeField] private float _invaderChance;

    [SerializeField] private Vehicle _vehicle;
    [SerializeField] private AIMovement _movement;
    [SerializeField] private AIShooter _shooter;

    private AIBehaviourType _behaviourType;
    public AIBehaviourType behaviourType => _behaviourType;

    //private Vehicle _targetFire;
    private TeamBase _teamBase;
    private Vector3 _movementTarget;

    private int _startCountTeamMember;
    private int _countTeamMember;
    private int _defBaseTank;

    private bool _isCaptureTeamBase;
    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
        
        _movement.enabled = false;
        _shooter.enabled = false;
        _teamBase = AIPath.Instance.GetMyBase(_vehicle.TeamId);
        _teamBase.CaptureBase += OnCaptureBase;


        CalcTeamMember();
        SetStartBehaviour();
    }

    private void Update()
    {
        if (isServer == true)
        {
            UpdateBehaviour();
        }
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Match.MatchStart -= OnMatchStart;
        _vehicle.Destroyed -= OnVehicleDestoyed;
    }

    private void OnMatchStart()
    {
        _movement.enabled = true;
        _shooter.enabled = true;
    }

    private void OnVehicleDestoyed(Destructible arg0)
    {
        _movement.enabled = false;
        _shooter.enabled = false;
        _vehicle.enabled = false;
    }

    private void SetStartBehaviour()
    {
        float chance = Random.Range(0.0f, _patrolChance + _supportChance + _invaderChance);

        if (chance >= 0.0f && chance <= _patrolChance)
        {
            StartBehaviour(AIBehaviourType.Patrol);
            return;
        }

        if (chance >= _patrolChance && chance <= _patrolChance + _supportChance)
        {
            StartBehaviour(AIBehaviourType.Support);
            return;
        }

        if (chance >= _patrolChance + _supportChance && chance <= _patrolChance + _supportChance + _invaderChance)
        {
            StartBehaviour(AIBehaviourType.InvaderBase);
            return;
        }
    }

    private void CalcTeamMember()
    {
        Vehicle[] v = FindObjectsOfType<Vehicle>();

        for (int i = 0; i < v.Length; i++)
        {
            if (v[i].TeamId == _vehicle.TeamId)
            {            
                if (v[i] != _vehicle)
                {
                    _startCountTeamMember++;
                    v[i].Destroyed += OnTeamMemberDestroyed;
                }
            }
        }

        _countTeamMember = _startCountTeamMember;
    }

    #region Behaviour

    private void StartBehaviour(AIBehaviourType type)
    {
        _behaviourType = type;
                 
        if (_behaviourType == AIBehaviourType.InvaderBase)
        {
            _movementTarget = AIPath.Instance.GetBasePoint(_vehicle.TeamId);
        }

        if (_behaviourType == AIBehaviourType.Patrol)
        {
            _movementTarget = AIPath.Instance.GetRandomPatrolPoint();
        }

        if (_behaviourType == AIBehaviourType.Support && _startCountTeamMember > 2)
        {
            _movementTarget = AIPath.Instance.GetRandomFirePoint(_vehicle.TeamId);
        }
     
        _movement.ResetPath();
    }
    private void OnCaptureBase(bool isCapture, int teamId)
    {
        if (_vehicle.TeamId != teamId) return;

        print("Евент захват базы");
        _isCaptureTeamBase = isCapture;
    }
    private void OnReachedDistination()
    {
        if (_behaviourType == AIBehaviourType.Patrol)
        {
            _movementTarget = AIPath.Instance.GetRandomPatrolPoint();
        }

        _movement.ResetPath();
    }
    private void OnTeamMemberDestroyed(Destructible dest)
    {
        _countTeamMember--;
        dest.Destroyed -= OnTeamMemberDestroyed;

        if ((float)_countTeamMember / (float)_startCountTeamMember < 0.4f)
        { 
            StartBehaviour(AIBehaviourType.Patrol);
        }

        if (_countTeamMember <= 1)
        {
            StartBehaviour(AIBehaviourType.Patrol);
        }
    }

    private void UpdateBehaviour()
    {
        //TODO
        //_shooter.FindTarget();

        if (_isCaptureTeamBase == true && _defBaseTank <= 2)
        {
            OnDefendBase();
            return;
        }

        if (_movement.ReachedDistination == true)
        {
            OnReachedDistination();
        }

        if (_movement.HasPatch == false)
        {
            _movement.SetDestination(_movementTarget);
        }

    }

    private void OnDefendBase()
    {
        if (_defBaseTank < 3)
        {
            print("Выбор защитников");
            Vehicle[] v = FindObjectsOfType<Vehicle>();

            float minDist = float.MaxValue;
            for (int i = 0; i < v.Length; i++)
            {
                if (v[i].TeamId == _vehicle.TeamId && v[i].GetComponent<AITank>().behaviourType != AIBehaviourType.DefendBase)
                {
                    float dist = Vector3.Distance(_teamBase.transform.position, v[i].transform.position);
                    if (minDist < dist)
                    {
                        minDist = dist;
                    }
                }
            }
            float dist2 = Vector3.Distance(_teamBase.transform.position, transform.position);
            if (dist2 > minDist && _defBaseTank > 2) return;
            else
            {
                print("назначения защитников");
                _defBaseTank++;
                _behaviourType = AIBehaviourType.DefendBase;
                _movementTarget = _teamBase.transform.position;
                _movement.SetDestination(_movementTarget);
            }
        }
    }
    #endregion
}
