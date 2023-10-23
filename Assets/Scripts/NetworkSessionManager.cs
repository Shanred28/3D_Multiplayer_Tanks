using Mirror;
using UnityEngine;

public class NetworkSessionManager : NetworkManager
{
    [SerializeField] private SphereArea[] _spawZoneRed;
    [SerializeField] private SphereArea[] _spawnZoneBlue;

    public Vector3 RandomSpawnPointRed => _spawZoneRed[Random.Range(0, _spawZoneRed.Length)].RandomInside;
    public Vector3 RandomSpawnPintBlue => _spawnZoneBlue[Random.Range(0, _spawnZoneBlue.Length)].RandomInside;

    public static NetworkSessionManager Instance => singleton as NetworkSessionManager;
    public static GameEventCollector Events => Instance._gameEventCollector;
    public static MatchController Match => Instance._matchController;

    public bool IsServer => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly);
    public bool IsClient => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ClientOnly);

    [SerializeField] private GameEventCollector _gameEventCollector;
    [SerializeField] private MatchController _matchController;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        _gameEventCollector.SvOnAddPlayer();
    }
}
