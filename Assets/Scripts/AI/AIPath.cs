using UnityEngine;

public class AIPath : MonoBehaviour
{
    public static AIPath Instance;

    [SerializeField] private Transform _baseRedPoint;
    [SerializeField] private Transform _baseBluePoint;

    [SerializeField] private Transform[] _fireRedPoint;
    [SerializeField] private Transform[] _fireBluePoint;

    [SerializeField] private Transform[] _patrolPoint;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetBasePoint(int teamId)
    {
        if (teamId == TeamSide.TeamRed)
        {
            return _baseBluePoint.position;
        }

        if (teamId == TeamSide.TeamBlue)
        { 
            return _baseRedPoint.position;
        }

        return Vector3.zero;
    }

    public Vector3 GetRandomFirePoint(int teamId)
    {
        if (teamId == TeamSide.TeamRed)
        {
            return _fireRedPoint[Random.Range(0, _fireRedPoint.Length)].position;
        }

        if (teamId == TeamSide.TeamBlue)
        {
            return _fireBluePoint[Random.Range(0, _fireBluePoint.Length)].position;
        }

        return Vector3.zero;
    }

    public Vector3 GetRandomPatrolPoint()
    {
        return _patrolPoint[Random.Range(0, _patrolPoint.Length)].position;
    }
}
