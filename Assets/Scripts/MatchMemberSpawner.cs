using UnityEngine;
using Mirror;

public class MatchMemberSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _botPrefab;
    [Range(0, 15)]
    [SerializeField] private int _targetAmmountMeemberTeam;

    [Server]
    public void SvRespawnVehiclesAllMembers()
    {
        SvRespawnPlayerVehicle();
        SvReespawnBotVehicle();
    }

    [Server]
    private void SvRespawnPlayerVehicle()
    {
        foreach (var p in FindObjectsOfType<Player>())
        {
            if (p.activeVehicle != null)
            {
                NetworkServer.UnSpawn(p.activeVehicle.gameObject);
                Destroy(p.activeVehicle.gameObject);

                p.activeVehicle = null;
            }
        }

        foreach (var p in FindObjectsOfType<Player>())
        {
            p.SvSpawnClientVehicle();
        }
    }

    [Server]
    private void SvReespawnBotVehicle()
    {
        foreach (var b in FindObjectsOfType<Bot>())
        {
            NetworkServer.UnSpawn(b.gameObject);
            Destroy(b.activeVehicle.gameObject);            
        }

       int botAmmount = _targetAmmountMeemberTeam * 2 - MatchMemberList.Instance.MeemberDataCount;

        for (int i = 0; i < botAmmount; i++)
        {
            GameObject b = Instantiate(_botPrefab);
            NetworkServer.Spawn(b);
        }
    }
}
