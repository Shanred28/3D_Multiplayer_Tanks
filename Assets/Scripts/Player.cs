using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public static Player Local
    {
        get 
        {
            var x = NetworkClient.localPlayer;

            if(x != null)
                return x.GetComponent<Player>();

            return null;
        }
    }

    private static int TeamIdCounter;

    public Vehicle activeVehicle { get; set; }

    [SerializeField] private Vehicle[] _vehiclePrefs;

    [Header("Player")]
    [SyncVar(hook = nameof(OnNicknameChanged))]
    public string Nickname;

    [SyncVar]
    [SerializeField] private int _teamId;
    public int TeamId => _teamId;

    private void OnNicknameChanged(string oldValue, string newValue)
    { 
        gameObject.name = "Player_" + newValue; 
    }

    [Command]
    public void CmdSetName(string name)
    { 
        Nickname = name;
        gameObject.name = "Player_" + name;
    }

    [Command]
    public void CmdSetTeamId(int teamId)
    { 
        _teamId = teamId;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        _teamId = TeamIdCounter % 2;
        TeamIdCounter++;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isOwned == true)
        {
            CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);
        }
    }


    private void Update()
    {
        if (isLocalPlayer == true)
        {
            if (activeVehicle != null)
            {
                activeVehicle.SetVisibile(!VehicleCamera.Instance.IsZoom);
            }
        }

        if (isServer == true)
        {
            if (Input.GetKeyDown(KeyCode.F9))
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
                    p.SvSpawnClientVeehicle();
                }
            }
        }

        if (isOwned == true)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                if (Cursor.lockState != CursorLockMode.Locked)
                    Cursor.lockState = CursorLockMode.Locked;
                else
                    Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    [Server]
    public void SvSpawnClientVeehicle()
    {
        if (activeVehicle != null) return;

        GameObject playerVehicle = Instantiate(_vehiclePrefs[Random.Range(0, _vehiclePrefs.Length)].gameObject, transform.position, Quaternion.identity);
        playerVehicle.transform.position = _teamId % 2 == 0 ?
            NetworkSessionManager.Instance.RandomSpawnPointRed : NetworkSessionManager.Instance.RandomSpawnPintBlue;

        NetworkServer.Spawn(playerVehicle, netIdentity.connectionToClient);
       

        activeVehicle = playerVehicle.GetComponent<Vehicle>();        
        activeVehicle.Owner = netIdentity;

        RpcSetVehicle(activeVehicle.netIdentity);
    }

    [ClientRpc]
    private void RpcSetVehicle(NetworkIdentity vehicle)
    {
        if (vehicle == null) return;

        activeVehicle = vehicle.GetComponent<Vehicle>();

        if (activeVehicle != null && activeVehicle.isOwned && VehicleCamera.Instance != null)
        {
            VehicleCamera.Instance.SetTarget(activeVehicle);
        }
    }
    
}
