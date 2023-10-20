using Mirror;
using UnityEngine;

public class NetworkSessionManager : NetworkManager
{
    [SerializeField] private SphereArea[] _spawZoneRed;
    [SerializeField] private SphereArea[] _spawnZoneBlue;

    public Vector3 RandomSpawnPointRed => _spawZoneRed[Random.Range(0, _spawZoneRed.Length)].RandomInside;
    public Vector3 RandomSpawnPintBlue => _spawnZoneBlue[Random.Range(0, _spawnZoneBlue.Length)].RandomInside;

    public static NetworkSessionManager Instance => singleton as NetworkSessionManager;

    public bool IsServer => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly);
    public bool IsClient => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ClientOnly);
}
