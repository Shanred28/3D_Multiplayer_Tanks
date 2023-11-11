using Mirror;
using UnityEngine;
using UnityEngine.Events;


public class Player : MatchMember
{
    public static Player Local
    {
        get
        {
            var x = NetworkClient.localPlayer;

            if (x != null)
                return x.GetComponent<Player>();

            return null;
        }
    }
 
    public event UnityAction<Vehicle> VehicleSpawned;
    public event UnityAction<ProjectileHitResult> ProjectileHit;

    [SerializeField] private Vehicle _vehiclePrefs;
    [SerializeField] private VehicleInputControl _vehicleInputControl;

    public override void OnStartServer()
    {
        base.OnStartServer();

        _teamId = MatchController.GetNextTeam();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isOwned == true)
        {
            CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);

            NetworkSessionManager.Match.MatchStart += OnMatchStart;
            NetworkSessionManager.Match.MatchEnd += OnMatchEnd;

            _data = new MatchMemberData((int)netId, NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname, _teamId, netIdentity);

            CmdAddPlayer(MemberData);

            CmdUpdateData(_data);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Start()
    {
        _vehicleInputControl.enabled = false;
    }

    [Server]
    public void SvSpawnClientVehicle()
    {
        if (activeVehicle != null) return;

        GameObject playerVehicle = Instantiate(_vehiclePrefs.gameObject);
        playerVehicle.transform.position = _teamId % 2 == 0 ?
            NetworkSessionManager.Instance.RandomSpawnPointRed : NetworkSessionManager.Instance.RandomSpawnPintBlue;

        NetworkServer.Spawn(playerVehicle, netIdentity.connectionToClient);


        activeVehicle = playerVehicle.GetComponent<Vehicle>();
        activeVehicle.Owner = netIdentity;
        activeVehicle.TeamId = _teamId;

        RpcSetVehicle(activeVehicle.netIdentity);
    }

    [ClientRpc]
    private void RpcSetVehicle(NetworkIdentity vehicle)
    {
        if (vehicle == null) return;

        activeVehicle = vehicle.GetComponent<Vehicle>();
        activeVehicle.Owner = netIdentity;
        activeVehicle.TeamId = _teamId;

        if (activeVehicle != null && activeVehicle.isOwned && VehicleCamera.Instance != null)
        {
            VehicleCamera.Instance.SetTarget(activeVehicle);
        }

        VehicleSpawned?.Invoke(activeVehicle);
        
    }

    [Command]
    private void CmdAddPlayer(MatchMemberData playerData)
    {
        MatchMemberList.Instance.SvAddPlayer(playerData);
    }

    private void OnMatchStart()
    {
        _vehicleInputControl.enabled = true;
    }

    private void OnMatchEnd()
    {
        if (activeVehicle != null)
        {
            activeVehicle.SetTargetControl(Vector3.zero);
            _vehicleInputControl.enabled = false;
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (isOwned)
        {
            NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public override void OnStopServer()
    {
        base.OnStopServer();

        MatchMemberList.Instance.SvRemovePlayer(_data);
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
    public void SvInvokeProjectileHit(ProjectileHitResult hitResult)
    {
        ProjectileHit?.Invoke(hitResult);
        RpcInvokeProjectileHit(hitResult.type, hitResult.damage, hitResult.point);
    }

    [ClientRpc]
    public void RpcInvokeProjectileHit(ProjectileHitType type, float damage, Vector3 hitPoint)
    {
        ProjectileHitResult hitResult = new ProjectileHitResult();
        hitResult.damage = damage;
        hitResult.type = type;
        hitResult.point = hitPoint;

        ProjectileHit?.Invoke(hitResult);
    }
}
