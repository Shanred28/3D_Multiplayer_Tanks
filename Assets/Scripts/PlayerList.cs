using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class PlayerList : NetworkBehaviour
{
    public static PlayerList Instance;
    public static UnityAction<List<PlayerData>> UpdatePlayerList;
    [SerializeField] private List<PlayerData> _allPlayerData = new List<PlayerData>();

    private void Awake()
    {
        Instance = this;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        _allPlayerData.Clear();
    }
      
    [Server]
    public void SvAddPlayer(PlayerData playerData )
    {
        PlayerData data = new PlayerData(playerData.Id, playerData.Nickname, playerData.TeamId);
        _allPlayerData.Add(data);

         RpcClearPlayerDataList();

        for (int i = 0; i < _allPlayerData.Count; i++)
        {
            RpcAddPlayer(_allPlayerData[i].Id, _allPlayerData[i].Nickname, _allPlayerData[i].TeamId);
        }
    }

    [Server]
    public void SvRemovePlayer(PlayerData playerData)
    {
        for (int i = 0; i < _allPlayerData.Count; i++)
        {
            if (_allPlayerData[i].Id == playerData.Id)
            {
                _allPlayerData.RemoveAt(i);
                break;
            }                                  
        }

        RpcRemovePlayer(playerData.Id);
    }

    [ClientRpc]
    private void RpcClearPlayerDataList()
    {
        //Check host
        if (isServer == true) return;
            _allPlayerData.Clear();
    }

    [ClientRpc]
    private void RpcAddPlayer(int userId, string userNickName, int teamId)
    {
        if (isClient == true && isServer == true)
        {
            UpdatePlayerList?.Invoke(_allPlayerData);
            return;
        }

        PlayerData data = new PlayerData(userId, userNickName, teamId);
        _allPlayerData.Add(data);
        UpdatePlayerList?.Invoke(_allPlayerData);
    }

    [ClientRpc]
    private void RpcRemovePlayer(int userId)
    {
        for (int i = 0; i < _allPlayerData.Count; i++)
        {
            if (_allPlayerData[i].Id == userId)
            {
                _allPlayerData.RemoveAt(i);
                break;
            }
        }

        UpdatePlayerList?.Invoke(_allPlayerData);
    }

    public int GetIdByNicName(string nickName)
    {
        for (int i = 0; i < _allPlayerData.Count; i++)
        {
            if (_allPlayerData[i].Nickname == nickName)
            {
                return _allPlayerData[i].Id;
            }
                    
        }
        return 0;
    }
}


