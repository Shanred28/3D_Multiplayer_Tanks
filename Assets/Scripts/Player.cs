using Mirror;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class  PlayerData
{
    public int Id;
    public string Nickname;
    public int TeamId;

    public PlayerData(int id, string nickname, int teamId)
    {
        Id = id;
        Nickname = nickname;
        TeamId = teamId;
    }
}

public static class PlayerDataReadWriter    
{
    public static void WritePlayerData(this NetworkWriter writer, PlayerData value)
    {
        writer.WriteInt(value.Id);
        writer.WriteString(value.Nickname);
        writer.WriteInt(value.TeamId);
    }

    public static PlayerData ReadPlayerData(this NetworkReader reader)
    {
        return new PlayerData(reader.ReadInt(), reader.ReadString(), reader.ReadInt());
    }
}

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
    public static UnityAction<int, int> ChangeFrags;

    public UnityAction<Vehicle> VehicleSpawned;

    public Vehicle activeVehicle { get; set; }

    [SerializeField] private Vehicle _vehiclePrefs;
    [SerializeField] private VehicleInputControl _vehicleInputControl;

    [Header("Player")]
    [SyncVar(hook = nameof(OnNicknameChanged))]
    public string Nickname;

    [SyncVar(hook = nameof(OnFragChanged))]
    private int _frags;
    public int Frags
    {
        get { return _frags; }

        set 
        { 
            _frags = value;
            ChangeFrags?.Invoke((int) netId, _frags);
        }
    }

    [SyncVar]
    private int _teamId;
    public int TeamId => _teamId;

    private PlayerData _playerData;
    public PlayerData PlayerData => _playerData;

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

    public override void OnStopServer()
    {
        base.OnStopServer();

        PlayerList.Instance.SvRemovePlayer(_playerData);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isOwned == true)
        {
            CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);
            NetworkSessionManager.Match.MatchEnd += OnMatchEnd;

           _playerData = new PlayerData((int)netId, NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname, _teamId);

            CmdAddPlayer(PlayerData);

            CmdUpdateData(_playerData);
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (isOwned)
        {
            NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
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
                    NetworkSessionManager.Match.SvRestartMatch();
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

        GameObject playerVehicle = Instantiate(_vehiclePrefs.gameObject);
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

        VehicleSpawned?.Invoke(activeVehicle);
        _vehicleInputControl.enabled = true;
    }

    [Command]
    private void CmdAddPlayer(PlayerData playerData)
    {
        PlayerList.Instance.SvAddPlayer(playerData);
    }

    [Command]
    private void CmdUpdateData(PlayerData playerData)
    { 
        _playerData = playerData;
    }

    private void OnMatchEnd()
    {
        if (activeVehicle != null)
        {
            activeVehicle.SetTargetControl(Vector3.zero);
            _vehicleInputControl.enabled = false;
        }
    }

    private void OnFragChanged(int old, int newValue)
    {
        ChangeFrags?.Invoke((int)netId, newValue);
    }
}
