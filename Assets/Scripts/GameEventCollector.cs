using Mirror;
using UnityEngine.Events;

public class GameEventCollector : NetworkBehaviour
{
    public UnityAction<Vehicle> PlayerVehicleSpawned;

    [Server]
    public void SvOnAddPlayer()
    {
        RpcOnAddPlayer();
    }

    [ClientRpc]
    public void RpcOnAddPlayer()
    {
        Player.Local.VehicleSpawned += OnPlayerVehicleSpaned;
    }

    private void OnPlayerVehicleSpaned(Vehicle vehicle)
    {
        PlayerVehicleSpawned?.Invoke(vehicle);
    }
    
}
